using Core.Config;
using Core.Element.Config;
using Services;
using Unity.Mathematics;
using UnityEngine;

namespace Core
{
    public sealed class TransformCalculator : ITransformCalculator
    {
        private readonly GameConfig _gameConfig;
        private readonly ElementConfig _elementConfig;

        public TransformCalculator(IConfigProvider configs)
        {
            _gameConfig = configs.GetConfig<GameConfig>();
            _elementConfig = configs.GetConfig<ElementConfig>();
        }
        
        public Vector3 GetScale(int elementsCount)
        {
            float elementSize = GetElementSize(elementsCount);
            float updatedScale = elementSize / _elementConfig.ElementSize.x; 
            Vector3 scale = Vector3.one * updatedScale;
            return scale;
        }

        public Vector3 GetStartSpawnPosition(int elementsCount)
        {
            StartPointsProvider startPointsProvider = Object.FindObjectOfType<StartPointsProvider>();
            Vector3 downCenterPosition = startPointsProvider.StartDownPoint.position;
            float elementSize = _gameConfig.DefaultGridWidth / elementsCount;
            Vector3 offset = new Vector3((elementSize * elementsCount / 2) - elementSize / 2, 0.0f);
            Vector3 startSpawnPosition = downCenterPosition - offset;
            return startSpawnPosition;
        }

        public Vector3 GetSpawnPosition(int elementsCount, int2 elementIndex)
        {
            float elementSize = GetElementSize(elementsCount);
            Vector3 spawnPosition = GetStartSpawnPosition(elementsCount) + new Vector3(elementSize * elementIndex.y, elementSize * elementIndex.x);
            return spawnPosition;
        }

        public float GetElementSize(int elementsCount) => 
            _gameConfig.DefaultGridWidth / elementsCount;
    }
}