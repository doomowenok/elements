using Core.Balloon.Config;
using Services;
using Services.Pool;
using UnityEngine;

namespace Core.Balloon.Factory
{
    public sealed class BalloonFactory : IBalloonFactory
    {
        private readonly BalloonConfig _balloonConfig;
        private readonly ObjectPool<GameBalloon> _pool;

        public BalloonFactory(IConfigProvider configs)
        {
            _balloonConfig = configs.GetConfig<BalloonConfig>();
            _pool = new GameBalloonObjectPool();
        }
        
        public GameBalloon Create(BalloonType type, Vector3 spawnPosition)
        {
            GameBalloon balloon = null;
            
            balloon = _pool.TryGet(out IPoolObject<GameBalloon> poolObject) 
                ? poolObject.PoolObject 
                : Object.Instantiate(_balloonConfig.Prefab, spawnPosition, Quaternion.identity);

            balloon.gameObject.SetActive(true);
            balloon.transform.position = spawnPosition;
            balloon.transform.rotation = Quaternion.identity;

            balloon.OnEndMove += ReturnBalloonToPool;
            
            return balloon;
        }

        private void ReturnBalloonToPool(GameBalloon balloon)
        {
            balloon.OnEndMove -= ReturnBalloonToPool;
            _pool.Return(balloon);
        }
    }
}