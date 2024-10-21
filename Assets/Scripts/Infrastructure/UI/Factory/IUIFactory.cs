using Infrastructure.UI.MVVM;

namespace Infrastructure.UI.Factory
{
    public interface IUIFactory
    {
        TView CreateWindow<TView>(int viewType) where TView : IView;
    }
}