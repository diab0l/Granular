using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xaml;
using Granular.Extensions;

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

    public static class PropertyAdapter
    {
        public static IPropertyAdapter CreateAdapter(Type targetType, XamlName propertyName)
        {
            if (propertyName.IsEmpty)
            {
                return null;
            }

            DependencyProperty dependencyProperty = DependencyProperty.GetProperty(targetType, propertyName);
            if (dependencyProperty != null)
            {
                return new DependencyPropertyAdapter(dependencyProperty);
            }

            PropertyInfo clrProperty = GetClrProperty(targetType, propertyName);
            if (clrProperty != null)
            {
                return new ClrPropertyAdapter(clrProperty);
            }

            return null;
        }

        private static PropertyInfo GetClrProperty(Type containingType, XamlName propertyName)
        {
            string propertyMemberName = propertyName.MemberName;
            Type propertyContainingType = propertyName.IsMemberName ? TypeParser.ParseType(propertyName.ContainingTypeName) : containingType;

            return propertyContainingType.GetInstanceProperty(propertyMemberName);
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
