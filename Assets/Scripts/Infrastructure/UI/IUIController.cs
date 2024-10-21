using Infrastructure.UI.Factory;
using Infrastructure.UI.MVVM;

namespace Infrastructure.UI
{
    public interface IUIController
    {
        void SubscribeViewModel<TViewModel>() where TViewModel : IViewModel;
        void UnsubscribeViewModel<TViewModel>() where TViewModel : IViewModel;
        void CreateView<TView>(UIViewType viewType) where TView : class, IView;
        void DestroyView<TView>(UIViewType viewType) where TView : IView;
    }
}