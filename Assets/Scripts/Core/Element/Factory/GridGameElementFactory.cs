using System.Collections.Generic;
using Core.Element.Config;
using Services;
using Unity.Mathematics;
using UnityEngine;

namespace Core.Element.Factory
{
    public sealed class GridGameElementFactory : IGridGameElementFactory
    {
        private readonly IRenderOrderHelper _renderOrderHelper;
        private readonly ICollisionDetector _collisionDetector;
        private readonly ElementConfig _elementConfig;

        public GridGameElementFactory(IConfigProvider configs, IRenderOrderHelper renderOrderHelper, ICollisionDetector collisionDetector)
        {
            _renderOrderHelper = renderOrderHelper;
            _collisionDetector = collisionDetector;
            _elementConfig = configs.GetConfig<ElementConfig>();
        }

        public IReadOnlyList<GridGameElement> GetAllActiveElements() =>
            _collisionDetector.GetAllRegisteredElements();

        public GridGameElement Create(ElementType type, int2 gridIndex, Vector3 position, Vector3 scale, Transform parent = null)
        {
            GridGameElement element = Object.Instantiate(_elementConfig.ElementPrefabs[type], position, Quaternion.identity);
            element.transform.localScale = scale;
            element.SetRenderOrder(_renderOrderHelper.GetRenderOrder(gridIndex.x, gridIndex.y));
            element.SetElementType(type);
            element.SetGridIndex(gridIndex);
            element.SetAvailability(ElementAvailabilityType.Available);
            _collisionDetector.Add(element);
            element.OnDestroy += RemoveFromCollisionDetector;
            return element;
        }

        private void RemoveFromCollisionDetector(GridGameElement element)
        {
            element.OnDestroy -= RemoveFromCollisionDetector;
            _collisionDetector.Remove(element.gameObject.GetInstanceID());
        }
    }
}