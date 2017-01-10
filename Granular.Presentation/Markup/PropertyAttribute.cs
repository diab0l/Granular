using System;
using System.Collections.Generic;
using Granular.Collections;
using Granular.Compatibility.Linq;

namespace System.Windows.Markup
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public abstract class PropertyAttribute : Attribute
    {
        public string Name { get; private set; }

        public PropertyAttribute(string name)
        {
            this.Name = name;
        }

        protected static string ResolvePropertyName<T>(Type type) where T : PropertyAttribute
        {
            return type.GetCustomAttributes(true).OfType<T>().Select(attribute => attribute.Name).FirstOrDefault();
        }
    }

    public class ContentPropertyAttribute : PropertyAttribute
    {
        private static CacheDictionary<Type, string> propertyNameCache = CacheDictionary<Type, string>.CreateUsingStringKeys(type => PropertyAttribute.ResolvePropertyName<ContentPropertyAttribute>(type), type => type.FullName);

        public ContentPropertyAttribute(string name) :
            base(name)
        {
            //
        }

        public static string GetPropertyName(Type type)
        {
            return propertyNameCache.GetValue(type);
        }
    }

    public class RuntimeNamePropertyAttribute : PropertyAttribute
    {
        private static CacheDictionary<Type, string> propertyNameCache = CacheDictionary<Type, string>.CreateUsingStringKeys(type => PropertyAttribute.ResolvePropertyName<RuntimeNamePropertyAttribute>(type), type => type.FullName);

        public RuntimeNamePropertyAttribute(string name) :
            base(name)
        {
            //
        }

        public static string GetPropertyName(Type type)
        {
            return propertyNameCache.GetValue(type);
        }
    }

    public class DictionaryKeyPropertyAttribute : PropertyAttribute
    {
        private static CacheDictionary<Type, string> propertyNameCache = CacheDictionary<Type, string>.CreateUsingStringKeys(type => PropertyAttribute.ResolvePropertyName<DictionaryKeyPropertyAttribute>(type), type => type.FullName);

        public DictionaryKeyPropertyAttribute(string name) :
            base(name)
        {
            //
        }

        public static string GetPropertyName(Type type)
        {
            return propertyNameCache.GetValue(type);
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class MarkupExtensionParameterAttribute : PropertyAttribute
    {
        public int Index { get; private set; }

        public MarkupExtensionParameterAttribute(string name, int index = 0) :
            base(name)
        {
            Index = index;
        }
    }
}
