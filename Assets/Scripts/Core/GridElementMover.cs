using System;
using Core.Element;
using Core.Input;
using Core.Session;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

namespace Core
{
    public sealed class GridElementMover : IGridElementMover
    {
        private const float DotMoveCheck = 0.5f;
        
        private readonly IInputSystem _inputSystem;
        private readonly SessionData _sessionData;
        private readonly IRenderOrderHelper _renderOrderHelper;
        private readonly ISessionSaver _sessionSaver;

        public GridElementMover(
            IInputSystem inputSystem, 
            SessionData sessionData, 
            IRenderOrderHelper renderOrderHelper,
            ISessionSaver sessionSaver)
        {
            _inputSystem = inputSystem;
            _sessionData = sessionData;
            _renderOrderHelper = renderOrderHelper;
            _sessionSaver = sessionSaver;
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
        }

        private void SwapElements(GridGameElement selectedElement, int2 selectedElementIndex, int2 switchedElementIndex, GridGameElement switchedElement, MoveType moveType)
        {
            Vector3 currentElementPosition = _sessionData.Positions[selectedElementIndex.x][selectedElementIndex.y];
            Vector3 upperPosition = _sessionData.Positions[switchedElementIndex.x][switchedElementIndex.y];

            selectedElement.SetGridIndex(switchedElementIndex);
            switchedElement.SetGridIndex(selectedElementIndex);

            selectedElement.SetRenderOrder(_renderOrderHelper.GetRenderOrder(switchedElementIndex.x, switchedElementIndex.y));
            switchedElement.SetRenderOrder(_renderOrderHelper.GetRenderOrder(selectedElementIndex.x, selectedElementIndex.y));
                    
            selectedElement.transform.DOMove(upperPosition, 0.2f);
            switchedElement.transform.DOMove(currentElementPosition, 0.2f);

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

            _sessionSaver.UpdateSaveData();
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