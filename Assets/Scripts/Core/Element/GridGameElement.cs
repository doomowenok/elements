using System;
using Cysharp.Threading.Tasks;
using Services.Pool;
using Unity.Mathematics;
using UnityEngine;

namespace Core.Element
{
    public sealed class GridGameElement : MonoBehaviour, IPoolObject<GridGameElement>
    { 
        private static readonly int DestroyHash = Animator.StringToHash("Destroy");
        
        public event Action<GridGameElement> OnDestroy;
        
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Animator _animator;
        
        private bool _animationEnded;

        public ElementType Type { get; private set; }
        public int2 GridIndex { get; private set; }
        public ElementAvailabilityType Availability { get; private set; }

        public void Initialize(RuntimeAnimatorController animatorController, Sprite sprite)
        {
            _renderer.sprite = sprite;
            _animator.runtimeAnimatorController = animatorController;
        }

        public void SetElementType(ElementType type) => Type = type;
        public void SetRenderOrder(int order) => _renderer.sortingOrder = order;
        public void SetGridIndex(int2 index) => GridIndex = index;
        public void SetAvailability(ElementAvailabilityType availability) => Availability = availability;
        public void SetAnimationReceivedState(bool ended) => _animationEnded = ended;

        public async UniTask Destroy()
        {
            _animator.SetTrigger(DestroyHash);
            await UniTask.WaitWhile(AnimationNotEnded);
            OnDestroy?.Invoke(this);
        }

        private bool AnimationNotEnded() => 
            !_animationEnded;

        GridGameElement IPoolObject<GridGameElement>.PoolObject => this;

        void IPoolObject<GridGameElement>.ReturnToPool()
        {
            gameObject.SetActive(false);
            _animationEnded = false;
            Type = ElementType.None;
            GridIndex = new int2(int.MaxValue, int.MaxValue);
            Availability = ElementAvailabilityType.NotAvailable;
        }
    }
}