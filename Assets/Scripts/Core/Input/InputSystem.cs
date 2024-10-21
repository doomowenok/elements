using System;
using Core.Element;
using Core.Grid;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Input
{
    public sealed class InputSystem : ITickable, IInputSystem
    {
        public event Action<Vector3, GridGameElement> OnEndInput;
        
        private Camera _camera;
        private ICollisionDetector _collisionDetector;
        
        private GridGameElement _lastTouchedGridElement;
        private Vector3 _startTouchSelectPosition;
        private Vector3 _endTouchSelectPosition;
        private bool _inputEnabled;

        [Inject]
        private void Construct(ICollisionDetector collisionDetector)
        {
            _collisionDetector = collisionDetector;
        }

        public void EnableInput()
        {
            _camera = Camera.main;
            _inputEnabled = true;
        }

        public void DisableInput()
        {
            _inputEnabled = false;
        }

        void ITickable.Tick()
        {
            if (!_inputEnabled || _camera == null) return;
            
            if (AnyInput())
            {
                _endTouchSelectPosition = GetWorldTouchPosition();
                
                if(_lastTouchedGridElement != null) return;
                
                RaycastHit2D hit = Physics2D.Raycast(GetWorldTouchPosition(), Vector3.forward * 20.0f);
                
                if (hit != default)
                {
                    int id = hit.collider.gameObject.GetInstanceID();
                    if (_collisionDetector.Contains(id))
                    {
                        _lastTouchedGridElement = _collisionDetector.GetGridGameElementByID(id);
                        _startTouchSelectPosition = GetWorldTouchPosition();
                    }
                }
            }
            else
            {
                if (_lastTouchedGridElement != null)
                {
                    Vector3 delta = _endTouchSelectPosition - _startTouchSelectPosition;
                    OnEndInput?.Invoke(delta, _lastTouchedGridElement);
                    ClearTouchData();
                }
            }
        }

        private Vector3 GetWorldTouchPosition()
        {
            Touch touch = UnityEngine.Input.GetTouch(0);
            Vector3 worldTouchPosition = _camera.ScreenToWorldPoint(touch.position);
            return worldTouchPosition;
        }

        private void ClearTouchData()
        {
            _startTouchSelectPosition = Vector3.zero;
            _endTouchSelectPosition = Vector3.zero;
            _lastTouchedGridElement = null;
        }

        private static bool AnyInput()
        {
            return UnityEngine.Input.touchCount > 0;
        }
    }
}