using System;

namespace Extensions.Property
{
    public class NotifyProperty<TProperty> : INotifyProperty<TProperty>
    {
        public event Action<TProperty> OnChanged;
        
        private TProperty _value;

        public TProperty Value
        {
            get => _value;
            set
            {
                if(_value.Equals(value)) return;

                _value = value;
                OnChanged?.Invoke(_value);
            }
        }

        public NotifyProperty(TProperty value = default)
        {
            _value = value;
        }
    }
}