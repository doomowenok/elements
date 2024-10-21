using Infrastructure.UI.Factory;
using Infrastructure.UI.MVVM;

namespace Infrastructure.UI
{
    public interface IUIController
    {
        void SubscribeViewModel<TViewModel>() where TViewModel : IViewModel;
        void UnsubscribeViewModel<TViewModel>() where TViewModel : IViewModel;
        void CreateView<TView>(int viewType) where TView : class, IView;
        void DestroyView<TView>(int viewType) where TView : IView;
    }
}