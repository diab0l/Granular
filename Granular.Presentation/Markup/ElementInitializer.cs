using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            if (element is ISupportInitialize)
            {
                ((ISupportInitialize)element).BeginInit();
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

            if (element is ISupportInitialize)
            {
                ((ISupportInitialize)element).EndInit();
            }
        }

        private static IElementInitializer CreateContentInitializer(XamlElement element)
        {
            Type elementType = element.GetElementType();

            string contentPropertyName = PropertyAttribute.GetPropertyName<ContentPropertyAttribute>(elementType);
            if (!contentPropertyName.IsNullOrEmpty())
            {
                return ElementMemberInitializer.Create(new XamlName(contentPropertyName), elementType, element.Values, element.Namespaces);
            }

            if (ElementCollectionContentInitailizer.IsCollectionType(elementType))
            {
                return ElementCollectionContentInitailizer.Create(element.Values, elementType);
            }

            return null;
        }

        private static IEnumerable<IElementInitializer> CreateMemberInitializers(XamlElement element)
        {
            Type elementType = element.GetElementType();

            List<IElementInitializer> list = new List<IElementInitializer>();

            int index = 0;
            foreach (XamlMember member in element.Members)
            {
                // markup extensions may contain members with an empty name, the name should be resolved from the member index
                XamlName memberName = member.Name.IsEmpty ? GetParameterName(elementType, index) : member.Name;

                list.Add(ElementMemberInitializer.Create(memberName, elementType, member.Values, member.Namespaces));
                index++;
            }

            return list;
        }

        private static XamlName GetParameterName(Type type, int index)
        {
            MarkupExtensionParameterAttribute parameterAttribute = type.GetCustomAttributes(true).OfType<MarkupExtensionParameterAttribute>().FirstOrDefault(attribute => attribute.Index == index);

            if (parameterAttribute == null)
            {
                throw new Granular.Exception("Type \"{0}\" does not declare MarkupExtensionParameter for index {1}", type.Name, index);
            }

            return new XamlName(parameterAttribute.Name);
        }

        private static string GetNameDirectiveValue(XamlElement element)
        {
            XamlMember nameDirective = element.Directives.FirstOrDefault(directive => directive.Name == XamlLanguage.NameDirective);
            return nameDirective != null ? (string)nameDirective.GetSingleValue() : null;
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
        public static IElementInitializer Create(XamlName memberName, Type containingType, IEnumerable<object> values, XamlNamespaces namespaces)
        {
            IEventAdapter eventAdapter = EventAdapter.CreateAdapter(containingType, memberName);
            if (eventAdapter != null)
            {
                return new ElementEventMemberInitializer(eventAdapter, GetEventHandlerName(memberName, values));
            }

            IPropertyAdapter propertyAdapter = PropertyAdapter.CreateAdapter(containingType, memberName);
            if (propertyAdapter != null)
            {
                return ElementPropertyMemberInitializer.Create(propertyAdapter, values, namespaces);
            }

            throw new Granular.Exception("Type \"{0}\" does not contain a member named \"{1}\"", containingType.Name, memberName);
        }

        private static string GetEventHandlerName(XamlName memberName, IEnumerable<object> values)
        {
            if (!values.Any())
            {
                throw new Granular.Exception("Member \"{0}\" doesn't have values", memberName);
            }

            if (values.Count() > 1)
            {
                throw new Granular.Exception("Member \"{0}\" cannot have multiple values", memberName);
            }

            if (!(values.First() is String))
            {
                throw new Granular.Exception("Member \"{0}\" value is not an event handler name", memberName);
            }

            return (string)values.First();
        }
    }

    public class ElementEventMemberInitializer : IElementInitializer
    {
        private IEventAdapter eventAdapter;
        private string eventHandlerName;

        public ElementEventMemberInitializer(IEventAdapter eventAdapter, string eventHandlerName)
        {
            this.eventAdapter = eventAdapter;
            this.eventHandlerName = eventHandlerName;
        }

        public void InitializeElement(object element, InitializeContext context)
        {
            eventAdapter.AddHandler(element, CreateEventHandler(eventAdapter.HandlerType, context.Root, eventHandlerName));
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

        private ElementPropertyMemberInitializer(IPropertyAdapter propertyAdapter, IElementFactory propertyValueFactory)
        {
            this.propertyAdapter = propertyAdapter;
            this.propertyValueFactory = propertyValueFactory;
        }

        public void InitializeElement(object element, InitializeContext context)
        {
            propertyAdapter.SetValue(element, propertyValueFactory.CreateElement(context), context.ValueSource);
        }

        public static IElementInitializer Create(IPropertyAdapter propertyAdapter, IEnumerable<object> values, XamlNamespaces namespaces)
        {
            if (!values.Any())
            {
                return ElementInitializer.Empty;
            }

            if (ElementCollectionContentInitailizer.IsCollectionType(propertyAdapter.PropertyType) &&
                !(values.Count() == 1 && values.First() is XamlElement && propertyAdapter.PropertyType.IsAssignableFrom(((XamlElement)values.First()).GetElementType())))
            {
                IElementInitializer propertyContentInitializer = ElementCollectionContentInitailizer.Create(values, propertyAdapter.PropertyType);

                // wrap with a factory that creates the collection (when it's null) before adding its values
                return new ElementPropertyMemberFactoryInitializer(propertyAdapter, propertyContentInitializer);
            }

            if (values.Count() == 1)
            {
                if (propertyAdapter.PropertyType == typeof(IFrameworkElementFactory))
                {
                    return new FrameworkElementFactoryInitializer(propertyAdapter, ElementFactory.FromValue(values.First(), null, namespaces));
                }

                IElementFactory contentFactory = ElementFactory.FromValue(values.First(), propertyAdapter.PropertyType, namespaces);
                return new ElementPropertyMemberInitializer(propertyAdapter, contentFactory);
            }

            throw new Granular.Exception("Member of type \"{0}\" cannot have more than one child", propertyAdapter.PropertyType.Name);
        }
    }

    public static class ElementCollectionContentInitailizer
    {
        public static IElementInitializer Create(IEnumerable<object> values, Type containingType)
        {
            if (IsList(containingType))
            {
                return new ElementListContentInitializer(values);
            }

            if (IsDictionary(containingType))
            {
                return new ElementDictionaryContentInitializer(values);
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

        public ElementListContentInitializer(IEnumerable<object> values)
        {
            elementsFactory = values.Select(value => ElementFactory.FromValue(value, null, XamlNamespaces.Empty)).ToArray();
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
                XamlMember keyDirective = element.Directives.FirstOrDefault(directive => directive.Name == XamlLanguage.KeyDirective);
                return keyDirective != null ? keyDirective.GetSingleValue() : null;
            }

            private static IPropertyAdapter GetKeyProperty(Type type)
            {
                return PropertyAdapter.CreateAdapter(type, new XamlName(PropertyAttribute.GetPropertyName<DictionaryKeyPropertyAttribute>(type)));
            }
        }

        private IEnumerable<KeyElementFactory> keyElementFactories;

        public ElementDictionaryContentInitializer(IEnumerable<object> values)
        {
            keyElementFactories = CreateElementsFactories(values);
        }

        public void InitializeElement(object element, InitializeContext context)
        {
            foreach (KeyElementFactory keyElementFactory in keyElementFactories)
            {
                KeyValuePair<object, object> pair = keyElementFactory.CreateElement(context);

                Granular.Compatibility.Dictionary.DynamicAdd(element, pair.Key, pair.Value);
            }
        }

        private static IEnumerable<KeyElementFactory> CreateElementsFactories(IEnumerable<object> values)
        {
            if (values.Any(value => !(value is XamlElement)))
            {
                throw new Granular.Exception("Can't add a value of type \"{0}\" to a dictionary, as it cannot have a key", values.First(value => !(value is XamlElement)).GetType().Name);
            }

            IEnumerable<XamlElement> valuesElements = values.Cast<XamlElement>();

            List<KeyElementFactory> list = new List<KeyElementFactory>();

            foreach (XamlElement contentChild in valuesElements)
            {
                bool isShared = contentChild.Directives.All(directive => directive.Name != XamlLanguage.SharedDirective || (bool)TypeConverter.ConvertValue(directive.GetSingleValue(), typeof(bool), XamlNamespaces.Empty));

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
