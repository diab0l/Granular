using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xaml;
using Granular.Extensions;

namespace System.Windows.Markup
{
    public interface IElementInitializer
    {
        void InitializeElement(object element, InitializeContext context);
    }

    public class ElementInitializer : IElementInitializer
    {
        private class EmptyElementInitializer : IElementInitializer
        {
            public void InitializeElement(object element, InitializeContext context)
            {
                //
            }
        }

        public static readonly IElementInitializer Empty = new EmptyElementInitializer();

        private Type elementType;
        private XamlNamespaces namespaces;
        private IEnumerable<IElementInitializer> memberInitializers;
        private IElementInitializer contentInitializer;

        private IPropertyAdapter nameProperty;
        private string nameDirectiveValue;

        public ElementInitializer(XamlElement element)
        {
            elementType = element.GetElementType();
            namespaces = element.Namespaces;

            memberInitializers = CreateMemberInitializers(element);
            contentInitializer = CreateContentInitializer(element);

            nameDirectiveValue = GetNameDirectiveValue(element);
            nameProperty = GetNameProperty(element.GetElementType());
        }

        public void InitializeElement(object element, InitializeContext context)
        {
            context = context.CreateChildContext(element);

            if (!elementType.IsAssignableFrom(element.GetType()))
            {
                throw new Granular.Exception("Can't initialize element of type \"{0}\" as it's not a subclass of \"{1}\"", element.GetType().Name, elementType.Name);
            }

            if (element == context.Root && element is DependencyObject)
            {
                NameScope.SetNameScope((DependencyObject)element, context.NameScope);
            }

            if (element is FrameworkElement)
            {
                ((FrameworkElement)element).TemplatedParent = context.TemplatedParent;
            }

            foreach (IElementInitializer memberInitializer in memberInitializers)
            {
                memberInitializer.InitializeElement(element, context);
            }

            string name = nameDirectiveValue.DefaultIfNullOrEmpty(nameProperty != null ? (string)nameProperty.GetValue(element) : String.Empty);

            if (!nameDirectiveValue.IsNullOrEmpty() && nameProperty != null)
            {
                // name property exists, but the name directive was used, so update the property
                nameProperty.SetValue(element, name, context.ValueSource);
            }

            if (!name.IsNullOrEmpty())
            {
                context.NameScope.RegisterName(name, element);
            }

            if (contentInitializer != null)
            {
                contentInitializer.InitializeElement(element, context);
            }

            if (element == context.Root)
            {
                foreach (KeyValuePair<string, object> pair in context.NameScope)
                {
                    SetFieldValue(element, pair.Key, pair.Value);
                }
            }
        }

        private static IElementInitializer CreateContentInitializer(XamlElement element)
        {
            Type elementType = element.GetElementType();

            string contentPropertyName = PropertyAttribute.GetPropertyName<ContentPropertyAttribute>(elementType);
            if (!contentPropertyName.IsNullOrEmpty())
            {
                return ElementMemberInitializer.FromXamlElement(new XamlName(contentPropertyName), element, elementType);
            }

            if (ElementCollectionContentInitailizer.IsCollectionType(element.GetElementType()))
            {
                return ElementCollectionContentInitailizer.FromXamlElement(element, element.GetElementType());
            }

            return null;
        }

        private static IEnumerable<IElementInitializer> CreateMemberInitializers(XamlElement element)
        {
            Type elementType = element.GetElementType();

            List<IElementInitializer> list = new List<IElementInitializer>();

            int index = 0;
            foreach (XamlAttribute attribute in element.GetMemberAttributes())
            {
                // markup extensions can contain empty attributes, the member name is determined by its index
                XamlName memberName = attribute.Name.IsEmpty ? GetParameterName(elementType, index) : attribute.Name;
                list.Add(ElementMemberInitializer.FromXamlAttribute(memberName, attribute, elementType));

                index++;
            }

            foreach (XamlElement child in element.GetMemberChildren())
            {
                list.Add(ElementMemberInitializer.FromXamlElement(child.Name, child, elementType));
            }

            return list;
        }

        private static XamlName GetParameterName(Type type, int index)
        {
            MarkupExtensionParameterAttribute parameterAttribute = type.GetCustomAttributes(true).OfType<MarkupExtensionParameterAttribute>().FirstOrDefault(attribute => attribute.Index == index);
            return parameterAttribute != null ? new XamlName(parameterAttribute.Name) : XamlName.Empty;
        }

        private static string GetNameDirectiveValue(XamlElement element)
        {
            object value;
            return element.TryGetMemberValue(XamlLanguage.NameDirective, out value) ? (string)value : null;
        }

