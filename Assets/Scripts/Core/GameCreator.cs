using System.Collections.Generic;
using Core.Config;
using Core.Element;
using Core.Element.Config;
using Core.Element.Factory;
using Core.Grid;
using Core.Grid.Config;
using Cysharp.Threading.Tasks;
using Services;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Core
{
    public sealed class GameCreator : IGameCreator
    {
        private readonly IGridGameElementFactory _elementFactory;
        private readonly ITransformCalculator _transformCalculator;
        private readonly GameConfig _gameConfig;
        
        private GridGameElement[][] _gridGameElements;
        private Vector3[][] _gridPositions;

        public GameCreator(
            IConfigProvider configProvider,
            IGridGameElementFactory elementFactory, 
            ITransformCalculator transformCalculator)
        {
            _elementFactory = elementFactory;
            _transformCalculator = transformCalculator;
            _gameConfig = configProvider.GetConfig<GameConfig>();
        }

        public async UniTask CreateGame(int level)
        {
            LevelGridConfig levelGridConfig = _gameConfig.LevelGridConfigs[level];
            _gridGameElements = new GridGameElement[levelGridConfig.LevelGrid.Length][];
            _gridPositions = new Vector3[levelGridConfig.LevelGrid.Length][];
            
            int elementsCount = levelGridConfig.LevelGrid[0].Length;
            
            for (int i = 0; i < levelGridConfig.LevelGrid.Length; i++)
            {
                _gridGameElements[i] = new GridGameElement[elementsCount];
                _gridPositions[i] = new Vector3[elementsCount];
                
                for (int k = 0; k < levelGridConfig.LevelGrid[i].Length; k++)
                {
                    Vector3 spawnPosition = _transformCalculator.GetSpawnPosition(elementsCount, new int2(i, k));
                    ElementType elementType = levelGridConfig.LevelGrid[i][k];
                    
                    _gridPositions[i][k] = spawnPosition;
                    
                    if (elementType == ElementType.None) continue;

                    GridGameElement element = await _elementFactory.Create(elementType, new int2(i, k), spawnPosition, _transformCalculator.GetScale(elementsCount));
                    _gridGameElements[i][k] = element;
                }
            }
        }
    }
}