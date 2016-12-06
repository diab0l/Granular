using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Markup;
using Granular.Collections;

namespace System.Windows.Markup
{
    internal class ObjectTypeConverter : ITypeConverter
    {
        public static readonly ObjectTypeConverter Default = new ObjectTypeConverter();

        private ObjectTypeConverter()
        {
            //
        }

        public object ConvertFrom(XamlNamespaces namespaces, Uri sourceUri, object value)
        {
            return value;
        }
    }

    internal class StringTypeConverter : ITypeConverter
    {
        public static readonly StringTypeConverter Default = new StringTypeConverter();

        private StringTypeConverter()
        {
            //
        }

        public object ConvertFrom(XamlNamespaces namespaces, Uri sourceUri, object value)
        {
            return value.ToString();
        }
    }

    internal class BooleanTypeConverter : ITypeConverter
    {
        public static readonly BooleanTypeConverter Default = new BooleanTypeConverter();

        private BooleanTypeConverter()
        {
            //
        }

        public object ConvertFrom(XamlNamespaces namespaces, Uri sourceUri, object value)
        {
            string text = value.ToString().Trim().ToLower();

            if (text != "true" && text != "false")
            {
                throw new Granular.Exception("Can't convert \"{0}\" to boolean", text);
            }

            return text == "true";
        }
    }

    internal class Int32TypeConverter : ITypeConverter
    {
        public static readonly Int32TypeConverter Default = new Int32TypeConverter();

        private Int32TypeConverter()
        {
            //
        }

        public object ConvertFrom(XamlNamespaces namespaces, Uri sourceUri, object value)
        {
            return Int32.Parse(value.ToString());
        }
    }

    internal class DoubleTypeConverter : ITypeConverter
    {
        public static readonly DoubleTypeConverter Default = new DoubleTypeConverter();

        private DoubleTypeConverter()
        {
            //
        }

        public object ConvertFrom(XamlNamespaces namespaces, Uri sourceUri, object value)
        {
            string text = value.ToString().Trim().ToLower();

            if (text == "auto")
            {
                return Double.NaN;
            }

            return Double.Parse(text);
        }
    }

    internal class TimeSpanTypeConverter : ITypeConverter
    {
        public static readonly TimeSpanTypeConverter Default = new TimeSpanTypeConverter();

        private TimeSpanTypeConverter()
        {
            //
        }

        public object ConvertFrom(XamlNamespaces namespaces, Uri sourceUri, object value)
        {
            TimeSpan timeSpan;
            if (Granular.Compatibility.TimeSpan.TryParse(value.ToString(), out timeSpan))
            {
                return timeSpan;
            }

            throw new Granular.Exception("Can't parse TimeSpan value \"{0}\"", value);
        }
    }

    internal class EnumTypeConverter : ITypeConverter
    {
        private Type type;

        public EnumTypeConverter(Type type)
        {
            this.type = type;
        }

        public object ConvertFrom(XamlNamespaces namespaces, Uri sourceUri, object value)
        {
            return Enum.Parse(type, value.ToString().Trim());
        }
    }

    internal class TypeTypeConverter : ITypeConverter
    {
        public static readonly TypeTypeConverter Default = new TypeTypeConverter();

        private TypeTypeConverter()
        {
            //
        }

        public object ConvertFrom(XamlNamespaces namespaces, Uri sourceUri, object value)
        {
            return TypeParser.ParseType(value.ToString().Trim(), namespaces);
        }
    }

    internal class UriTypeConverter : ITypeConverter
    {
        public static readonly UriTypeConverter Default = new UriTypeConverter();

        private UriTypeConverter()
        {
            //
        }

        public object ConvertFrom(XamlNamespaces namespaces, Uri sourceUri, object value)
        {
            return Granular.Compatibility.Uri.CreateRelativeOrAbsoluteUri(value.ToString().Trim());
        }
    }

    public static class KnownTypes
    {
        private static CacheDictionary<Type, ITypeConverter> typeConverterCache = new CacheDictionary<Type, ITypeConverter>(ResolveTypeConverter);

        public static ITypeConverter GetTypeConverter(Type type)
        {
            return typeConverterCache.GetValue(type);
        }

        private static ITypeConverter ResolveTypeConverter(Type type)
        {
            if (type == typeof(object))
            {
                return ObjectTypeConverter.Default;
            }

            if (type == typeof(string))
            {
                return StringTypeConverter.Default;
            }

            if (type == typeof(bool))
            {
                return BooleanTypeConverter.Default;
            }

            if (type == typeof(int))
            {
                return Int32TypeConverter.Default;
            }

            if (type == typeof(double))
            {
                return DoubleTypeConverter.Default;
            }

            if (type == typeof(TimeSpan))
            {
                return TimeSpanTypeConverter.Default;
            }

            if (type.IsEnum)
            {
                return new EnumTypeConverter(type);
            }

            if (type == typeof(Type))
            {
                return TypeTypeConverter.Default;
            }

            if (type == typeof(Uri))
            {
                return UriTypeConverter.Default;
            }

            if (type.GetIsGenericType() && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return GetTypeConverter(type.GetGenericArguments().First());
            }

            TypeConverterAttribute typeConverterAttribute = type.GetCustomAttributes(typeof(TypeConverterAttribute), false).FirstOrDefault() as TypeConverterAttribute;
            if (typeConverterAttribute != null)
            {
                return Activator.CreateInstance(typeConverterAttribute.ConverterType) as ITypeConverter;
            }

            return null;
        }
    }
}
