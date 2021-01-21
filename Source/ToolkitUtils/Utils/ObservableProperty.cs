namespace SirRandoo.ToolkitUtils.Utils
{
    public class ObservableProperty<T>
    {
        public delegate void ChangeEvent(T data);

        private T value;

        public ObservableProperty(T initialValue)
        {
            value = initialValue;
        }

        public event ChangeEvent Changed;

        public void Set(T val)
        {
            if (val.Equals(value))
            {
                return;
            }

            value = val;

            Changed?.Invoke(value);
        }

        public T Get()
        {
            return value;
        }

        public static implicit operator T(ObservableProperty<T> prop)
        {
            return prop.value;
        }
    }
}
