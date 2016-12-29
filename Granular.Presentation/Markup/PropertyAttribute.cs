using System;
using System.Collections.Generic;
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

        public static string GetPropertyName<T>(Type type) where T : PropertyAttribute
        {
            return type.GetCustomAttributes(true).OfType<T>().Select(attribute => attribute.Name).DefaultIfEmpty(String.Empty).First();
        }
    }

    public class ContentPropertyAttribute : PropertyAttribute
    {
        public ContentPropertyAttribute(string name) :
            base(name)
        {
            //
        }
    }

    public class RuntimeNamePropertyAttribute : PropertyAttribute
    {
        public RuntimeNamePropertyAttribute(string name) :
            base(name)
        {
            //
        }
    }

    public class DictionaryKeyPropertyAttribute : PropertyAttribute
    {
        public DictionaryKeyPropertyAttribute(string name) :
            base(name)
        {
            //
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
