using System;

namespace KeelPlugins.Utils
{
    internal class Lazy<T>
    {
        private readonly Func<T> initializer;
        private T value;

        public Lazy(Func<T> initializer)
        {
            this.initializer = initializer ?? throw new ArgumentNullException(nameof(initializer));
        }

        public T Value
        {
            get
            {
                if(value == null)
                    value = initializer();
                return value;
            }
        }

        public static implicit operator Lazy<T>(Func<T> func)
        {
            return new Lazy<T>(func);
        }

        public static implicit operator T(Lazy<T> lazy)
        {
            return lazy.Value;
        }
    }
}
