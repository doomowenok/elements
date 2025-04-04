using System;
using System.Collections.Generic;
using Core.Balloon.Config;
using Core.Balloon.Factory;
using Cysharp.Threading.Tasks;
using Extensions;
using Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Balloon
{
    public sealed class GameBalloonController : IGameBalloonController
    {
        private readonly IBalloonFactory _balloonFactory;
        private readonly BalloonConfig _balloonConfig;
        private readonly BalloonType[] _balloonTypes;
        private readonly List<GameBalloon> _balloons = new List<GameBalloon>();

        private Camera _camera;

        public GameBalloonController(IConfigProvider configs, IBalloonFactory balloonFactory)
        {
            _balloonFactory = balloonFactory;
            _balloonConfig = configs.GetConfig<BalloonConfig>();
            _balloonTypes = Enum.GetValues(typeof(BalloonType)) as BalloonType[];
        }
        
        public async UniTask StartCreatingBalloons()
        {
            _camera ??= Camera.main;

            if (_camera == null) return;
            
            int count = Random.Range(1, 4);

            Vector2 delayData = _balloonConfig.DelayBeforeNextCreation;
            
            for (int i = 0; i < count; i++)
            {
                await UniTask.WaitForSeconds(Random.Range(delayData.x, delayData.y));
                
                BalloonType type = _balloonTypes[Random.Range(0, _balloonTypes.Length)];

                BalloonData data = _balloonConfig.Balloons[type];

                Vector3 spawnPosition = CalculateSpawnPosition();
                Vector2 endPosition = CalculateEndPosition();
                float speed = _balloonConfig.BalloonMinMaxSpeed.GetRandomInRange();
                float frequency = _balloonConfig.BalloonMinMaxFrequency.GetRandomInRange();
                float amplitude = _balloonConfig.BalloonMinMaxAmplitude.GetRandomInRange();

                GameBalloon balloon = _balloonFactory.Create(type, spawnPosition);
                balloon.Initialize(data.Sprite, spawnPosition, endPosition, speed, frequency, amplitude);
                balloon.StartMove();
                balloon.OnEndMove += RemoveFromActive;

                _balloons.Add(balloon);
            }
        }

        private Vector3 CalculateSpawnPosition()
        {
            Vector2 centerLeftPosition = _camera.ScreenToWorldPoint(new Vector3(0, Screen.height / 2.0f));
            Vector2 upperLeftPosition = _camera.ScreenToWorldPoint(new Vector3(0, Screen.height - Screen.height * 0.2f));

            float randomYPosition = Random.Range(centerLeftPosition.y - _balloonConfig.BalloonSize.y * 2, 
                upperLeftPosition.y - _balloonConfig.BalloonSize.y);
            float xPosition = centerLeftPosition.x - _balloonConfig.BalloonSize.x;
            Vector3 spawnPosition = new Vector3(xPosition, randomYPosition);
            return spawnPosition;
        }

        private Vector3 CalculateEndPosition()
        {
            return _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2.0f)) + new Vector3(_balloonConfig.BalloonSize.x, 0.0f);
        }

        private void RemoveFromActive(GameBalloon balloon)
        {
            balloon.OnEndMove -= RemoveFromActive;
            _balloons.Remove(balloon);
            
            if (_balloons.Count == 0)
            {
                StartCreatingBalloons().Forget();
            }
        }
    }
}