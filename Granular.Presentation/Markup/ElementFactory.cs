using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xaml;
using Granular.Extensions;

namespace System.Windows.Markup
{
    public interface IElementFactory
    {
        Type ElementType { get; }
        object CreateElement(InitializeContext context);
    }

    public class ElementFactory : IElementFactory
    {
        public Type ElementType { get; private set; }
        private IElementInitializer elementInitializer;

        public ElementFactory(Type elementType, IElementInitializer elementInitializer)
        {
            this.ElementType = elementType;
            this.elementInitializer = elementInitializer;
        }

        public object CreateElement(InitializeContext context)
        {
            object target = Activator.CreateInstance(ElementType);
            elementInitializer.InitializeElement(target, context);
            return target;
        }

        public static IElementFactory FromXamlAttribute(XamlAttribute attribute, Type targetType)
        {
            if (attribute.Value is XamlElement)
            {
                return FromXamlElement((XamlElement)attribute.Value, targetType);
            }

            return FromElementFactory(new ConstantElementFactory(attribute.Value), targetType, attribute.Namespaces);
        }

        public static IElementFactory FromXamlElement(XamlElement element, Type targetType)
        {
            Type elementType = element.GetElementType();

            if (elementType.GetDefaultConstructor() == null)
            {
                return FromElementFactory(FromXamlElementContent(element), targetType, element.Namespaces);
            }

            IElementInitializer elementInitializer = new ElementInitializer(element);
            IElementFactory elementFactory = new ElementFactory(elementType, elementInitializer);

            return FromElementFactory(elementFactory, targetType, element.Namespaces);
        }

        private static IElementFactory FromXamlElementContent(XamlElement element)
        {
            if (element.GetMemberNodes().Any())
            {
                throw new Granular.Exception("Element \"{0}\" can't have members, as its type doesn't have a default constructor and it can only be converted from its content", element.Name);
            }

            IEnumerable<XamlElement> contentChilren = element.GetContentChildren();

            if (contentChilren.Any() && !element.TextValue.IsNullOrEmpty())
            {
                throw new Granular.Exception("Element \"{0}\" cannot have both children and text value", element.Name);
            }

            if (!contentChilren.Any())
            {
                return new ConvertedElementFactory(new ConstantElementFactory(element.TextValue), element.GetElementType(), element.Namespaces);
            }

            if (contentChilren.Count() == 1)
            {
                return ElementFactory.FromXamlElement(contentChilren.First(), element.GetElementType());
            }

            throw new Granular.Exception("Element \"{0}\" can't have multiple children, as its type doesn't have a default constructor and it can only be converted from its content", element.Name);
        }

        private static IElementFactory FromElementFactory(IElementFactory elementFactory, Type targetType, XamlNamespaces namespaces)
        {
            if (elementFactory.ElementType.GetInterfaces().Contains(typeof(IMarkupExtension)))
            {
                return new MarkupExtensionElementFactory(elementFactory);
            }

            if (targetType != null && !targetType.IsAssignableFrom(elementFactory.ElementType))
            {
                return new ConvertedElementFactory(elementFactory, targetType, namespaces);
            }

            return elementFactory;
        }
    }

    public class ConvertedElementFactory : IElementFactory
    {
        public Type ElementType { get; private set; }

        private IElementFactory valueFactory;
        private XamlNamespaces namespaces;

        public ConvertedElementFactory(IElementFactory elementFactory, Type elementTargetType, XamlNamespaces namespaces)
        {
            this.valueFactory = elementFactory;
            this.ElementType = elementTargetType;
            this.namespaces = namespaces;
        }

        public object CreateElement(InitializeContext context)
        {
            return TypeConverter.ConvertValue(valueFactory.CreateElement(context), ElementType, namespaces);
        }
    }

    public class ConstantElementFactory : IElementFactory
    {
        public Type ElementType { get; private set; }

        private object value;

        public ConstantElementFactory(object value)
        {
            this.value = value;
            this.ElementType = value.GetType();
        }

        public object CreateElement(InitializeContext context)
        {
            return value;
        }
    }

    public class MarkupExtensionElementFactory : IElementFactory
    {
        public Type ElementType { get { return typeof(object); } }

        private IElementFactory valueFactory;

        public MarkupExtensionElementFactory(IElementFactory valueFactory)
        {
            this.valueFactory = valueFactory;
        }

        public object CreateElement(InitializeContext context)
        {
            return ((IMarkupExtension)valueFactory.CreateElement(context)).ProvideValue(context);
        }
    }
}
