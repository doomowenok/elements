using Core.Config;
using Core.Element;
using Core.Element.Config;
using Core.Grid.Config;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;

namespace Core
{
    public sealed class GameCreator : IGameCreator
    {
        private readonly GameConfig _gameConfig;
        private readonly ElementConfig _elementConfig;

        public GameCreator(IConfigProvider configProvider)
        {
            _gameConfig = configProvider.GetConfig<GameConfig>();
            _elementConfig = configProvider.GetConfig<ElementConfig>();
        }

        public async UniTask CreateGame(int level)
        {
            StartPointsProvider startPointsProvider = Object.FindObjectOfType<StartPointsProvider>();
            
            LevelGridConfig levelGridConfig = _gameConfig.LevelGridConfigs[level];
            
            Vector3 downCenterPosition = startPointsProvider.StartDownPoint.position;
            int elementsCount = levelGridConfig.LevelGrid[0].Length;
            float elementSize = _gameConfig.DefaultGridWidth / elementsCount;
            float updatedScale = elementSize / _elementConfig.ElementSize.x; 
            
            Vector3 offset = new Vector3((elementSize * elementsCount / 2) - elementSize / 2, 0.0f);
            Vector3 startSpawnPosition = downCenterPosition - offset;

            for (int i = 0; i < levelGridConfig.LevelGrid.Length; i++)
            {
                for (int k = 0; k < levelGridConfig.LevelGrid[i].Length; k++)
                {
                    ElementType elementType = levelGridConfig.LevelGrid[i][k];
                    
                    if (elementType == ElementType.None) continue;

                    Vector3 spawnPosition = startSpawnPosition + new Vector3(elementSize * k, elementSize * i);
                    GridGameElement element = Object.Instantiate(
                        _elementConfig.ElementPrefabs[elementType],
                        spawnPosition,
                        Quaternion.identity);
                    element.transform.localScale = Vector3.one * updatedScale;
                }
            }
        }
    }
}