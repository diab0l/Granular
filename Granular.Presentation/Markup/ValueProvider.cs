using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Markup
{
    public interface IValueProvider
    {
        object ProvideValue();
    }

    public class ValueProvider : IValueProvider
    {
        private Func<object> provideValue;

        public ValueProvider(Func<object> provideValue)
        {
            this.provideValue = provideValue;
        }

        public object ProvideValue()
        {
            return provideValue();
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SupportsValueProviderAttribute : Attribute
    {
    }
}
