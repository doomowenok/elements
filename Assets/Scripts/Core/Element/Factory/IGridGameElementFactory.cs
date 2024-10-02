using Core.Element.Config;
using Cysharp.Threading.Tasks;
using Services;
using Unity.Mathematics;
using UnityEngine;

namespace Core.Element.Factory
{
    public interface IGridGameElementFactory
    {
        UniTask<GridGameElement> Create(ElementType type, int2 gridIndex, Vector3 position, Vector3 scale, Transform parent = null);
    }

    public sealed class GridGameElementFactory : IGridGameElementFactory
    {
        private readonly IRenderOrderHelper _renderOrderHelper;
        private readonly ElementConfig _elementConfig;

        public GridGameElementFactory(IConfigProvider configs, IRenderOrderHelper renderOrderHelper)
        {
            _renderOrderHelper = renderOrderHelper;
            _elementConfig = configs.GetConfig<ElementConfig>();
        }
        
        public async UniTask<GridGameElement> Create(ElementType type, int2 gridIndex, Vector3 position, Vector3 scale, Transform parent = null)
        {
            GridGameElement element = Object.Instantiate(_elementConfig.ElementPrefabs[type], position, Quaternion.identity);
            element.transform.localScale = scale;
            element.SetRenderOrder(_renderOrderHelper.GetRenderOrder(gridIndex.x, gridIndex.y));
            element.SetElementType(type);
            return element;
        }
    }
}