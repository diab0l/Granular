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

        public static IElementFactory FromValue(object value, Type targetType, XamlNamespaces namespaces)
        {
            if (value is XamlElement)
            {
                return FromXamlElement((XamlElement)value, targetType);
            }

            return FromElementFactory(new ConstantElementFactory(value), targetType, namespaces);
        }

        public static IElementFactory FromXamlElement(XamlElement element, Type targetType)
        {
            Type elementType = element.GetElementType();

            if (element.Values.Any() && PropertyAttribute.GetPropertyName<ContentPropertyAttribute>(elementType).IsNullOrEmpty() && !ElementCollectionContentInitailizer.IsCollectionType(elementType))
            {
                return FromElementFactory(FromXamlElementContent(element), targetType, element.Namespaces);
            }

            IElementInitializer elementInitializer = new ElementInitializer(element);
            IElementFactory elementFactory = new ElementFactory(elementType, elementInitializer);

            return FromElementFactory(elementFactory, targetType, element.Namespaces);
        }

        private static IElementFactory FromXamlElementContent(XamlElement element)
        {
            if (element.Members.Any())
            {
                throw new Granular.Exception("Element \"{0}\" can't have members, as it's not a collection type and does not declare ContentProperty and can only be converted from its content", element.Name);
            }

            if (element.Values.Count() > 1)
            {
                throw new Granular.Exception("Element \"{0}\" can't have multiple children, as it's not a collection type and does not declare ContentProperty and can only be converted from its content", element.Name);
            }

            return FromValue(element.Values.First(), element.GetElementType(), element.Namespaces);
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
        private ITypeConverter typeConverter;

        public ConvertedElementFactory(IElementFactory elementFactory, Type elementTargetType, XamlNamespaces namespaces)
        {
            this.valueFactory = elementFactory;
            this.ElementType = elementTargetType;
            this.namespaces = namespaces;
            this.typeConverter = TypeConverter.GetTypeConverter(elementFactory.ElementType, elementTargetType);
        }

        public object CreateElement(InitializeContext context)
        {
            return typeConverter.ConvertFrom(namespaces, valueFactory.CreateElement(context));
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
