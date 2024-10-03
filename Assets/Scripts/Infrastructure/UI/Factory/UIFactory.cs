using UnityEngine;
using VContainer;

namespace Infrastructure.UI.Factory
{
    public sealed class UIFactory : IUIFactory
    {
        private static readonly string WindowsPath = "UI/Level/Prefabs";
        
        private readonly IObjectResolver _resolver;

        public UIFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }
        
        public TWindow CreateWindow<TWindow>() where TWindow : BaseWindow
        {
            TWindow windowPrefab = Resources.Load<TWindow>(WindowsPath + "/" + typeof(TWindow).Name);
            TWindow window = Object.Instantiate(windowPrefab);
            window.gameObject.SetActive(false);
            _resolver.Inject(window);
            return window;
        }
    }
}