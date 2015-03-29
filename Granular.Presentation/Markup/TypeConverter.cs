using System;
using System.Collections.Generic;
using System.Linq;
using System.Xaml;

namespace System.Windows.Markup
{
    public interface ITypeConverter
    {
        object ConvertFrom(XamlNamespaces namespaces, object value);
    }

    public class TypeConverterAttribute : Attribute
    {
        public Type ConverterType { get; private set; }

        public TypeConverterAttribute(Type converterType)
        {
            this.ConverterType = converterType;
        }
    }

    public static class TypeConverter
    {
        [System.Runtime.CompilerServices.Reflectable(false)]
        public static bool TryConvertValue(object value, Type type, XamlNamespaces namespaces, out object result)
        {
            if (type.IsInstanceOfType(value))
            {
                result = value;
                return true;
            }

            ITypeConverter typeConverter = KnownTypes.GetTypeConverter(type);

            if (typeConverter != null)
            {
                result = typeConverter.ConvertFrom(namespaces, value);
                return true;
            }

            result = null;
            return false;
        }

        public static object ConvertValue(object value, Type type, XamlNamespaces namespaces)
        {
            object result;

            if (!TryConvertValue(value, type, namespaces, out result))
            {
                throw new Granular.Exception("Can't convert \"{0}\" to {1}", value, type.Name);
            }

            return result;
        }
    }
}
