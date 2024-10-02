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
        private readonly IInputSystem _inputSystem;
        private readonly SessionData _sessionData;
        private readonly IRenderOrderHelper _renderOrderHelper;

        public GridElementMover(IInputSystem inputSystem, SessionData sessionData, IRenderOrderHelper renderOrderHelper)
        {
            _inputSystem = inputSystem;
            _sessionData = sessionData;
            _renderOrderHelper = renderOrderHelper;
        }

        public void Initialize()
        {
            _inputSystem.OnEndInput += ChangeElementPosition;
        }

        private void ChangeElementPosition(Vector3 delta, GridGameElement selectedElement)
        {
            float dot = Vector3.Dot(delta.normalized, Vector3.up);
            MoveType moveType = DetectMoveType(dot, delta.x);

            switch (moveType)
            {
                case MoveType.Up:
                    int2 selectedElementIndex = selectedElement.GridIndex;
                    
                    if (selectedElementIndex.x >= _sessionData.Elements.Length) return;
                    
                    int nextUpIndex = selectedElementIndex.x + 1;
                    
                    if (_sessionData.Elements[nextUpIndex][selectedElementIndex.y] == null) return;

                    GridGameElement upperElement = _sessionData.Elements[nextUpIndex][selectedElementIndex.y];
                    int2 upperElementIndex = upperElement.GridIndex;
                    
                    Vector3 currentElementPosition = _sessionData.Positions[selectedElementIndex.x][selectedElementIndex.y];
                    Vector3 upperPosition = _sessionData.Positions[upperElementIndex.x][upperElementIndex.y];

                    selectedElement.SetGridIndex(upperElementIndex);
                    upperElement.SetGridIndex(selectedElementIndex);

                    selectedElement.SetRenderOrder(_renderOrderHelper.GetRenderOrder(upperElementIndex.x, upperElementIndex.y));
                    upperElement.SetRenderOrder(_renderOrderHelper.GetRenderOrder(selectedElementIndex.x, selectedElementIndex.y));
                    
                    selectedElement.transform.DOMove(upperPosition, 0.2f);
                    upperElement.transform.DOMove(currentElementPosition, 0.2f);

                    _sessionData.Elements[nextUpIndex][selectedElementIndex.y] = selectedElement;
                    _sessionData.Elements[selectedElementIndex.x][selectedElementIndex.y] = upperElement;

                    break;
                case MoveType.Down:
                    break;
                case MoveType.Left:
                    break;
                case MoveType.Right:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private MoveType DetectMoveType(float dot, float xDirection)
        {
            MoveType moveType;
            
            if (dot > 0.5f)
            {
                moveType = MoveType.Up;
            }
            else if (dot < -0.5)
            {
                moveType = MoveType.Down;
            }
            else
            {
                moveType = xDirection > 0.0f ? MoveType.Right : MoveType.Left;
            }

            return moveType;
        }
    }
}