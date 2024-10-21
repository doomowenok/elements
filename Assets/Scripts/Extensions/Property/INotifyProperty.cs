using System;

namespace Extensions.Property
{
    public interface INotifyProperty<TProperty>
    {
        TProperty Value { get; set; }
        event Action<TProperty> OnChanged;
    }
}