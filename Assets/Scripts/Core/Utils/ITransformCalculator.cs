using Unity.Mathematics;
using UnityEngine;

namespace Core.Utils
{
    public interface ITransformCalculator
    {
        Vector3 GetScale(int elementsCount);
        Vector3 GetSpawnPosition(int elementsCount, int2 elementIndex);
    }
}