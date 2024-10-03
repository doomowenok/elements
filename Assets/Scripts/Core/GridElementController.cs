using System;
using System.Collections.Generic;
using System.Text;
using Core.Element;
using Core.Element.Config;
using Core.Element.Factory;
using Core.Grid;
using Core.Input;
using Core.Session;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Services;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core
{
    public sealed class GridElementController : IGridElementController
    {
        private const float DotMoveCheck = 0.5f;
        
        private readonly IInputSystem _inputSystem;
        private readonly SessionData _sessionData;
        private readonly IRenderOrderHelper _renderOrderHelper;
        private readonly ISessionSaver _sessionSaver;
        private readonly IMatcher _matcher;
        private readonly IGridGameElementFactory _elementFactory;
        private readonly ElementConfig _elementConfig;

        private readonly List<GridGameElement> _matchedElements = new List<GridGameElement>();
        private readonly List<GridGameElement> _elementsInColumns = new List<GridGameElement>();
        private readonly List<MatchData> _elementsToMove = new List<MatchData>();

        public GridElementController(
            IInputSystem inputSystem, 
            SessionData sessionData, 
            IRenderOrderHelper renderOrderHelper,
            ISessionSaver sessionSaver,
            IMatcher matcher,
            IConfigProvider configs,
            IGridGameElementFactory elementFactory)
        {
            _inputSystem = inputSystem;
            _sessionData = sessionData;
            _renderOrderHelper = renderOrderHelper;
            _sessionSaver = sessionSaver;
            _matcher = matcher;
            _elementFactory = elementFactory;
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
                    if (selectedElementIndex.x >= _sessionData.Elements.Length - 1) return;
                    if (_sessionData.Elements[selectedElementIndex.x + 1][selectedElementIndex.y] == null) return;

                    GridGameElement upperElement = _sessionData.Elements[selectedElementIndex.x + 1][selectedElementIndex.y];
                    int2 upperElementIndex = upperElement.GridIndex;

                    SwapElements(selectedElement, selectedElementIndex, upperElementIndex, upperElement, moveType);
                    break;
                case MoveType.Down:
                    if(selectedElementIndex.x == 0) return;
                    
                    GridGameElement lowerElement = _sessionData.Elements[selectedElementIndex.x - 1][selectedElementIndex.y];
                    int2 lowerElementIndex = lowerElement.GridIndex;

                    SwapElements(selectedElement, selectedElementIndex, lowerElementIndex, lowerElement, moveType);
                    break;
                case MoveType.Left:
                    if (selectedElementIndex.y == 0) return;
                    if(_sessionData.Elements[selectedElementIndex.x][selectedElementIndex.y - 1] == null) return;
                    
                    GridGameElement leftElement = _sessionData.Elements[selectedElementIndex.x][selectedElementIndex.y - 1];
                    int2 leftElementIndex = leftElement.GridIndex;
                    
                    SwapElements(selectedElement, selectedElementIndex, leftElementIndex, leftElement, moveType);
                    break;
                case MoveType.Right:
                    if(selectedElementIndex.y == _sessionData.Elements[0].Length - 1) return;
                    if(_sessionData.Elements[selectedElementIndex.x][selectedElementIndex.y + 1] == null) return;
                    
                    GridGameElement rightElement = _sessionData.Elements[selectedElementIndex.x][selectedElementIndex.y + 1];
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
            
            Vector3 selectedElementPosition = _sessionData.Positions[selectedElementIndex.x][selectedElementIndex.y];
            Vector3 upperPosition = _sessionData.Positions[switchedElementIndex.x][switchedElementIndex.y];

            selectedElement.SetGridIndex(switchedElementIndex);
            switchedElement.SetGridIndex(selectedElementIndex);

            selectedElement.SetRenderOrder(_renderOrderHelper.GetRenderOrder(switchedElementIndex.x, switchedElementIndex.y));
            switchedElement.SetRenderOrder(_renderOrderHelper.GetRenderOrder(selectedElementIndex.x, selectedElementIndex.y));
                    
            selectedElement.transform.DOMove(upperPosition, 0.2f);
            switchedElement.transform.DOMove(selectedElementPosition, 0.2f);

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
            
            _sessionData.Elements[nextRow][nextColumn] = selectedElement;
            _sessionData.Elements[selectedElementIndex.x][selectedElementIndex.y] = switchedElement;
        }

        private async void FindMatches()
        {
            _elementsToMove.Clear();
            
            List<int2> matches = _matcher.FindMatches(_sessionData.Elements);

            if (matches.Count == 0) return;
            
            _matchedElements.Clear();
            
            for (int i = 0; i < _sessionData.Elements.Length; i++)
            {
                for (int k = 0; k < _sessionData.Elements[0].Length; k++)
                {
                    GridGameElement element = _sessionData.Elements[i][k];
                    
                    if(element == null) continue;

                    if (matches.Contains(element.GridIndex))
                    {
                        _matchedElements.Add(element);
                    }
                }
            }
            
            _matchedElements.ForEach(element =>
            {
                element.SetAvailability(ElementAvailabilityType.NotAvailable);
                element.Destroy(_elementConfig.DelayBeforeDestroy);
            });

            matches.ForEach(index => _sessionData.Elements[index.x][index.y] = null);

            await UniTask.WaitForSeconds(_elementConfig.DelayBeforeDestroy + 2f);
            
            _elementsToMove.Clear();
            
            for (int i = 0; i < _sessionData.Elements[0].Length; i++)
            {
                bool hasAnyElementInColumns = false;
                
                for (int k = 0; k < _sessionData.Elements.Length && !hasAnyElementInColumns; k++)
                {
                    GridGameElement element = _sessionData.Elements[k][i];
                    
                    if (element != null)
                    {
                        hasAnyElementInColumns = true;
                    }
                }
                
                if(!hasAnyElementInColumns) continue;

                bool anyNotHold = false;
                for (int k = 0; k < _sessionData.Elements.Length && !anyNotHold; k++)
                {
                    GridGameElement element = _sessionData.Elements[k][i];
                    
                    if (element == null) anyNotHold = true;
                }
                
                if(!anyNotHold) continue;

                bool notStableInColumn = false;

                if (_sessionData.Elements[0][i] == null)
                {
                    notStableInColumn = true;
                }
                else
                {
                    for (int k = 1; k < _sessionData.Elements.Length && !notStableInColumn; k++)
                    {
                        GridGameElement element = _sessionData.Elements[k][i];

                        if (element != null && _sessionData.Elements[k - 1][i] == null)
                        {
                            notStableInColumn = true;
                        }
                    }
                }
                
                if(!notStableInColumn) continue;
                
                _elementsInColumns.Clear();
                
                for (int k = 0; k < _sessionData.Elements.Length; k++)
                {
                    GridGameElement element = _sessionData.Elements[k][i];
                    
                    if (element != null)
                    {
                        _elementsInColumns.Add(element);
                    }
                }
                
                int2 index = new int2(0, i);
                
                for (int c = 0; c < _elementsInColumns.Count; c++)
                {
                    _elementsToMove.Add(new MatchData()
                    {
                        Element = _elementsInColumns[c],
                        Index = index,
                        PreviousIndex = _elementsInColumns[c].GridIndex,
                    });

                    index += new int2(1, 0);
                }
            }
            
            for (int j = 0; j < _elementsToMove.Count; j++)
            {
                MatchData matchData = _elementsToMove[j];
                int2 elementIndex = matchData.Index;
                int2 previousIndex = matchData.PreviousIndex;
                GridGameElement element = matchData.Element;
                float moveTime = Mathf.Abs(previousIndex.x - elementIndex.x) * 0.2f;
                _sessionData.Elements[elementIndex.x][elementIndex.y] = element;
                // _sessionData.Elements[previousIndex.x][previousIndex.y] = null;
                Vector3 position = _sessionData.Positions[elementIndex.x][elementIndex.y];
                element.SetGridIndex(elementIndex);
                element.transform.DOMove(position, moveTime);
                element.SetRenderOrder(_renderOrderHelper.GetRenderOrder(elementIndex.x, elementIndex.y));
            }
            
            for (int i = 0; i < _sessionData.Elements.Length; i++)
            {
                for (int j = 0; j < _sessionData.Elements[i].Length; j++)
                {
                    _sessionData.Elements[i][j] = null;
                }
            }

            IEnumerable<GridGameElement> elements = _elementFactory.GetAllActiveElements();
            
            foreach (GridGameElement element in elements)
            {
                int2 index = element.GridIndex;
                _sessionData.Elements[index.x][index.y] = element;
            }
            

            Debug.Log(_sessionData);
            
            _inputSystem.EnableInput();
        }

        class MatchData
        {
            public int2 Index;
            public int2 PreviousIndex;
            public GridGameElement Element;
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
}