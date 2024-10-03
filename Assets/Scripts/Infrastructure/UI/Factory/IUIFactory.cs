namespace Infrastructure.UI.Factory
{
    public interface IUIFactory
    {
        TWindow CreateWindow<TWindow>() where TWindow : BaseWindow;
    }
}