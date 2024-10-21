using Infrastructure.UI.MVVM;
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
        
        public TView CreateWindow<TView>(int viewType) where TView : IView
        {
            GameObject viewPrefab = Resources.Load<GameObject>(WindowsPath + "/" + UIViewTypes.GetName(viewType));
            GameObject windowObject = Object.Instantiate(viewPrefab);
            TView view = windowObject.GetComponent<TView>();
            _resolver.Inject(view);
            view.Subscribe();
            return view;
        }
    }
}