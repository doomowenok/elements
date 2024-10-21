using System.Collections.Generic;
using Infrastructure.UI.Factory;
using Infrastructure.UI.MVVM;
using UnityEngine;
using VContainer;

namespace Infrastructure.UI
{
    public sealed class UIController : IUIController
    {
        private readonly IUIFactory _uiFactory;
        private readonly IObjectResolver _resolver;

        private readonly Dictionary<int, IView> _windows = new Dictionary<int, IView>();

        public UIController(IUIFactory uiFactory, IObjectResolver resolver)
        {
            _uiFactory = uiFactory;
            _resolver = resolver;
        }

        public void SubscribeViewModel<TViewModel>() where TViewModel : IViewModel
        {
            _resolver.Resolve<TViewModel>().Subscribe();
        }

        public void UnsubscribeViewModel<TViewModel>() where TViewModel : IViewModel
        {
            _resolver.Resolve<TViewModel>().Unsubscribe();
        }

        public void CreateView<TView>(int viewType) where TView : class, IView
        {
            if (!IsOpen(viewType))
            {
                TView baseWindow = _uiFactory.CreateWindow<TView>(viewType);
                _windows.Add(viewType, baseWindow);
            }
        }

        public void DestroyView<TView>(int viewType) where TView : IView
        {
            if (_windows.TryGetValue(viewType, out IView window))
            {
                Object.Destroy(window.ViewObject);
                _windows.Remove(viewType);
            }
        }

        private bool IsOpen(int viewType) => _windows.ContainsKey(viewType);
    }
}