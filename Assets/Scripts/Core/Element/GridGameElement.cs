using Unity.Mathematics;
using UnityEngine;

namespace Core.Element
{
    public sealed class GridGameElement : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        
        public ElementType Type { get; private set; }
        public int2 GridIndex { get; private set; }
        
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
    }
}