        private static IPropertyAdapter GetNameProperty(Type type)
        {
            return PropertyAdapter.CreateAdapter(type, new XamlName(PropertyAttribute.GetPropertyName<RuntimeNamePropertyAttribute>(type)));
        }

        private static void SetFieldValue(object target, string fieldName, object fieldValue)
        {
            FieldInfo fieldInfo = target.GetType().GetField(fieldName, Granular.Compatibility.BindingFlags.InstanceNonPublic);

            if (fieldInfo == null)
            {
                return;
            }

            if (fieldInfo.FieldType != fieldValue.GetType())
            {
                throw new Granular.Exception("Cannot assign \"{0}\" of type \"{1}\" to field \"{2}.{3}\" of type \"{4}\"", fieldName, fieldValue.GetType().Name, target.GetType().Name, fieldName, fieldInfo.FieldType);
            }

            fieldInfo.SetValue(target, fieldValue);
        }
    }

    public static class ElementMemberInitializer
    {
        public static IElementInitializer FromXamlAttribute(XamlName memberName, XamlAttribute memberAttribute, Type containingType)
        {
            IEventAdapter eventAdapter = EventAdapter.CreateAdapter(containingType, memberName);
            if (eventAdapter != null)
            {
                return ElementEventMemberInitializer.FromXamlAttribute(eventAdapter, memberAttribute);
            }

            IPropertyAdapter propertyAdapter = PropertyAdapter.CreateAdapter(containingType, memberName);
            if (propertyAdapter != null)
            {
                return ElementPropertyMemberInitializer.FromXamlAttribute(propertyAdapter, memberAttribute);
            }

            throw new Granular.Exception("Type \"{0}\" does not contain a member named \"{1}\"", containingType.Name, memberName);
        }

        public static IElementInitializer FromXamlElement(XamlName memberName, XamlElement memberElement, Type containingType)
        {
            IEventAdapter eventAdapter = EventAdapter.CreateAdapter(containingType, memberName);
            if (eventAdapter != null)
            {
                return ElementEventMemberInitializer.FromXamlElement(eventAdapter, memberElement);
            }

            IPropertyAdapter propertyAdapter = PropertyAdapter.CreateAdapter(containingType, memberName);
            if (propertyAdapter != null)
            {
                return ElementPropertyMemberInitializer.FromXamlElement(propertyAdapter, memberElement);
            }

            throw new Granular.Exception("Type \"{0}\" does not contain a member named \"{1}\"", containingType.Name, memberName);
        }
    }

    public class ElementEventMemberInitializer : IElementInitializer
    {
        private IEventAdapter eventAdapter;
        private string eventHandlerName;

        private ElementEventMemberInitializer(IEventAdapter eventAdapter, string eventHandlerName)
        {
            this.eventAdapter = eventAdapter;
            this.eventHandlerName = eventHandlerName;
        }

        public void InitializeElement(object element, InitializeContext context)
        {
            eventAdapter.AddHandler(element, CreateEventHandler(eventAdapter.HandlerType, context.Root, eventHandlerName));
        }

        public static IElementInitializer FromXamlElement(IEventAdapter eventAdapter, XamlElement eventElement)
        {
            return new ElementEventMemberInitializer(eventAdapter, GetEventHandlerName(eventElement));
        }

        public static IElementInitializer FromXamlAttribute(IEventAdapter eventAdapter, XamlAttribute eventAttribute)
        {
            return new ElementEventMemberInitializer(eventAdapter, GetEventHandlerName(eventAttribute));
        }

        private static string GetEventHandlerName(XamlElement element)
        {
            if (element.Children.Any())
            {
                throw new Granular.Exception("Element \"{0}\" can't have children", element.Name);
            }

            if (element.Attributes.Any())
            {
                throw new Granular.Exception("Element \"{0}\" can't have attributes", element.Name);
            }

            if (element.TextValue.IsNullOrEmpty())
            {
                throw new Granular.Exception("Element \"{0}\" doesn't contain an event handler name", element.Name);
            }

            return element.TextValue;
        }

        private static string GetEventHandlerName(XamlAttribute attribute)
        {
            if (!(attribute.Value is String))
            {
                throw new Granular.Exception("Attribute \"{0}\" value is not an event handler name", attribute.Name);
            }

            return (string)attribute.Value;
        }

        private static Delegate CreateEventHandler(Type eventHandlerType, object source, string eventHandlerName)
        {
            MethodInfo methodInfo = source.GetType().GetMethod(eventHandlerName, Granular.Compatibility.BindingFlags.InstancePublicNonPublicFlattenHierarchy);

            if (methodInfo == null)
            {
                throw new Granular.Exception("Type \"{0}\" does not contain an event handler named \"{1}\"", source.GetType().Name, eventHandlerName);
            }

            return Delegate.CreateDelegate(eventHandlerType, source, methodInfo);
        }
    }

