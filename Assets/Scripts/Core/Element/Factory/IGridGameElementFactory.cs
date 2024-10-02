using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace Core.Element.Factory
{
    public interface IGridGameElementFactory
    {
        UniTask<GridGameElement> Create(ElementType type, int2 gridIndex, Vector3 position, Vector3 scale, Transform parent = null);
    }
}