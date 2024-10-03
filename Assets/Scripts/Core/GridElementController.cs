using System;
using System.Collections.Generic;
using Core.Element;
using Core.Element.Config;
using Core.Element.Factory;
using Core.Grid;
using Core.Input;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Services;
using Unity.Mathematics;
using UnityEngine;

namespace Core
{
    public sealed class GridElementController : IGridElementController
    {
        private const float DotMoveCheck = 0.5f;
        
        private readonly IInputSystem _inputSystem;
        private readonly Session.Session _session;
        private readonly IRenderOrderHelper _renderOrderHelper;
        private readonly ISessionSaver _sessionSaver;
        private readonly IMatcher _matcher;
        private readonly IGridGameElementFactory _elementFactory;
        private readonly IGridRecalculationController _gridRecalculationController;
        private readonly ElementConfig _elementConfig;

        private readonly List<GridGameElement> _matchedElements = new List<GridGameElement>();
        private readonly List<GridGameElement> _elementsInColumns = new List<GridGameElement>();
        private readonly List<AfterMergeData> _elementsToMove = new List<AfterMergeData>();

        public GridElementController(
            IInputSystem inputSystem, 
            Session.Session session, 
            IRenderOrderHelper renderOrderHelper,
            ISessionSaver sessionSaver,
            IMatcher matcher,
            IConfigProvider configs,
            IGridGameElementFactory elementFactory,
            IGridRecalculationController gridRecalculationController)
        {
            _inputSystem = inputSystem;
            _session = session;
            _renderOrderHelper = renderOrderHelper;
            _sessionSaver = sessionSaver;
            _matcher = matcher;
            _elementFactory = elementFactory;
            _gridRecalculationController = gridRecalculationController;
            _elementConfig = configs.GetConfig<ElementConfig>();
        }

        public void Initialize()
        {
            _inputSystem.OnEndInput += ChangeElementPosition;
        }