    public class ElementPropertyMemberInitializer : IElementInitializer
    {
        private class ElementPropertyMemberFactoryInitializer : IElementInitializer
        {
            private IPropertyAdapter propertyAdapter;
            private IElementInitializer propertyValueInitializer;

            public ElementPropertyMemberFactoryInitializer(IPropertyAdapter propertyAdapter, IElementInitializer propertyContentInitializer)
            {
                this.propertyAdapter = propertyAdapter;
                this.propertyValueInitializer = propertyContentInitializer;
            }

            public void InitializeElement(object element, InitializeContext context)
            {
                object contentTarget = propertyAdapter.GetValue(element);

                if (contentTarget == null && propertyAdapter.PropertyType.GetDefaultConstructor() != null)
                {
                    contentTarget = Activator.CreateInstance(propertyAdapter.PropertyType);
                    propertyAdapter.SetValue(element, contentTarget, context.ValueSource);
                }

                propertyValueInitializer.InitializeElement(contentTarget, context);
            }
        }

        private IPropertyAdapter propertyAdapter;
        private IElementFactory propertyValueFactory;

        public ElementPropertyMemberInitializer(IPropertyAdapter propertyAdapter, IElementFactory propertyValueFactory)
        {
            this.propertyAdapter = propertyAdapter;
            this.propertyValueFactory = propertyValueFactory;
        }

        public void InitializeElement(object element, InitializeContext context)
        {
            propertyAdapter.SetValue(element, propertyValueFactory.CreateElement(context), context.ValueSource);
        }

        public static IElementInitializer FromXamlAttribute(IPropertyAdapter propertyAdapter, XamlAttribute memberAttribute)
        {
            IElementFactory contentFactory = ElementFactory.FromXamlAttribute(memberAttribute, propertyAdapter.PropertyType);
            return new ElementPropertyMemberInitializer(propertyAdapter, contentFactory);
        }

        public static IElementInitializer FromXamlElement(IPropertyAdapter propertyAdapter, XamlElement memberElement)
        {
            IEnumerable<XamlElement> children = memberElement.GetContentChildren();

            if (!children.Any())
            {
                if (!memberElement.TextValue.IsNullOrEmpty())
                {
                    object value = TypeConverter.ConvertValue(memberElement.TextValue, propertyAdapter.PropertyType, memberElement.Namespaces);
                    return new ElementPropertyMemberInitializer(propertyAdapter, new ConstantElementFactory(value));
                }

                return ElementInitializer.Empty;
            }

            if (ElementCollectionContentInitailizer.IsCollectionType(propertyAdapter.PropertyType) &&
                !(children.Count() == 1 && propertyAdapter.PropertyType.IsAssignableFrom(children.First().GetElementType())))
            {
                IElementInitializer propertyContentInitializer = ElementCollectionContentInitailizer.FromXamlElement(memberElement, propertyAdapter.PropertyType);
                // use a factory that creates the property value before initializing its content (when its null)
                return new ElementPropertyMemberFactoryInitializer(propertyAdapter, propertyContentInitializer);
            }

            if (children.Count() == 1)
            {
                if (propertyAdapter.PropertyType == typeof(IFrameworkElementFactory))
                {
                    return new FrameworkElementFactoryInitializer(propertyAdapter, ElementFactory.FromXamlElement(children.First(), children.First().GetElementType()));
                }

                IElementFactory contentFactory = ElementFactory.FromXamlElement(children.First(), propertyAdapter.PropertyType);
                return new ElementPropertyMemberInitializer(propertyAdapter, contentFactory);
            }

            throw new Granular.Exception("Element \"{0}\" cannot have more than one child", memberElement.Name);
        }
    }

    public static class ElementCollectionContentInitailizer
    {
        public static IElementInitializer FromXamlElement(XamlElement element, Type containingType)
        {
            if (IsList(containingType))
            {
                return new ElementListContentInitializer(element);
            }

            if (IsDictionary(containingType))
            {
                return new ElementDictionaryContentInitializer(element);
            }

            throw new Granular.Exception("Can't initialize type \"{0}\" content", containingType.Name);
        }

        public static bool IsCollectionType(Type type)
        {
            return IsList(type) || IsDictionary(type);
        }

        private static bool IsDictionary(Type type)
        {
            return type.GetInterfaceType(typeof(IDictionary<,>)) != null;
        }

        private static bool IsList(Type type)
        {
            return type.GetInterfaceType(typeof(IList<>)) != null;
        }
    }

    public class ElementListContentInitializer : IElementInitializer
    {
        private IEnumerable<IElementFactory> elementsFactory;

