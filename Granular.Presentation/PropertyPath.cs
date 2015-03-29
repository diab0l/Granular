using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;
using Granular.Extensions;

namespace System.Windows
{
    [TypeConverter(typeof(PropertyPathElementTypeConverter))]
    public interface IPropertyPathElement
    {
        [System.Runtime.CompilerServices.Reflectable(false)]
        bool TryGetValue(object target, out object value);

        [System.Runtime.CompilerServices.Reflectable(false)]
        bool TryGetDependencyProperty(Type containingType, out DependencyProperty dependencyProperty);

        IPropertyObserver CreatePropertyObserver(Type baseValueType);
    }

    public static class PropertyPathElementExtensions
    {
        public static DependencyProperty GetDependencyProperty(this IPropertyPathElement propertyPathElement, Type containingType)
        {
            DependencyProperty dependencyProperty;
            if (propertyPathElement.TryGetDependencyProperty(containingType, out dependencyProperty))
            {
                return dependencyProperty;
            }

            throw new Granular.Exception("Type \"{0}\" does not contain a dependency property \"{1}\"", containingType.Name, propertyPathElement);
        }
    }

    public class PropertyPathElement : IPropertyPathElement
    {
        public XamlName PropertyName { get; private set; }

        public PropertyPathElement(XamlName propertyName)
        {
            this.PropertyName = propertyName;
        }

        public override bool Equals(object obj)
        {
            PropertyPathElement other = obj as PropertyPathElement;

            return other != null && this.GetType() == other.GetType() &&
                this.PropertyName == other.PropertyName;
        }

        public override int GetHashCode()
        {
            return PropertyName.GetHashCode();
        }

