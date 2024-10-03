using System;
using Unity.Mathematics;
using UnityEngine;

namespace Core.Element
{
    public sealed class GridGameElement : MonoBehaviour
    { 
        private static readonly int DestroyHash = Animator.StringToHash("Destroy");
        
        public event Action<GridGameElement> OnDestroy;
        
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Animator _animator;
        
        public ElementType Type { get; private set; }
        public int2 GridIndex { get; private set; }
        public ElementAvailabilityType Availability { get; private set; }


        public void SetElementType(ElementType type)
        {
            Type = type;
        }

        public void SetRenderOrder(int order)
        {
            _renderer.sortingOrder = order;
        }

        public void SetGridIndex(int2 index)
        {
            GridIndex = index;
        }

        public void SetAvailability(ElementAvailabilityType availability)
        {
            Availability = availability;
        }

        public void Destroy(float delay)
        {
            _animator.SetTrigger(DestroyHash);
            OnDestroy?.Invoke(this);
            Destroy(gameObject, delay);
        }
    }
}