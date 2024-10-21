using Core.Config;
using Core.Element;
using Core.Element.Factory;
using Core.Grid.Config;
using Core.Session;
using Core.Utils;
using JetBrains.Annotations;
using Services;
using Unity.Mathematics;
using UnityEngine;

namespace Core.Boot
{
    public sealed class GameCreator : IGameCreator
    {
        private readonly IGridGameElementFactory _elementFactory;
        private readonly ITransformCalculator _transformCalculator;
        private readonly SessionController _sessionController;
        private readonly GameConfig _gameConfig;

        public GameCreator(
            IConfigProvider configProvider,
            IGridGameElementFactory elementFactory, 
            ITransformCalculator transformCalculator,
            SessionController sessionController)
        {
            _elementFactory = elementFactory;
            _transformCalculator = transformCalculator;
            _sessionController = sessionController;
            _gameConfig = configProvider.GetConfig<GameConfig>();
        }

        public void CreateGame(int level, [CanBeNull] SessionSaveData saveData)
        {
            bool anySave = saveData != null;

            LevelGridConfig levelGridConfig = _gameConfig.LevelGridConfigs[level % _gameConfig.LevelGridConfigs.Count];
            
            GridGameElement[][] gridGameElements = new GridGameElement[levelGridConfig.LevelGrid.Length][];
            Vector3[][] gridPositions = new Vector3[levelGridConfig.LevelGrid.Length][];
            int elementsCount = levelGridConfig.LevelGrid[0].Length;

            ElementType[][] levelGrid = levelGridConfig.LevelGrid;

            if(anySave)
            {
                levelGrid = new ElementType[levelGridConfig.LevelGrid.Length][];

                for (int i = 0; i < levelGrid.Length; i++)
                {
                    levelGrid[i] = new ElementType[elementsCount];
                }

                for (int i = 0; i < saveData.Elements.Count; i++)
                {
                    int row = i / elementsCount;
                    int column = i % elementsCount;
                    levelGrid[row][column] = (ElementType)saveData.Elements[i];
                }
            }

            for (int i = 0; i < levelGrid.Length; i++)
            {
                gridGameElements[i] = new GridGameElement[elementsCount];
                gridPositions[i] = new Vector3[elementsCount];
                
                for (int k = 0; k < levelGrid[i].Length; k++)
                {
                    Vector3 spawnPosition = _transformCalculator.GetSpawnPosition(elementsCount, new int2(i, k));
                    ElementType elementType = levelGrid[i][k];
                    
                    gridPositions[i][k] = spawnPosition;
                    
                    if (elementType == ElementType.None) continue;

                    GridGameElement element = _elementFactory.Create(elementType, new int2(i, k), spawnPosition, _transformCalculator.GetScale(elementsCount));
                    gridGameElements[i][k] = element;
                }
            }

            _sessionController.Level.Value = level;
            _sessionController.Elements = gridGameElements;
            _sessionController.Positions = gridPositions;
        }
    }
}