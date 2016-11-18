using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xaml;
using Granular.Collections;
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

            private int hashCode;

            public DependencyPropertyHashKey(Type owner, string name)
            {
                this.Owner = owner;
                this.Name = name;

                this.hashCode = Owner.GetHashCode() ^ Name.GetHashCode();
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
                return hashCode;
            }

            public override string ToString()
            {
                return String.Format("{0}.{1}", Owner.FullName, Name);
            }
        }

        private class TypeComparer : IComparer<Type>
        {
            public static readonly TypeComparer Default = new TypeComparer();

            private TypeComparer()
            {
                //
            }

            public int Compare(Type x, Type y)
            {
                if (y.IsSubclassOf(x))
                {
                    return -1;
                }

                if (x.IsSubclassOf(y))
                {
                    return 1;
                }

                return 0;
            }
        }

        public string Name { get; private set; }
        public Type OwnerType { get; private set; }
        public Type PropertyType { get; private set; }
        public ValidateValueCallback ValidateValueCallback { get; private set; }
        public bool IsReadOnly { get; private set; }
        public bool Inherits { get; private set; }
        public bool IsAttached { get; private set; }

        private DependencyPropertyHashKey hashKey;
        private Dictionary<Type, PropertyMetadata> typeMetadata;
        private PropertyMetadata ownerMetadata;
        private bool isMetadataOverridden;

        private CacheDictionary<Type, PropertyMetadata> typeMetadataCache;
        private CacheDictionary<Type, bool> typeContainsCache;
        private IEnumerable<Type> orderedTypeMetadataCache;

        private static readonly Dictionary<DependencyPropertyHashKey, DependencyProperty> registeredProperties = new Dictionary<DependencyPropertyHashKey, DependencyProperty>();
        private static readonly Dictionary<DependencyPropertyHashKey, DependencyPropertyKey> registeredReadOnlyPropertiesKey = new Dictionary<DependencyPropertyHashKey, DependencyPropertyKey>();

        private static readonly ListDictionary<Type, DependencyProperty> typeRegisteredProperties = new ListDictionary<Type, DependencyProperty>();
        private static readonly CacheDictionary<Type, IEnumerable<DependencyProperty>> typeFlattenedPropertiesCache = new CacheDictionary<Type, IEnumerable<DependencyProperty>>(ResolveTypeFlattenedProperties);

        private DependencyProperty(DependencyPropertyHashKey hashKey, Type propertyType, PropertyMetadata metadata, ValidateValueCallback validateValueCallback, bool isAttached, bool isReadOnly)
        {
            this.hashKey = hashKey;
            this.Name = hashKey.Name;
            this.OwnerType = hashKey.Owner;
            this.PropertyType = propertyType;
            this.ValidateValueCallback = validateValueCallback;
            this.IsReadOnly = isReadOnly;
            this.Inherits = metadata.Inherits;

            this.ownerMetadata = metadata;
            this.IsAttached = isAttached;

            typeMetadata = new Dictionary<Type, PropertyMetadata>();
            typeMetadata.Add(OwnerType, ownerMetadata);

            typeMetadataCache = new CacheDictionary<Type, PropertyMetadata>(ResolveTypeMetadata);
            typeContainsCache = new CacheDictionary<Type, bool>(ResolveTypeContains);
        }

        public override int GetHashCode()
        {
            return hashKey.GetHashCode();
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
            return typeMetadataCache.GetValue(type);
        }

        private PropertyMetadata ResolveTypeMetadata(Type type)
        {
            if (!isMetadataOverridden)
            {
                return ownerMetadata;
            }

            Type closestBaseType = GetOrderedTypeMetadata().Where(baseType => type == baseType || type.IsSubclassOf(baseType)).LastOrDefault();
            return closestBaseType != null ? typeMetadata[closestBaseType] : ownerMetadata;
        }

        public bool IsContainedBy(Type type)
        {
            return typeContainsCache.GetValue(type);
        }

        private bool ResolveTypeContains(Type type)
        {
            return GetFlattenedProperties(type).Contains(this);
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

                PropertyMetadata propertyMetadata = typeMetadata[type];

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
                List<Type> remainingTypes = typeMetadata.Keys.Where(type => type != OwnerType).ToList();

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
            typeRegisteredProperties.Add(key.Owner, dependencyProperty);
        }

        private static void VerifyNotRegistered(DependencyPropertyHashKey key, DependencyProperty dependencyProperty)
        {
            DependencyProperty registeredDependencyProperty;
            if (registeredProperties.TryGetValue(key, out registeredDependencyProperty))
            {
                throw new Granular.Exception("Can't register dependency property \"{0}\", Type \"{1}\" already has a dependency property with the same name \"{2}\"", dependencyProperty, key.Owner.Name, registeredDependencyProperty);
            }
        }

        public static IEnumerable<DependencyProperty> GetProperties(Type containingType)
        {
            return typeRegisteredProperties.GetValues(containingType);
        }

        // get all the properties that are owned by the containingType or its base types
        public static IEnumerable<DependencyProperty> GetFlattenedProperties(Type containingType)
        {
            return typeFlattenedPropertiesCache.GetValue(containingType);
        }

        private static IEnumerable<DependencyProperty> ResolveTypeFlattenedProperties(Type containingType)
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(containingType.GetTypeHandle());

            return containingType.BaseType != null ?
                GetFlattenedProperties(containingType.BaseType).Concat(GetProperties(containingType)).ToArray() :
                GetProperties(containingType).ToArray();
        }

        // get a property that is owned by the containingType or one of its base types (and verify there is no more than one match)
        public static DependencyProperty GetSingleProperty(Type containingType, string propertyName)
        {
            DependencyProperty[] properties = GetFlattenedProperties(containingType).Where(property => property.Name == propertyName).ToArray();

            if (properties.Length > 1)
            {
                throw new Granular.Exception("Type \"{0}\" contains more than one property named \"{1}\" ({2})", containingType.Name, propertyName, properties.Select(property => property.ToString()).Aggregate((s1, s2) => String.Format("{0}, {1}", s1, s2)));
            }

            return properties.FirstOrDefault();
        }

        public static DependencyProperty GetOwnedProperty(Type ownerType, string propertyName)
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(ownerType.GetTypeHandle());

            DependencyProperty property;
            return registeredProperties.TryGetValue(new DependencyPropertyHashKey(ownerType, propertyName), out property) ? property : null;
        }

        public static DependencyProperty GetProperty(Type containingType, XamlName propertyName)
        {
            if (!propertyName.IsMemberName)
            {
                // containing type wasn't specified, so the property could be owned by a base type
                return GetSingleProperty(containingType, propertyName.LocalName);
            }

            Type ownerType = TypeParser.ParseType(propertyName.ContainingTypeName);
            return GetOwnedProperty(ownerType, propertyName.MemberName);
        }
    }

    public class DependencyPropertyTypeConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, object value)
        {
            string text = value.ToString().Trim();

            int typeSeparatorIndex = text.IndexOf(".");
            if (typeSeparatorIndex == -1)
            {
                throw new Granular.Exception("Dependency property \"{0}\" does not contain owner type name", text);
            }

            Type ownerType = TypeParser.ParseType(text.Substring(0, typeSeparatorIndex), namespaces);
            DependencyProperty dependencyProperty = DependencyProperty.GetOwnedProperty(ownerType, text.Substring(typeSeparatorIndex + 1));

            if (dependencyProperty == null)
            {
                throw new Granular.Exception("Can't find dependency property named \"{0}\"", text);
            }

            return dependencyProperty;
        }
    }
}
