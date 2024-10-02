namespace Services.Save
{
    public interface ISaveService
    {
        void Save<TData>(TData data) where TData : class;
        TData Load<TData>() where TData : class;
        bool ContainsSave<TData>() where TData : class;
    }
}