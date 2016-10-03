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
        private class EmptyTypeConverter : ITypeConverter
        {
            public object ConvertFrom(XamlNamespaces namespaces, object value)
            {
                return value;
            }
        }

        public static ITypeConverter Empty = new EmptyTypeConverter();

        public static bool TryGetTypeConverter(Type sourceType, Type targetType, out ITypeConverter typeConverter)
        {
            if (targetType.IsAssignableFrom(sourceType))
            {
                typeConverter = Empty;
                return true;
            }

            typeConverter = KnownTypes.GetTypeConverter(targetType);
            return typeConverter != null;
        }

        public static ITypeConverter GetTypeConverter(Type sourceType, Type targetType)
        {
            ITypeConverter typeConverter;

            if (!TryGetTypeConverter(sourceType, targetType, out typeConverter))
            {
                throw new Granular.Exception("Can't create type converter from \"{0}\" to \"{1}\"", sourceType.Name, targetType.Name);
            }

            return typeConverter;
        }

        public static bool TryConvertValue(object value, Type type, XamlNamespaces namespaces, out object result)
        {
            ITypeConverter typeConverter;

            if (TryGetTypeConverter(value.GetType(), type, out typeConverter))
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
