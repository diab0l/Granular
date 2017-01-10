using System;
using System.Collections.Generic;
using System.ComponentModel;
using Granular.Compatibility.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Markup;
using Granular.Extensions;
using Granular.Collections;

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
        private Uri sourceUri;
        private IEnumerable<IElementInitializer> memberInitializers;
        private IElementInitializer contentInitializer;

        private IPropertyAdapter nameProperty;
        private string nameDirectiveValue;

        public ElementInitializer(XamlElement element)
        {
            elementType = element.GetElementType();
            namespaces = element.Namespaces;
            sourceUri = element.SourceUri;

            memberInitializers = CreateMemberInitializers(element, elementType);
            contentInitializer = CreateContentInitializer(element, elementType);

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

            if (element is IUriContext)
            {
                ((IUriContext)element).BaseUri = sourceUri;
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

        private static IElementInitializer CreateContentInitializer(XamlElement element, Type elementType)
        {
            if (!element.Values.Any())
            {
                return null;
            }

            string contentPropertyName = ContentPropertyAttribute.GetPropertyName(elementType);
            if (!contentPropertyName.IsNullOrEmpty())
            {
                return ElementMemberInitializer.Create(elementType, contentPropertyName, element.Values, element.Namespaces, element.SourceUri);
            }

            if (ElementCollectionContentInitailizer.IsCollectionType(elementType))
            {
                return ElementCollectionContentInitailizer.Create(element.Values, elementType);
            }

            throw new Granular.Exception("Cannot add content to element of type \"{0}\" as it's not a collection type and does not declare ContentProperty", elementType.Name);
        }

        private static IEnumerable<IElementInitializer> CreateMemberInitializers(XamlElement element, Type elementType)
        {
            List<IElementInitializer> list = new List<IElementInitializer>();

            int index = 0;
            foreach (XamlMember member in element.Members)
            {
                // markup extensions may contain members with an empty name, the name should be resolved from the member index
                XamlName memberName = member.Name.IsEmpty ? GetParameterName(elementType, index) : member.Name;

                list.Add(ElementMemberInitializer.Create(memberName.ResolveContainingType(elementType), memberName.MemberName, member.Values, member.Namespaces, member.SourceUri));
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
            string propertyName = RuntimeNamePropertyAttribute.GetPropertyName(type);
            return !propertyName.IsNullOrWhiteSpace() ? PropertyAdapter.CreateAdapter(type, propertyName) : null;
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
        public static IElementInitializer Create(Type containingType, string memberName, IEnumerable<object> values, XamlNamespaces namespaces, Uri sourceUri)
        {
            IPropertyAdapter propertyAdapter = PropertyAdapter.CreateAdapter(containingType, memberName);
            if (propertyAdapter != null)
            {
                return ElementPropertyMemberInitializer.Create(propertyAdapter, values, namespaces, sourceUri);
            }

            IEventAdapter eventAdapter = EventAdapter.CreateAdapter(containingType, memberName);
            if (eventAdapter != null)
            {
                return new ElementEventMemberInitializer(eventAdapter, GetEventHandlerName(memberName, values));
            }

            throw new Granular.Exception("Type \"{0}\" does not contain a member named \"{1}\"", containingType.Name, memberName);
        }

        private static string GetEventHandlerName(string memberName, IEnumerable<object> values)
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

                if (contentTarget == null)
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

        public static IElementInitializer Create(IPropertyAdapter propertyAdapter, IEnumerable<object> values, XamlNamespaces namespaces, Uri sourceUri)
        {
            if (!values.Any())
            {
                return ElementInitializer.Empty;
            }

            if (values.Count() == 1)
            {
                object value = values.First();

                if (propertyAdapter.PropertyType == typeof(IFrameworkElementFactory))
                {
                    return new FrameworkElementFactoryInitializer(propertyAdapter, ElementFactory.FromValue(value, null, namespaces, sourceUri));
                }

                Type valueType = value is XamlElement ? ((XamlElement)value).GetElementType() : value.GetType();

                ITypeConverter typeConverter;
                if (propertyAdapter.PropertyType.IsAssignableFrom(valueType) || typeof(IMarkupExtension).IsAssignableFrom(valueType) || TypeConverter.TryGetTypeConverter(valueType, propertyAdapter.PropertyType, out typeConverter))
                {
                    IElementFactory contentFactory = ElementFactory.FromValue(value, propertyAdapter.PropertyType, namespaces, sourceUri);
                    return new ElementPropertyMemberInitializer(propertyAdapter, contentFactory);
                }
            }

            if (ElementCollectionContentInitailizer.IsCollectionType(propertyAdapter.PropertyType))
            {
                IElementInitializer propertyContentInitializer = ElementCollectionContentInitailizer.Create(values, propertyAdapter.PropertyType);

                // wrap with a factory that creates the collection (when it's null) before adding its values
                return new ElementPropertyMemberFactoryInitializer(propertyAdapter, propertyContentInitializer);
            }

            if (values.Count() == 1)
            {
                object value = values.First();
                throw new Granular.Exception("Cannot assign value of type \"{0}\" to member of type \"{1}\"", value is XamlElement ? ((XamlElement)value).GetElementType() : value.GetType(), propertyAdapter.PropertyType.Name);
            }

            throw new Granular.Exception("Member of type \"{0}\" cannot have more than one child", propertyAdapter.PropertyType.Name);
        }
    }

    public static class ElementCollectionContentInitailizer
    {
        private static CacheDictionary<Type, bool> IsCollectionTypeCache = CacheDictionary<Type, bool>.CreateUsingStringKeys(ResolveIsCollectionType, type => type.FullName);

        public static IElementInitializer Create(IEnumerable<object> values, Type containingType)
        {
            Type keyType;
            Type valueType;

            if (TryGetDictionaryGenericArguments(containingType, out keyType, out valueType))
            {
                return new ElementDictionaryContentInitializer(containingType, keyType, valueType, values);
            }

            if (TryGetCollectionGenericArgument(containingType, out valueType))
            {
                return new ElementCollectionContentInitializer(valueType, values);
            }

            throw new Granular.Exception("Can't initialize type \"{0}\" content", containingType.Name);
        }

        public static bool IsCollectionType(Type type)
        {
            return IsCollectionTypeCache.GetValue(type);
        }

        private static bool ResolveIsCollectionType(Type type)
        {
            Type keyType;
            Type valueType;

            return TryGetDictionaryGenericArguments(type, out keyType, out valueType) || TryGetCollectionGenericArgument(type, out valueType);
        }

        private static bool TryGetDictionaryGenericArguments(Type type, out Type keyType, out Type valueType)
        {
            Type interfaceType = type.GetInterfaceType(typeof(IDictionary<,>));

            if (interfaceType != null)
            {
                Type[] arguments = Granular.Compatibility.Type.GetTypeInterfaceGenericArguments(type, interfaceType).ToArray();
                keyType = arguments[0];
                valueType = arguments[1];
                return true;
            }

            valueType = null;
            keyType = null;
            return false;
        }

        private static bool TryGetCollectionGenericArgument(Type type, out Type valueType)
        {
            Type interfaceType = type.GetInterfaceType(typeof(ICollection<>));

            if (interfaceType != null)
            {
                valueType = Granular.Compatibility.Type.GetTypeInterfaceGenericArguments(type, interfaceType).First();
                return true;
            }

            valueType = null;
            return false;
        }
    }

    public class ElementCollectionContentInitializer : IElementInitializer
    {
        private IEnumerable<IElementFactory> elementsFactory;

        public ElementCollectionContentInitializer(Type valueTargetType, IEnumerable<object> values)
        {
            elementsFactory = values.Select(value => ElementFactory.FromValue(value, valueTargetType, XamlNamespaces.Empty, null)).ToArray();
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
        private class DeferredValueFactory : IElementFactory
        {
            public Type ElementType { get { return typeof(ValueProvider); } }

            private XamlElement element;
            private Type targetType;
            private bool isShared;

            private IElementFactory elementFactory;

            public DeferredValueFactory(XamlElement element, Type targetType, bool isShared)
            {
                this.element = element;
                this.targetType = targetType;
                this.isShared = isShared;
            }

            public object CreateElement(InitializeContext context)
            {
                object value = null;

                return new ValueProvider(() =>
                {
                    if (elementFactory == null)
                    {
                        elementFactory = ElementFactory.FromXamlElement(element, targetType);
                    }

                    if (value == null || !isShared)
                    {
                        value = elementFactory.CreateElement(context);
                    }

                    return value;
                });
            }
        }

        private class DeferredKeyFactory : IElementFactory
        {
            public Type ElementType { get { return typeof(object); } }

            private IDeferredValueKeyProvider provider;
            private XamlElement element;

            public DeferredKeyFactory(IDeferredValueKeyProvider provider, XamlElement element)
            {
                this.provider = provider;
                this.element = element;
            }

            public object CreateElement(InitializeContext context)
            {
                return provider.GetValueKey(element);
            }
        }

        private class KeyValueElementFactory
        {
            private IElementFactory valueFactory;
            private IElementFactory keyDirectiveFactory;
            private IElementFactory deferredKeyFactory;
            private IPropertyAdapter keyProperty;

            public KeyValueElementFactory(Type keyType, IElementFactory valueFactory, XamlElement xamlElement, bool isValueDeferred)
            {
                this.valueFactory = valueFactory;

                keyDirectiveFactory = GetKeyDirectiveFactory(xamlElement, keyType);
                deferredKeyFactory = isValueDeferred && keyDirectiveFactory == null ? GetDeferredKeyFactory(xamlElement) : null;
                keyProperty = GetKeyProperty(valueFactory.ElementType);

                if (keyDirectiveFactory == null && deferredKeyFactory == null && keyProperty == null)
                {
                    throw new Granular.Exception("Dictionary item \"{0}\" must have a key", xamlElement.Name);
                }
            }

            public KeyValuePair<object, object> CreateElement(InitializeContext context)
            {
                object element = valueFactory.CreateElement(context);

                object key = null;

                if (keyDirectiveFactory != null)
                {
                    key = keyDirectiveFactory.CreateElement(context);

                    if (keyProperty != null)
                    {
                        keyProperty.SetValue(element, key, context.ValueSource);
                    }
                }
                else if (deferredKeyFactory != null)
                {
                    key = deferredKeyFactory.CreateElement(context);
                }
                else
                {
                    key = keyProperty.GetValue(element);
                }

                return new KeyValuePair<object, object>(key, element);
            }

            private static IElementFactory GetKeyDirectiveFactory(XamlElement element, Type keyType)
            {
                XamlMember keyDirective = element.Directives.FirstOrDefault(directive => directive.Name == XamlLanguage.KeyDirective);
                return keyDirective != null ? ElementFactory.FromValue(keyDirective.GetSingleValue(), keyType, element.Namespaces, element.SourceUri) : null;
            }

            private static IPropertyAdapter GetKeyProperty(Type type)
            {
                string propertyName = DictionaryKeyPropertyAttribute.GetPropertyName(type);
                return !propertyName.IsNullOrWhiteSpace() ? PropertyAdapter.CreateAdapter(type, propertyName) : null;
            }

            private static IElementFactory GetDeferredKeyFactory(XamlElement xamlElement)
            {
                Type elementType = xamlElement.GetElementType();

                IDeferredValueKeyProvider provider = DeferredValueKeyProviders.GetDeferredValueKeyProvider(elementType);
                if (provider != null)
                {
                    return new DeferredKeyFactory(provider, xamlElement);
                }

                string keyPropertyName = DictionaryKeyPropertyAttribute.GetPropertyName(elementType);
                if (!keyPropertyName.IsNullOrWhiteSpace())
                {
                    XamlMember keyMember = xamlElement.Members.FirstOrDefault(member => member.Name.LocalName == keyPropertyName);
                    if (keyMember != null)
                    {
                        IPropertyAdapter keyProperty = PropertyAdapter.CreateAdapter(elementType, keyPropertyName);
                        return ElementFactory.FromValue(keyMember.Values.Single(), keyProperty.PropertyType, xamlElement.Namespaces, xamlElement.SourceUri);
                    }
                }

                return null;
            }
        }

        private IEnumerable<KeyValueElementFactory> keyElementFactories;

        public ElementDictionaryContentInitializer(Type dictionaryType, Type keyType, Type valueType, IEnumerable<object> values)
        {
            keyElementFactories = CreateElementsFactories(dictionaryType, keyType, valueType, values);
        }

        public void InitializeElement(object element, InitializeContext context)
        {
            foreach (KeyValueElementFactory keyElementFactory in keyElementFactories)
            {
                KeyValuePair<object, object> pair = keyElementFactory.CreateElement(context);

                Granular.Compatibility.Dictionary.DynamicAdd(element, pair.Key, pair.Value);
            }
        }

        private static IEnumerable<KeyValueElementFactory> CreateElementsFactories(Type dictionaryType, Type keyType, Type valueType, IEnumerable<object> values)
        {
            if (values.Any(value => !(value is XamlElement)))
            {
                throw new Granular.Exception("Can't add a value of type \"{0}\" to a dictionary, as it cannot have a key", values.First(value => !(value is XamlElement)).GetType().Name);
            }

            IEnumerable<XamlElement> valuesElements =  System.Linq.Enumerable.Cast<XamlElement>(values);

            bool isValueProviderSupported = dictionaryType.GetCustomAttributes(true).OfType<SupportsValueProviderAttribute>().Any();

            List<KeyValueElementFactory> list = new List<KeyValueElementFactory>();

            foreach (XamlElement contentChild in valuesElements)
            {
                bool isShared = contentChild.Directives.All(directive => directive.Name != XamlLanguage.SharedDirective || (bool)TypeConverter.ConvertValue(directive.GetSingleValue(), typeof(bool), XamlNamespaces.Empty, null));

                if (!isShared && !isValueProviderSupported)
                {
                    throw new Granular.Exception($"Can't add a non shared value to \"{dictionaryType.FullName}\" as it does not declare a \"SupportsValueProvider\" attribute");
                }

                IElementFactory contentChildFactory = isValueProviderSupported ? new DeferredValueFactory(contentChild, valueType, isShared) : ElementFactory.FromXamlElement(contentChild, valueType);

                list.Add(new KeyValueElementFactory(keyType, contentChildFactory, contentChild, isValueProviderSupported));
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