        public ElementListContentInitializer(XamlElement element)
        {
            elementsFactory = element.GetContentChildren().Select(contentChild => ElementFactory.FromXamlElement(contentChild, null)).ToArray();
        }

        public void InitializeElement(object element, InitializeContext context)
        {
            foreach (IElementFactory elementFactory in elementsFactory)
            {
                Granular.Compatibility.Collection.DynamicAdd(element, elementFactory.CreateElement(context));
            }
        }
    }

    public class ElementDictionaryContentInitializer : IElementInitializer
    {
        private class ValueProviderFactory : IElementFactory
        {
            public Type ElementType { get { return typeof(ValueProvider); } }

            private IElementFactory elementFactory;

            public ValueProviderFactory(IElementFactory elementFactory)
            {
                this.elementFactory = elementFactory;
            }

            public object CreateElement(InitializeContext context)
            {
                return new ValueProvider(() =>
                {
                    InitializeContext localContext = new InitializeContext(null, null, new NameScope(context.NameScope), context.TemplatedParent, context.ValueSource);
                    return elementFactory.CreateElement(localContext);
                });
            }
        }

        private class KeyElementFactory
        {
            private IElementFactory elementFactory;
            private object keyDirectiveValue;
            private IPropertyAdapter keyProperty;

            public KeyElementFactory(IElementFactory elementFactory, XamlElement xamlElement)
            {
                this.elementFactory = elementFactory;

                keyDirectiveValue = GetKeyDirectiveValue(xamlElement);
                keyProperty = GetKeyProperty(elementFactory.ElementType);

                if (keyDirectiveValue == null && keyProperty == null)
                {
                    throw new Granular.Exception("Dictionary item \"{0}\" must have a key", xamlElement.Name);
                }
            }

            public KeyValuePair<object, object> CreateElement(InitializeContext context)
            {
                object element = elementFactory.CreateElement(context);

                object key = keyDirectiveValue ?? (keyProperty != null ? keyProperty.GetValue(element) : null);

                if (keyDirectiveValue != null && keyProperty != null)
                {
                    // key property exists, but the key directive was used, so update the property
                    keyProperty.SetValue(element, key, context.ValueSource);
                }

                return new KeyValuePair<object, object>(key, element);
            }

            private static object GetKeyDirectiveValue(XamlElement element)
            {
                object value;
                return element.TryGetMemberValue(XamlLanguage.KeyDirective, out value) ? value : null;
            }

            private static IPropertyAdapter GetKeyProperty(Type type)
            {
                return PropertyAdapter.CreateAdapter(type, new XamlName(PropertyAttribute.GetPropertyName<DictionaryKeyPropertyAttribute>(type)));
            }
        }

        private IEnumerable<KeyElementFactory> keyElementFactories;

        public ElementDictionaryContentInitializer(XamlElement memberElement)
        {
            keyElementFactories = CreateElementsFactories(memberElement);
        }

        public void InitializeElement(object element, InitializeContext context)
        {
            foreach (KeyElementFactory keyElementFactory in keyElementFactories)
            {
                KeyValuePair<object, object> pair = keyElementFactory.CreateElement(context);

                Granular.Compatibility.Dictionary.DynamicAdd(element, pair.Key, pair.Value);
            }
        }

        private static IEnumerable<KeyElementFactory> CreateElementsFactories(XamlElement memberElement)
        {
            List<KeyElementFactory> list = new List<KeyElementFactory>();

            foreach (XamlElement contentChild in memberElement.GetContentChildren())
            {
                bool isShared = contentChild.Attributes.All(attribute => attribute.Name != XamlLanguage.SharedDirective || (bool)TypeConverter.ConvertValue(attribute.Value, typeof(bool), XamlNamespaces.Empty));

                IElementFactory contentChildFactory = ElementFactory.FromXamlElement(contentChild, null);

                if (!isShared)
                {
                    contentChildFactory = new ValueProviderFactory(contentChildFactory);
                }

                list.Add(new KeyElementFactory(contentChildFactory, contentChild));
            }

            return list;
        }
    }

    public class FrameworkElementFactoryInitializer : IElementInitializer
    {
        private IPropertyAdapter propertyAdapter;
        private IElementFactory elementFactory;

        public FrameworkElementFactoryInitializer(IPropertyAdapter propertyAdapter, IElementFactory elementFactory)
        {
            this.propertyAdapter = propertyAdapter;
            this.elementFactory = elementFactory;
        }

        public void InitializeElement(object element, InitializeContext context)
        {
            propertyAdapter.SetValue(element, new FrameworkElementFactory(elementFactory, context), context.ValueSource);
        }
    }
}
