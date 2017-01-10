using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Granular.Collections;
using Granular.Compatibility;
using Granular.Extensions;

namespace System.Windows
{
    public delegate bool ValidateValueCallback(object value);

    public sealed class DependencyPropertyKey
    {
        public DependencyProperty DependencyProperty { get; private set; }

        public DependencyPropertyKey(DependencyProperty dependencyProperty)
        {
            this.DependencyProperty = dependencyProperty;
        }
    }

    [TypeConverter(typeof(DependencyPropertyTypeConverter))]
    public sealed class DependencyProperty
    {
        private sealed class DependencyPropertyHashKey
        {
            public Type Owner { get; private set; }
            public string Name { get; private set; }
            public string StringKey { get; private set; }

            public DependencyPropertyHashKey(Type owner, string name)
            {
                this.Owner = owner;
                this.Name = name;
                this.StringKey = owner.FullName + "," + name;
            }

            public override bool Equals(object obj)
            {
                DependencyPropertyHashKey other = obj as DependencyPropertyHashKey;

                return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
                    Object.Equals(this.Owner, other.Owner) &&
                    Object.Equals(this.Name, other.Name);
            }

            public override int GetHashCode()
            {
                return StringKey.GetHashCode();
            }

            public override string ToString()
            {
                return String.Format("{0}.{1}", Owner.FullName, Name);
            }
        }

        public string Name { get; private set; }
        public Type OwnerType { get; private set; }
        public Type PropertyType { get; private set; }
        public ValidateValueCallback ValidateValueCallback { get; private set; }
        public bool IsReadOnly { get; private set; }
        public bool Inherits { get; private set; }
        public bool IsAttached { get; private set; }
        public string StringKey { get; private set; }

        private DependencyPropertyHashKey hashKey;
        private IMinimalDictionary<Type, PropertyMetadata> typeMetadata;
        private PropertyMetadata ownerMetadata;
        private bool isMetadataOverridden;
        private int hashCode;

        private CacheDictionary<Type, PropertyMetadata> typeMetadataCache;
        private CacheDictionary<Type, bool> typeContainsCache;
        private IEnumerable<Type> orderedTypeMetadataCache;

        private static readonly ConvertedStringDictionary<DependencyPropertyHashKey, DependencyProperty> registeredProperties = new ConvertedStringDictionary<DependencyPropertyHashKey, DependencyProperty>(hashKey => hashKey.StringKey);
        private static readonly ConvertedStringDictionary<DependencyPropertyHashKey, DependencyPropertyKey> registeredReadOnlyPropertiesKey = new ConvertedStringDictionary<DependencyPropertyHashKey, DependencyPropertyKey>(hashKey => hashKey.StringKey);

        private DependencyProperty(DependencyPropertyHashKey hashKey, Type propertyType, PropertyMetadata metadata, ValidateValueCallback validateValueCallback, bool isAttached, bool isReadOnly)
        {
            this.hashKey = hashKey;
            this.Name = hashKey.Name;
            this.OwnerType = hashKey.Owner;
            this.PropertyType = propertyType;
            this.ValidateValueCallback = validateValueCallback;
            this.IsReadOnly = isReadOnly;
            this.Inherits = metadata.Inherits;
            this.StringKey = hashKey.StringKey;
            this.hashCode = hashKey.GetHashCode();

            this.ownerMetadata = metadata;
            this.IsAttached = isAttached;

            typeMetadata = new ConvertedStringDictionary<Type, PropertyMetadata>(type => type.FullName);
            typeMetadata.Add(OwnerType, ownerMetadata);

            typeMetadataCache = CacheDictionary<Type, PropertyMetadata>.CreateUsingStringKeys(ResolveTypeMetadata, type => type.FullName);
            typeContainsCache = CacheDictionary<Type, bool>.CreateUsingStringKeys(ResolveTypeContains, type => type.FullName);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}", OwnerType.FullName, Name);
        }

        public bool IsValidValue(object value)
        {
            return IsValidType(value, PropertyType) && (ValidateValueCallback == null || ValidateValueCallback(value));
        }