        private void ChangeElementPosition(Vector3 delta, GridGameElement selectedElement)
        {
            float dot = Vector3.Dot(delta.normalized, Vector3.up);
            MoveType moveType = DetectMoveType(dot, delta.x);
            int2 selectedElementIndex = selectedElement.GridIndex;

            switch (moveType)
            {
                case MoveType.Up:
                    if (selectedElementIndex.x >= _session.Elements.Length - 1) return;
                    if (_session.Elements[selectedElementIndex.x + 1][selectedElementIndex.y] == null) return;

                    GridGameElement upperElement = _session.Elements[selectedElementIndex.x + 1][selectedElementIndex.y];
                    int2 upperElementIndex = upperElement.GridIndex;

                    SwapElements(selectedElement, selectedElementIndex, upperElementIndex, upperElement, moveType);
                    break;
                case MoveType.Down:
                    if(selectedElementIndex.x == 0) return;
                    
                    GridGameElement lowerElement = _session.Elements[selectedElementIndex.x - 1][selectedElementIndex.y];
                    int2 lowerElementIndex = lowerElement.GridIndex;

                    SwapElements(selectedElement, selectedElementIndex, lowerElementIndex, lowerElement, moveType);
                    break;
                case MoveType.Left:
                    if (selectedElementIndex.y == 0) return;
                    if(_session.Elements[selectedElementIndex.x][selectedElementIndex.y - 1] == null) return;
                    
                    GridGameElement leftElement = _session.Elements[selectedElementIndex.x][selectedElementIndex.y - 1];
                    int2 leftElementIndex = leftElement.GridIndex;
                    
                    SwapElements(selectedElement, selectedElementIndex, leftElementIndex, leftElement, moveType);
                    break;
                case MoveType.Right:
                    if(selectedElementIndex.y == _session.Elements[0].Length - 1) return;
                    if(_session.Elements[selectedElementIndex.x][selectedElementIndex.y + 1] == null) return;
                    
                    GridGameElement rightElement = _session.Elements[selectedElementIndex.x][selectedElementIndex.y + 1];
                    int2 rightElementIndex = rightElement.GridIndex;
                    
                    SwapElements(selectedElement, selectedElementIndex, rightElementIndex, rightElement, moveType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _sessionSaver.UpdateSaveData();
            
            FindMatches();
        }

        private void SwapElements(GridGameElement selectedElement, int2 selectedElementIndex, int2 switchedElementIndex, GridGameElement switchedElement, MoveType moveType)
        {
            if (selectedElement.Availability == ElementAvailabilityType.NotAvailable ||
                switchedElement.Availability == ElementAvailabilityType.NotAvailable)
            {
                return;
            }
            
            Vector3 selectedElementPosition = _session.Positions[selectedElementIndex.x][selectedElementIndex.y];
            Vector3 upperPosition = _session.Positions[switchedElementIndex.x][switchedElementIndex.y];

            selectedElement.SetGridIndex(switchedElementIndex);
            switchedElement.SetGridIndex(selectedElementIndex);

            selectedElement.SetRenderOrder(_renderOrderHelper.GetRenderOrder(switchedElementIndex.x, switchedElementIndex.y));
            switchedElement.SetRenderOrder(_renderOrderHelper.GetRenderOrder(selectedElementIndex.x, selectedElementIndex.y));
                    
            selectedElement.transform.DOMove(upperPosition, 0.1f);
            switchedElement.transform.DOMove(selectedElementPosition, 0.1f);

            int nextRow = 0;
            int nextColumn = 0;

            switch (moveType)
            {
                case MoveType.Up:
                    nextRow = selectedElementIndex.x + 1;
                    nextColumn = selectedElementIndex.y;
                    break;
                case MoveType.Down:
                    nextRow = selectedElementIndex.x - 1;
                    nextColumn = selectedElementIndex.y;
                    break;
                case MoveType.Left:
                    nextRow = selectedElementIndex.x;
                    nextColumn = selectedElementIndex.y - 1;
                    break;
                case MoveType.Right:
                    nextRow = selectedElementIndex.x;
                    nextColumn = selectedElementIndex.y + 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(moveType), moveType, null);
            }
            
            _session.Elements[nextRow][nextColumn] = selectedElement;
            _session.Elements[selectedElementIndex.x][selectedElementIndex.y] = switchedElement;
        }

        private async void FindMatches()
        {
            _elementsToMove.Clear();
            
            List<int2> matches = _matcher.FindMatches(_session.Elements);

            if (matches.Count == 0) return;
            
            ProcessMatchElements(matches);
            StartDestroyMatchElements(matches);

            await UniTask.WaitForSeconds(_elementConfig.DelayBeforeDestroy);

            _gridRecalculationController.RecalculateGrid();
            RecalculateGrid();
        }

        private void RecalculateGrid()
        {
            _elementsToMove.Clear();
            
            for (int column = 0; column < _session.Elements[0].Length; column++)
            {
                if(!HasAnyElementInColumns(column)) continue;
                if(!AnyNotHold(column)) continue;
                if(!NotStableInColumn(column)) continue;
                
                FillElementsInColumn(column);
                FillAfterMergeData(column);
            }
            
            MoveElementsToNewPlaces();
            
            _session.ClearElements();
            _session.FillElements(_elementFactory.GetAllActiveElements());
            
            Debug.Log(_session);
        }

        private void FillElementsInColumn(int column)
        {
            _elementsInColumns.Clear();

            for (int k = 0; k < _session.Elements.Length; k++)
            {
                GridGameElement element = _session.Elements[k][column];
                    
                if (element != null)
                {
                    _elementsInColumns.Add(element);
                }
            }
        }

        private void FillAfterMergeData(int column)
        {
            int2 index = new int2(0, column);

            for (int c = 0; c < _elementsInColumns.Count; c++)
            {
                _elementsToMove.Add(new AfterMergeData()
                {
                    Element = _elementsInColumns[c],
                    Index = index,
                    PreviousIndex = _elementsInColumns[c].GridIndex,
                });

                index += new int2(1, 0);
            }
        }

        private bool NotStableInColumn(int column)
        {
            bool notStableInColumn = false;

            if (_session.Elements[0][column] == null)
            {
                notStableInColumn = true;
            }
            else
            {
                for (int k = 1; k < _session.Elements.Length && !notStableInColumn; k++)
                {
                    GridGameElement element = _session.Elements[k][column];

                    if (element != null && _session.Elements[k - 1][column] == null)
                    {
                        notStableInColumn = true;
                    }
                }
            }

            return notStableInColumn;
        }

        private bool AnyNotHold(int column)
        {
            bool anyNotHold = false;
            for (int k = 0; k < _session.Elements.Length && !anyNotHold; k++)
            {
                GridGameElement element = _session.Elements[k][column];
                    
                if (element == null) anyNotHold = true;
            }

            return anyNotHold;
        }

        private bool HasAnyElementInColumns(int column)
        {
            bool hasAnyElementInColumns = false;

            for (int k = 0; k < _session.Elements.Length && !hasAnyElementInColumns; k++)
            {
                GridGameElement element = _session.Elements[k][column];
                    
                if (element != null)
                {
                    hasAnyElementInColumns = true;
                }
            }

            return hasAnyElementInColumns;
        }

        private void StartDestroyMatchElements(List<int2> matches)
        {
            _matchedElements.ForEach(element =>
            {
                element.SetAvailability(ElementAvailabilityType.NotAvailable);
                element.Destroy(_elementConfig.DelayBeforeDestroy);
            });

            matches.ForEach(index => _session.Elements[index.x][index.y] = null);
        }

        private void ProcessMatchElements(List<int2> matches)
        {
            _matchedElements.Clear();

            for (int i = 0; i < _session.Elements.Length; i++)
            {
                for (int k = 0; k < _session.Elements[0].Length; k++)
                {
                    GridGameElement element = _session.Elements[i][k];
                    
                    if(element == null) continue;

                    if (matches.Contains(element.GridIndex))
                    {
                        _matchedElements.Add(element);
                    }
                }
            }
        }

        private void MoveElementsToNewPlaces()
        {
            foreach (AfterMergeData data in _elementsToMove)
            {
                int2 elementIndex = data.Index;
                int2 previousIndex = data.PreviousIndex;
                GridGameElement element = data.Element;
                float moveTime = Mathf.Abs(previousIndex.x - elementIndex.x) * 0.1f;
                _session.Elements[elementIndex.x][elementIndex.y] = element;
                Vector3 position = _session.Positions[elementIndex.x][elementIndex.y];
                element.SetGridIndex(elementIndex);
                element.transform.DOMove(position, moveTime);
                element.SetRenderOrder(_renderOrderHelper.GetRenderOrder(elementIndex.x, elementIndex.y));
            }
        }

        private MoveType DetectMoveType(float dot, float xDirection)
        {
            MoveType moveType = dot switch
            {
                > DotMoveCheck => MoveType.Up,
                < -DotMoveCheck => MoveType.Down,
                _ => xDirection > 0.0f ? MoveType.Right : MoveType.Left
            };

            return moveType;
        }
    }

    public interface IGridRecalculationController
    {
        void RecalculateGrid();
    }

    public sealed class GridRecalculationController : IGridRecalculationController
    {
        public void RecalculateGrid()
        {
            
        }
    }
}