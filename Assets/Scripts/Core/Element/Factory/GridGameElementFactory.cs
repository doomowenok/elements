using System.Collections.Generic;
using System.Linq;
using Core.Element.Config;
using Core.Grid;
using Core.Utils;
using Cysharp.Threading.Tasks;
using Services;
using Services.Pool;
using Unity.Mathematics;
using UnityEngine;

namespace Core.Element.Factory
{
    public sealed class GridGameElementFactory : IGridGameElementFactory
    {
        private readonly IRenderOrderHelper _renderOrderHelper;
        private readonly ICollisionDetector _collisionDetector;
        private readonly ElementConfig _elementConfig;
        private readonly ObjectPool<GridGameElement> _pool;
        
        private readonly List<UniTask> _destroyTasks = new List<UniTask>();

        public GridGameElementFactory(IConfigProvider configs, IRenderOrderHelper renderOrderHelper, ICollisionDetector collisionDetector)
        {
            _renderOrderHelper = renderOrderHelper;
            _collisionDetector = collisionDetector;
            _elementConfig = configs.GetConfig<ElementConfig>();
            _pool = new GridGameElementObjectPool();
        }

        public IReadOnlyList<GridGameElement> GetAllActiveElements() =>
            _collisionDetector.GetAllRegisteredElements().ToList();

        public GridGameElement Create(ElementType type, int2 gridIndex, Vector3 position, Vector3 scale, Transform parent = null)
        {
            GridGameElement element = null;
            ElementData data = _elementConfig.ElementPrefabs[type];
            
            element = _pool.TryGet(out IPoolObject<GridGameElement> poolObject) 
                ? poolObject.PoolObject 
                : Object.Instantiate(_elementConfig.Prefab, position, Quaternion.identity);

            element.transform.position = position;
            element.transform.rotation = Quaternion.identity;
            element.transform.localScale = scale;
            
            element.Initialize(data.AnimatorOverrideController, data.Sprite);
            element.SetRenderOrder(_renderOrderHelper.GetRenderOrder(gridIndex.x, gridIndex.y));
            element.SetElementType(type);
            element.SetGridIndex(gridIndex);
            element.SetAvailability(ElementAvailabilityType.Available);
            element.gameObject.SetActive(true);
            _collisionDetector.Add(element);
            element.OnDestroy += RemoveFromCollisionDetector;
            return element;
        }

        public async UniTask DestroyAllElements()
        {
            _collisionDetector.GetAllRegisteredElements().ForEach(element =>
            {
                _destroyTasks.Add(element.Destroy());
            });

            await UniTask.WhenAll(_destroyTasks);
            
            _destroyTasks.Clear();
        }

        private void RemoveFromCollisionDetector(GridGameElement element)
        {
            element.OnDestroy -= RemoveFromCollisionDetector;
            _collisionDetector.Remove(element.gameObject.GetInstanceID());
            _pool.Return(element);
        }
    }
}