        public override string ToString()
        {
            return PropertyName.IsMemberName ? String.Format("({0})", PropertyName.LocalName) : PropertyName.LocalName;
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        public bool TryGetValue(object target, out object value)
        {
            return TryGetValue(target, PropertyName, out value);
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        public bool TryGetDependencyProperty(Type containingType, out DependencyProperty dependencyProperty)
        {
            dependencyProperty = DependencyProperty.GetProperty(containingType, PropertyName);
            return dependencyProperty != null;
        }

        public IPropertyObserver CreatePropertyObserver(Type baseValueType)
        {
            DependencyProperty dependencyProperty = DependencyProperty.GetProperty(baseValueType, PropertyName);
            if (dependencyProperty != null)
            {
                return new DependencyPropertyObserver(dependencyProperty);
            }

            Type propertyContainingType = PropertyName.IsMemberName ? TypeParser.ParseType(PropertyName.ContainingTypeName) : baseValueType;

            PropertyInfo propertyInfo = propertyContainingType.GetInstanceProperty(PropertyName.MemberName);
            if (propertyInfo != null)
            {
                return new ClrPropertyObserver(propertyInfo, new object[0]);
            }

            return null;
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        public static bool TryGetValue(object target, XamlName propertyName, out object value)
        {
            DependencyProperty dependencyProperty = DependencyProperty.GetProperty(target.GetType(), propertyName);
            if (dependencyProperty != null && target is DependencyObject)
            {
                value = ((DependencyObject)target).GetValue(dependencyProperty);
                return true;
            }

            Type propertyContainingType = propertyName.IsMemberName ? TypeParser.ParseType(propertyName.ContainingTypeName) : target.GetType();

            PropertyInfo propertyInfo = propertyContainingType.GetInstanceProperty(propertyName.MemberName);
            if (propertyInfo != null && !propertyInfo.GetIndexParameters().Any())
            {
                value = propertyInfo.GetValue(target, new object[0]);
                return true;
            }

            value = null;
            return false;
        }
    }

    public class IndexPropertyPathElement : IPropertyPathElement
    {
        public XamlName PropertyName { get; private set; }
        public IEnumerable<string> IndexRawValues { get; private set; }

        private XamlNamespaces namespaces;

        public IndexPropertyPathElement(XamlName propertyName, IEnumerable<string> indexRawValues, XamlNamespaces namespaces)
        {
            this.PropertyName = propertyName;
            this.IndexRawValues = indexRawValues;
            this.namespaces = namespaces;
        }

        public override bool Equals(object obj)
        {
            IndexPropertyPathElement other = obj as IndexPropertyPathElement;

            return other != null && this.GetType() == other.GetType() &&
                this.PropertyName == other.PropertyName &&
                this.IndexRawValues.SequenceEqual(other.IndexRawValues);
        }

        public override int GetHashCode()
        {
            return PropertyName.GetHashCode();
        }

        public override string ToString()
        {
            string propertyName = PropertyName.IsMemberName ? String.Format("({0})", PropertyName.LocalName) : PropertyName.LocalName;
            string indexRawValues = IndexRawValues.DefaultIfEmpty(String.Empty).Aggregate((s1, s2) => String.Format("{0}, {1}", s1, s2));

            return String.Format("{0}[{1}]", propertyName, indexRawValues);
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        public bool TryGetValue(object target, out object value)
        {
            Type propertyContainingType = PropertyName.IsMemberName ? TypeParser.ParseType(PropertyName.ContainingTypeName) : target.GetType();
            string propertyName = PropertyName.MemberName;

            bool isDefaultIndexProperty = propertyName.IsNullOrEmpty();

            object indexPropertyValue;

            // an index property that has a name (such as "Values[0]"), might be a regular property with the same name ("Values"), and a default index property ("[0]" or "Item[0]")
            if (!isDefaultIndexProperty && PropertyPathElement.TryGetValue(target, PropertyName, out indexPropertyValue))
            {
                if (indexPropertyValue == null)
                {
                    value = null;
                    return false;
                }

                target = indexPropertyValue;
                propertyContainingType = indexPropertyValue.GetType();
                isDefaultIndexProperty = true;
            }

            PropertyInfo indexPropertyInfo = isDefaultIndexProperty ? propertyContainingType.GetDefaultIndexProperty() : propertyContainingType.GetInstanceProperty(propertyName);

            if (indexPropertyInfo == null)
            {
                value = null;
                return false;
            }

            value = indexPropertyInfo.GetValue(target, ParseIndexValues(indexPropertyInfo).ToArray());
            return true;
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        public bool TryGetDependencyProperty(Type containingType, out DependencyProperty dependencyProperty)
        {
            dependencyProperty = null;
            return false;
        }

        public IPropertyObserver CreatePropertyObserver(Type baseValueType)
        {
            return new IndexPropertyObserver(baseValueType, this, namespaces);
        }

        public IEnumerable<object> ParseIndexValues(PropertyInfo indexPropertyInfo)
        {
            if (indexPropertyInfo.GetIndexParameters().Count() != IndexRawValues.Count())
            {
                throw new Granular.Exception("Invalid number of index parameters for \"{0}.{1}\"", indexPropertyInfo.DeclaringType.Name, indexPropertyInfo.Name);
            }

            return indexPropertyInfo.GetIndexParameters().Zip(IndexRawValues, (parameter, rawValue) => TypeConverter.ConvertValue(rawValue, parameter.ParameterType, namespaces)).ToArray();
        }
    }

    public class DependencyPropertyPathElement : IPropertyPathElement
    {
        public DependencyProperty DependencyProperty { get; private set; }

        public DependencyPropertyPathElement(DependencyProperty dependencyProperty)
        {
            this.DependencyProperty = dependencyProperty;
        }

        public override bool Equals(object obj)
        {
            DependencyPropertyPathElement other = obj as DependencyPropertyPathElement;

            return other != null && this.GetType() == other.GetType() &&
                this.DependencyProperty == other.DependencyProperty;
        }

        public override int GetHashCode()
        {
            return DependencyProperty.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("({0})", DependencyProperty);
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        public bool TryGetDependencyProperty(Type containingType, out DependencyProperty dependencyProperty)
        {
            dependencyProperty = this.DependencyProperty;
            return true;
        }

        public IPropertyObserver CreatePropertyObserver(Type baseValueType)
        {
            return new DependencyPropertyObserver(DependencyProperty);
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        public bool TryGetValue(object target, out object value)
        {
            if (target is DependencyObject)
            {
                value = ((DependencyObject)target).GetValue(DependencyProperty);
                return true;
            }

            value = null;
            return false;
        }
    }

    [TypeConverter(typeof(PropertyPathTypeConverter))]
    public class PropertyPath
    {
        public static readonly PropertyPath Empty = new PropertyPath(new IPropertyPathElement[0]);

        public IEnumerable<IPropertyPathElement> Elements { get; private set; }

        public bool IsEmpty { get { return !Elements.Any(); } }

        public PropertyPath(IEnumerable<IPropertyPathElement> elements)
        {
            this.Elements = elements;
        }

        public override string ToString()
        {
            return Elements.Select(element => element.ToString()).DefaultIfEmpty(String.Empty).Aggregate((s1, s2) => String.Format("{0}.{1}", s1, s2));
        }

        public static PropertyPath Parse(string value, XamlNamespaces namespaces = null)
        {
            PropertyPathParser parser = new PropertyPathParser(value, namespaces ?? XamlNamespaces.Empty);
            return new PropertyPath(parser.Parse());
        }

        public static PropertyPath FromDependencyProperty(DependencyProperty dependencyProperty)
        {
            return new PropertyPath(new[] { new DependencyPropertyPathElement(dependencyProperty) });
        }
    }

    public class PropertyPathTypeConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, object value)
        {
            return PropertyPath.Parse((string)value, namespaces);
        }
    }

    public class PropertyPathElementTypeConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, object value)
        {
            return new PropertyPathElement(XamlName.FromPrefixedName((string)value, namespaces));
        }
    }

    public static class PropertyPathExtensions
    {
        public static PropertyPath GetBasePropertyPath(this PropertyPath propertyPath)
        {
            return propertyPath.Elements.Count() > 1 ? new PropertyPath(propertyPath.Elements.Take(propertyPath.Elements.Count() - 1)) : PropertyPath.Empty;
        }

        public static PropertyPath Insert(this PropertyPath propertyPath, int index, IPropertyPathElement element)
        {
            IEnumerable<IPropertyPathElement> elements = propertyPath.Elements.Take(index).Concat(new [] { element }).Concat(propertyPath.Elements.Skip(index)).ToArray();
            return new PropertyPath(elements);
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        public static bool TryGetValue(this PropertyPath propertyPath, object root, out object value)
        {
            if (propertyPath.IsEmpty)
            {
                value = null;
                return false;
            }

            if (propertyPath.Elements.Count() > 1)
            {
                object baseValue;

                if (!propertyPath.GetBasePropertyPath().TryGetValue(root, out baseValue))
                {
                    value = null;
                    return false;
                }

                root = baseValue;
            }

            return propertyPath.Elements.Last().TryGetValue(root, out value);
        }
    }
}
