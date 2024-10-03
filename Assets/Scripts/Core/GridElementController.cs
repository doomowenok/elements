using System;
using System.Collections.Generic;
using Core.Element;
using Core.Element.Config;
using Core.Grid;
using Core.Input;
using Core.Session;
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
        private readonly SessionController _sessionController;
        private readonly IRenderOrderHelper _renderOrderHelper;
        private readonly ISessionSaver _sessionSaver;
        private readonly IMatcher _matcher;
        private readonly IGridRecalculationController _gridRecalculationController;
        private readonly ElementConfig _elementConfig;

        private readonly List<GridGameElement> _matchedElements = new List<GridGameElement>();

        public GridElementController(
            IInputSystem inputSystem, 
            SessionController sessionController, 
            IRenderOrderHelper renderOrderHelper,
            ISessionSaver sessionSaver,
            IMatcher matcher,
            IConfigProvider configs,
            IGridRecalculationController gridRecalculationController)
        {
            _inputSystem = inputSystem;
            _sessionController = sessionController;
            _renderOrderHelper = renderOrderHelper;
            _sessionSaver = sessionSaver;
            _matcher = matcher;
            _gridRecalculationController = gridRecalculationController;
            _elementConfig = configs.GetConfig<ElementConfig>();
        }

        public void Initialize()
        {
            _inputSystem.OnEndInput += (delta, element) => ChangeElementPosition(delta, element).Forget();
        }

        private async UniTask ChangeElementPosition(Vector3 delta, GridGameElement selectedElement)
        {
            float dot = Vector3.Dot(delta.normalized, Vector3.up);
            MoveType moveType = DetectMoveType(dot, delta.x);
            int2 selectedElementIndex = selectedElement.GridIndex;

            switch (moveType)
            {
                case MoveType.Up:
                    if (selectedElementIndex.x >= _sessionController.Elements.Length - 1) return;
                    if (_sessionController.Elements[selectedElementIndex.x + 1][selectedElementIndex.y] == null) return;

                    GridGameElement upperElement = _sessionController.Elements[selectedElementIndex.x + 1][selectedElementIndex.y];
                    int2 upperElementIndex = upperElement.GridIndex;

                    await SwapElements(selectedElement, selectedElementIndex, upperElement, upperElementIndex, moveType);
                    break;
                case MoveType.Down:
                    if(selectedElementIndex.x == 0) return;
                    
                    GridGameElement lowerElement = _sessionController.Elements[selectedElementIndex.x - 1][selectedElementIndex.y];
                    int2 lowerElementIndex = lowerElement.GridIndex;

                    await SwapElements(selectedElement, selectedElementIndex, lowerElement, lowerElementIndex, moveType);
                    break;
                case MoveType.Left:
                    if (selectedElementIndex.y == 0) return;
                    
                    GridGameElement leftElement = _sessionController.Elements[selectedElementIndex.x][selectedElementIndex.y - 1];
                    int2 leftElementIndex = leftElement == null 
                        ? new int2(selectedElementIndex.x, selectedElementIndex.y - 1) 
                        : leftElement.GridIndex;
                    
                    await SwapElements(selectedElement, selectedElementIndex, leftElement, leftElementIndex, moveType);

                    _gridRecalculationController.RecalculateGrid();
                    
                    break;
                case MoveType.Right:
                    if(selectedElementIndex.y == _sessionController.Elements[0].Length - 1) return;
                    
                    GridGameElement rightElement = _sessionController.Elements[selectedElementIndex.x][selectedElementIndex.y + 1];
                    
                    int2 rightElementIndex = rightElement == null 
                        ? new int2(selectedElementIndex.x, selectedElementIndex.y + 1)  
                        : rightElement.GridIndex;
                    
                    await SwapElements(selectedElement, selectedElementIndex, rightElement, rightElementIndex, moveType);
                    
                    _gridRecalculationController.RecalculateGrid();
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _sessionSaver.UpdateSaveData();

            FindMatches().Forget();
        }

        private async UniTask SwapElements(
            GridGameElement selectedElement, 
            int2 selectedElementIndex, 
            GridGameElement switchedElement, 
            int2 switchedElementIndex, 
            MoveType moveType)
        {
            if (selectedElement.Availability == ElementAvailabilityType.NotAvailable ||
                (switchedElement != null && switchedElement.Availability == ElementAvailabilityType.NotAvailable))
            {
                return;
            }
            
            Vector3 selectedElementPosition = _sessionController.Positions[selectedElementIndex.x][selectedElementIndex.y];
            Vector3 nextPosition = _sessionController.Positions[switchedElementIndex.x][switchedElementIndex.y];

            selectedElement.SetGridIndex(switchedElementIndex);

            if (switchedElement != null)
            {
                switchedElement.SetGridIndex(selectedElementIndex);
            }

            selectedElement.SetRenderOrder(_renderOrderHelper.GetRenderOrder(switchedElementIndex.x, switchedElementIndex.y));

            if (switchedElement != null)
            {
                switchedElement.SetRenderOrder(_renderOrderHelper.GetRenderOrder(selectedElementIndex.x, selectedElementIndex.y));
            }
            
            selectedElement.transform.DOMove(nextPosition, _elementConfig.MoveAcrossGridSpeed);

            if (switchedElement != null)
            {
                switchedElement.transform.DOMove(selectedElementPosition, _elementConfig.MoveAcrossGridSpeed);
            }

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
            
            _sessionController.Elements[nextRow][nextColumn] = selectedElement;
            _sessionController.Elements[selectedElementIndex.x][selectedElementIndex.y] = switchedElement == null ? null : switchedElement;

            await UniTask.WaitForSeconds(_elementConfig.MoveAcrossGridSpeed);
        }

        private async UniTask FindMatches()
        {
            while (true)
            {
                List<int2> matches = _matcher.FindMatches(_sessionController.Elements);

                if (matches.Count == 0) return;
            
                ProcessMatchElements(matches);
                StartDestroyMatchElements(matches);

                await UniTask.WaitForSeconds(_elementConfig.DelayBeforeDestroy);

                _gridRecalculationController.RecalculateGrid();
            }
        }

        private void StartDestroyMatchElements(List<int2> matches)
        {
            matches.ForEach(index => _sessionController.Elements[index.x][index.y] = null);
            
            foreach (GridGameElement element in _matchedElements)
            {
                element.Destroy(_elementConfig.DelayBeforeDestroy);
                element.SetAvailability(ElementAvailabilityType.NotAvailable);
            }
        }

        private void ProcessMatchElements(List<int2> matches)
        {
            _matchedElements.Clear();

            for (int i = 0; i < _sessionController.Elements.Length; i++)
            {
                for (int k = 0; k < _sessionController.Elements[0].Length; k++)
                {
                    GridGameElement element = _sessionController.Elements[i][k];
                    
                    if(element == null) continue;

                    if (matches.Contains(element.GridIndex))
                    {
                        _matchedElements.Add(element);
                    }
                }
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
}