        public void OverrideMetadata(Type forType, PropertyMetadata metadata)
        {
            if (typeMetadata.ContainsKey(forType))
            {
                throw new Granular.Exception("DependencyProperty \"{0}\" already contains metadata for type \"{1}\"", this, forType.Name);
            }

            if (metadata.DefaultValue == null)
            {
                metadata.DefaultValue = ownerMetadata.DefaultValue;
            }

            if (metadata.CoerceValueCallback == null)
            {
                metadata.CoerceValueCallback = ownerMetadata.CoerceValueCallback;
            }

            if (Inherits != metadata.Inherits)
            {
                throw new Granular.Exception("Overriding inheritance behavior is not supported, Overrides of \"{0}\" must declare \"inherits: {1}\"", this, Inherits);
            }

            if (Inherits)
            {
                if (metadata.DefaultValue != null && metadata.DefaultValue != ownerMetadata.DefaultValue)
                {
                    throw new Granular.Exception("Overriding inherited properties metadata with different default value is not supported, Overrides of \"{0}\" cannot set a different default value other than \"{1}\"", this, ownerMetadata.DefaultValue);
                }

                metadata.DefaultValue = ownerMetadata.DefaultValue;
            }

            typeMetadata.Add(forType, metadata);

            typeMetadataCache.Clear();
            orderedTypeMetadataCache = null;

            isMetadataOverridden = true;
        }

        public DependencyProperty AddOwner(Type ownerType, PropertyMetadata metadata = null)
        {
            AddRegisteredProperty(new DependencyPropertyHashKey(ownerType, Name), this);

            if (metadata != null)
            {
                OverrideMetadata(ownerType, metadata);
            }

            typeContainsCache.Clear();

            return this;
        }

        public PropertyMetadata GetMetadata(Type type)
        {
            if (!isMetadataOverridden)
            {
                return ownerMetadata;
            }

            return typeMetadataCache.GetValue(type);
        }

        private PropertyMetadata ResolveTypeMetadata(Type type)
        {
            Type closestBaseType = GetOrderedTypeMetadata().Where(baseType => type == baseType || type.IsSubclassOf(baseType)).LastOrDefault();
            return closestBaseType != null ? typeMetadata.GetValue(closestBaseType) : ownerMetadata;
        }

        public bool IsContainedBy(Type type)
        {
            return typeContainsCache.GetValue(type);
        }

        private bool ResolveTypeContains(Type type)
        {
            return typeMetadata.GetKeys().Any(baseType => type == baseType || type.IsSubclassOf(baseType));
        }

        internal void RaiseMetadataPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            // metadata's changed callback will be raised for
            // - the original owner metadata
            // - every attached property metadata
            // - every metadata that the currently changed object derives from its owner type

