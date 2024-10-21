using UnityEngine;
using VContainer;

namespace Infrastructure.UI.MVVM
{
    public abstract class BaseView<TViewModel> : MonoBehaviour, IView where TViewModel : IViewModel
    {
        protected TViewModel ViewModel { get; private set; }

        public GameObject ViewObject => gameObject;

        [Inject]
        private void Construct(TViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public abstract void Subscribe();
        public abstract void Unsubscribe();
    }
}