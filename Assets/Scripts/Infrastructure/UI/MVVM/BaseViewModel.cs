namespace Infrastructure.UI.MVVM
{
    public abstract class BaseViewModel<TModel> : IViewModel  where TModel : BaseModel
    {
        protected TModel Model { get; private set; }
        
        protected BaseViewModel(TModel model)
        {
            Model = model;
        }

        public abstract void Subscribe();
        public abstract void Unsubscribe();
    }
}