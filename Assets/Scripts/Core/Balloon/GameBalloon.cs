using System;
using Services.Pool;
using UnityEngine;

namespace Core.Balloon
{
    public sealed class GameBalloon : MonoBehaviour, IPoolObject<GameBalloon>
    {
        public event Action<GameBalloon> OnEndMove;
        
        [SerializeField] private SpriteRenderer _renderer;

        private Vector3 _startPosition;
        private float _speed;
        private float _frequency;
        private bool _canMove;
        private float _amplitude;
        private Vector3 _endPosition;

        public void Initialize(
            Sprite sprite, 
            Vector3 startPosition, 
            Vector3 endPosition, 
            float speed, 
            float frequency,
            float amplitude)
        {
            _renderer.sprite = sprite;
            _startPosition = startPosition;
            _endPosition = endPosition;
            _speed = speed;
            _frequency = frequency;
            _amplitude = amplitude;
        }

        private void Update()
        {
            if (_canMove)
            {
                float x = transform.position.x + _speed * Time.deltaTime;
                float y = _startPosition.y + Mathf.Sin(Time.time * _frequency) * _amplitude;
                transform.position = new Vector3(x, y);

                if (transform.position.x >= _endPosition.x)
                {
                    StopMove();
                    OnEndMove?.Invoke(this);
                }
            }
        }

        public void StartMove()
        {
            _canMove = true;
        }

        public void StopMove()
        {
            _canMove = false;
        }

        GameBalloon IPoolObject<GameBalloon>.PoolObject => this;
        void IPoolObject<GameBalloon>.ReturnToPool()
        {
            _startPosition = Vector3.zero;
            _speed = 0.0f;
            _frequency = 0.0f;
            StopMove();
            gameObject.SetActive(false);
        }
    }
}