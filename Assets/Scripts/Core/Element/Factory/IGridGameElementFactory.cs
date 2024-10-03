using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace Core.Element.Factory
{
    public interface IGridGameElementFactory
    {
        IReadOnlyList<GridGameElement> GetAllActiveElements();
        GridGameElement Create(ElementType type, int2 gridIndex, Vector3 position, Vector3 scale, Transform parent = null);
        UniTask DestroyAllElements();
    }
}