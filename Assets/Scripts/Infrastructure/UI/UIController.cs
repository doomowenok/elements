using System.Collections.Generic;
using Infrastructure.UI.Factory;
using UnityEngine;

namespace Infrastructure.UI
{
    public sealed class UIController : IUIController
    {
        private readonly IUIFactory _uiFactory;
        
        private readonly Dictionary<string, BaseWindow> _windows = new Dictionary<string, BaseWindow>();

        public UIController(IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
        }
        
        public TWindow GetWindow<TWindow>() where TWindow : BaseWindow
        {
            if (!_windows.TryGetValue(typeof(TWindow).Name, out BaseWindow window))
            {
                TWindow baseWindow = _uiFactory.CreateWindow<TWindow>();
                _windows.Add(typeof(TWindow).Name, baseWindow);
                return baseWindow;
            }

            return window as TWindow;
        }

        public void DestroyWindow<TWindow>() where TWindow : BaseWindow
        {
            if (_windows.TryGetValue(typeof(TWindow).Name, out BaseWindow window))
            {
                Object.Destroy(window.gameObject);
                _windows.Remove(typeof(TWindow).Name);
            }
        }

        public bool IsOpen<TWindow>() where TWindow : BaseWindow => 
            _windows.ContainsKey(typeof(TWindow).Name);

        public void SetInteractionState<TWindow>(bool canInteract) where TWindow : BaseWindow
        {
            if (_windows.TryGetValue(typeof(TWindow).Name, out BaseWindow window))
            {
                window.ChangeInteractionState(canInteract);
            }
        }
    }
}