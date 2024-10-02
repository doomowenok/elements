using Unity.Mathematics;
using UnityEngine;

namespace Core
{
    public interface ITransformCalculator
    {
        Vector3 GetScale(int elementsCount);
        Vector3 GetStartSpawnPosition(int elementsCount);
        Vector3 GetSpawnPosition(int elementsCount, int2 elementIndex);
    }
}