            foreach (Type type in GetOrderedTypeMetadata())
            {
                if (!type.IsInstanceOfType(dependencyObject) && (!IsAttached || type != OwnerType))
                {
                    continue;
                }

                PropertyMetadata propertyMetadata = typeMetadata.GetValue(type);

                if (propertyMetadata.PropertyChangedCallback != null)
                {
                    propertyMetadata.PropertyChangedCallback(dependencyObject, e);
                }
            }
        }

        private IEnumerable<Type> GetOrderedTypeMetadata()
        {
            if (orderedTypeMetadataCache == null)
            {
                // topological sorting, with the original owner type first, and each base class before all of its subclasses
                List<Type> orderedTypes = new List<Type> { OwnerType };
                List<Type> remainingTypes = typeMetadata.GetKeys().Where(type => type != OwnerType).ToList();

                while (remainingTypes.Any())
                {
                    Type nextType = remainingTypes.FirstOrDefault(type1 => remainingTypes.All(type2 => !type1.IsSubclassOf(type2)));
                    remainingTypes.Remove(nextType);
                    orderedTypes.Add(nextType);
                }

                orderedTypeMetadataCache = orderedTypes;
            }

            return orderedTypeMetadataCache;
        }

        private static DependencyProperty Register(DependencyPropertyHashKey key, Type propertyType, PropertyMetadata metadata, ValidateValueCallback validateValueCallback, bool isAttached, bool isReadOnly)
        {
            if (metadata == null)
            {
                metadata = new PropertyMetadata();
            }

            if (metadata.DefaultValue == null && propertyType.GetIsValueType())
            {
                metadata.DefaultValue = Activator.CreateInstance(propertyType);
            }

            if (metadata.DefaultValue != null && !propertyType.IsInstanceOfType(metadata.DefaultValue))
            {
                metadata.DefaultValue = ConvertDefaultValue(key, metadata.DefaultValue, propertyType);
            }

            DependencyProperty property = new DependencyProperty(key, propertyType, metadata, validateValueCallback, isAttached, isReadOnly);

            if (!property.IsValidValue(metadata.DefaultValue))
            {
                throw new Granular.Exception("Default value validation of dependency property \"{0}.{1}\" failed", key.Owner.Name, key.Name);
            }

            AddRegisteredProperty(key, property);

            return property;
        }

        public static DependencyProperty Register(string name, Type propertyType, Type ownerType, PropertyMetadata metadata = null, ValidateValueCallback validateValueCallback = null)
        {
            return Register(new DependencyPropertyHashKey(ownerType, name), propertyType, metadata, validateValueCallback, false, false);
        }

        public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType, PropertyMetadata metadata = null, ValidateValueCallback validateValueCallback = null)
        {
            return Register(new DependencyPropertyHashKey(ownerType, name), propertyType, metadata, validateValueCallback, true, false);
        }

        public static DependencyPropertyKey RegisterReadOnly(string name, Type propertyType, Type ownerType, PropertyMetadata metadata = null, ValidateValueCallback validateValueCallback = null)
        {
            DependencyPropertyHashKey hashKey = new DependencyPropertyHashKey(ownerType, name);
            DependencyPropertyKey readOnlyKey = new DependencyPropertyKey(Register(hashKey, propertyType, metadata, validateValueCallback, false, true));
            registeredReadOnlyPropertiesKey.Add(hashKey, readOnlyKey);
            return readOnlyKey;
        }

        public static DependencyPropertyKey RegisterAttachedReadOnly(string name, Type propertyType, Type ownerType, PropertyMetadata metadata = null, ValidateValueCallback validateValueCallback = null)
        {
            DependencyPropertyHashKey key = new DependencyPropertyHashKey(ownerType, name);
            DependencyPropertyKey readOnlyKey = new DependencyPropertyKey(Register(key, propertyType, metadata, validateValueCallback, true, true));
            registeredReadOnlyPropertiesKey.Add(key, readOnlyKey);
            return readOnlyKey;
        }

        public static bool IsValidReadOnlyKey(DependencyPropertyKey key)
        {
            DependencyPropertyKey registeredKey;
            return registeredReadOnlyPropertiesKey.TryGetValue(key.DependencyProperty.hashKey, out registeredKey) && registeredKey == key;
        }

        private static bool IsValidType(object value, Type propertyType)
        {
            return value == null ? !propertyType.GetIsValueType() || propertyType.GetIsGenericType() && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>) :
                propertyType.IsInstanceOfType(value);
        }

        private static object ConvertDefaultValue(DependencyPropertyHashKey key, object defaultValue, Type propertyType)
        {
            try
            {
                return Granular.Compatibility.Convert.ChangeType(defaultValue, propertyType);
            }
            catch (Exception e)
            {
                throw new Granular.Exception("Dependency property \"{0}.{1}\" default value \"{2}\" cannot be converted to \"{3}\" ({4})", key.Owner.Name, key.Name, defaultValue, propertyType, e.Message);
            }
        }

        private static void AddRegisteredProperty(DependencyPropertyHashKey key, DependencyProperty dependencyProperty)
        {
            VerifyNotRegistered(key, dependencyProperty);
            registeredProperties.Add(key, dependencyProperty);
        }

        private static void VerifyNotRegistered(DependencyPropertyHashKey key, DependencyProperty dependencyProperty)
        {
            DependencyProperty registeredDependencyProperty;
            if (registeredProperties.TryGetValue(key, out registeredDependencyProperty))
            {
                throw new Granular.Exception("Can't register dependency property \"{0}\", Type \"{1}\" already has a dependency property with the same name \"{2}\"", dependencyProperty, key.Owner.Name, registeredDependencyProperty);
            }
        }

        // Get property that is owned by containingType or one of its base classes
        public static DependencyProperty GetProperty(Type containingType, string propertyName)
        {
            DependencyProperty dependencyProperty;

            while (containingType != null && containingType != typeof(DependencyObject))
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(containingType.GetTypeHandle());

                if (registeredProperties.TryGetValue(new DependencyPropertyHashKey(containingType, propertyName), out dependencyProperty))
                {
                    return dependencyProperty;
                }

                containingType = containingType.BaseType;
            }

            return null;
        }
    }

    public class DependencyPropertyTypeConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, Uri sourceUri, object value)
        {
            string text = value.ToString().Trim();

            int typeSeparatorIndex = text.IndexOf(".");
            if (typeSeparatorIndex == -1)
            {
                throw new Granular.Exception("Dependency property \"{0}\" does not contain owner type name", text);
            }

            Type ownerType = TypeParser.ParseType(text.Substring(0, typeSeparatorIndex), namespaces);
            DependencyProperty dependencyProperty = DependencyProperty.GetProperty(ownerType, text.Substring(typeSeparatorIndex + 1));

            if (dependencyProperty == null)
            {
                throw new Granular.Exception("Can't find dependency property named \"{0}\"", text);
            }

            return dependencyProperty;
        }
    }
}
