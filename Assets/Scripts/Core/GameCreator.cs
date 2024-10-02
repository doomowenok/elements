using Core.Config;
using Core.Element;
using Core.Element.Factory;
using Core.Grid.Config;
using Core.Session;
using Cysharp.Threading.Tasks;
using Services;
using Unity.Mathematics;
using UnityEngine;

namespace Core
{
    public sealed class GameCreator : IGameCreator
    {
        private readonly IGridGameElementFactory _elementFactory;
        private readonly ITransformCalculator _transformCalculator;
        private readonly SessionData _sessionData;
        private readonly GameConfig _gameConfig;

        public GameCreator(
            IConfigProvider configProvider,
            IGridGameElementFactory elementFactory, 
            ITransformCalculator transformCalculator,
            SessionData sessionData)
        {
            _elementFactory = elementFactory;
            _transformCalculator = transformCalculator;
            _sessionData = sessionData;
            _gameConfig = configProvider.GetConfig<GameConfig>();
        }

        public async UniTask CreateGame(int level)
        {
            LevelGridConfig levelGridConfig = _gameConfig.LevelGridConfigs[level];
            var gridGameElements = new GridGameElement[levelGridConfig.LevelGrid.Length][];
            var gridPositions = new Vector3[levelGridConfig.LevelGrid.Length][];
            
            int elementsCount = levelGridConfig.LevelGrid[0].Length;
            
            for (int i = 0; i < levelGridConfig.LevelGrid.Length; i++)
            {
                gridGameElements[i] = new GridGameElement[elementsCount];
                gridPositions[i] = new Vector3[elementsCount];
                
                for (int k = 0; k < levelGridConfig.LevelGrid[i].Length; k++)
                {
                    Vector3 spawnPosition = _transformCalculator.GetSpawnPosition(elementsCount, new int2(i, k));
                    ElementType elementType = levelGridConfig.LevelGrid[i][k];
                    
                    gridPositions[i][k] = spawnPosition;
                    
                    if (elementType == ElementType.None) continue;

                    GridGameElement element = await _elementFactory.Create(elementType, new int2(i, k), spawnPosition, _transformCalculator.GetScale(elementsCount));
                    gridGameElements[i][k] = element;
                }
            }

            _sessionData.Elements = gridGameElements;
            _sessionData.Positions = gridPositions;
        }
    }
}