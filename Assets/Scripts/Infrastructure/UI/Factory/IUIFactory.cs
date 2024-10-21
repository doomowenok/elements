using Infrastructure.UI.MVVM;

namespace Infrastructure.UI.Factory
{
    public interface IUIFactory
    {
        TView CreateWindow<TView>(UIViewType viewType) where TView : IView;
    }
}