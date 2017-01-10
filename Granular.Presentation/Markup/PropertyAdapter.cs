using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Markup;
using Granular.Extensions;
using Granular.Collections;

namespace System.Windows.Markup
{
    public interface IPropertyAdapter
    {
        Type PropertyType { get; }
        bool HasGetter { get; }
        bool HasSetter { get; }
        object GetValue(object target);
        void SetValue(object target, object value, BaseValueSource valueSource);
    }

    public class TypeMemberKey
    {
        public Type Type { get; private set; }
        public string MemberName { get; private set; }
        public string StringKey { get; private set; }

        private int hashCode;

        public TypeMemberKey(Type type, string memberName)
        {
            this.Type = type;
            this.MemberName = memberName;
            this.StringKey = Type.FullName + "," + MemberName;

            this.hashCode = StringKey.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            TypeMemberKey other = obj as TypeMemberKey;

            return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
                Object.Equals(this.Type, other.Type) &&
                Object.Equals(this.MemberName, other.MemberName);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}", Type.FullName, MemberName);
        }
    }

    public static class PropertyAdapter
    {
        private static CacheDictionary<TypeMemberKey, IPropertyAdapter> adaptersCache = CacheDictionary<TypeMemberKey, IPropertyAdapter>.CreateUsingStringKeys(TryCreateAdapter, typeMemberKey => typeMemberKey.StringKey);

        public static IPropertyAdapter CreateAdapter(Type targetType, string propertyName)
        {
            IPropertyAdapter propertyAdapter;
            return adaptersCache.TryGetValue(new TypeMemberKey(targetType, propertyName), out propertyAdapter) ? propertyAdapter : null;
        }

        private static bool TryCreateAdapter(TypeMemberKey key, out IPropertyAdapter adapter)
        {
            adapter = null;

            DependencyProperty dependencyProperty = DependencyProperty.GetProperty(key.Type, key.MemberName);
            if (dependencyProperty != null)
            {
                adapter = new DependencyPropertyAdapter(dependencyProperty);
                return true;
            }

            PropertyInfo clrProperty = GetClrProperty(key.Type, key.MemberName);
            if (clrProperty != null)
            {
                adapter = new ClrPropertyAdapter(clrProperty);
                return true;
            }

            return false;
        }

        private static PropertyInfo GetClrProperty(Type containingType, string propertyName)
        {
            PropertyInfo propertyInfo = containingType.GetInstanceProperty(propertyName);
            return propertyInfo != null && !propertyInfo.IsDelegate() ? propertyInfo : null;
        }
    }

    public class DependencyPropertyAdapter : IPropertyAdapter
    {
        public Type PropertyType { get { return property.PropertyType; } }

        public bool HasGetter { get { return true; } }
        public bool HasSetter { get { return !property.IsReadOnly; } }

        private DependencyProperty property;

        public DependencyPropertyAdapter(DependencyProperty property)
        {
            this.property = property;
        }

        public object GetValue(object target)
        {
            return ((DependencyObject)target).GetValue(property);
        }

        public void SetValue(object target, object value, BaseValueSource valueSource)
        {
            ((DependencyObject)target).SetValue(property, value, valueSource);
        }
    }

    public class ClrPropertyAdapter : IPropertyAdapter
    {
        public Type PropertyType { get { return property.PropertyType; } }

        public bool HasGetter { get { return property.GetGetMethod() != null; } }
        public bool HasSetter { get { return property.GetSetMethod() != null; } }

        private PropertyInfo property;
        private object[] index;

        public ClrPropertyAdapter(PropertyInfo property, object[] index = null)
        {
            this.property = property;
            this.index = index ?? new object[0];
        }

        public object GetValue(object target)
        {
            return property.GetValue(target, index);
        }

        public void SetValue(object target, object value, BaseValueSource valueSource)
        {
            property.SetValue(target, value, index);
        }
    }
}
