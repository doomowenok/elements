namespace Infrastructure.UI
{
    public interface IUIController
    {
        TWindow GetWindow<TWindow>() where TWindow : BaseWindow;
        void DestroyWindow<TWindow>() where TWindow : BaseWindow;
        bool IsOpen<TWindow>() where TWindow : BaseWindow;
        void SetInteractionState<TWindow>(bool canInteract) where TWindow : BaseWindow;
    }
}