using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Extensions;
using System.Windows.Data;
using System.Windows.Markup;

namespace System.Windows
{
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class Condition
    {
        public Binding Binding { get; set; }
        public IPropertyPathElement Property { get; set; }
        public string SourceName { get; set; }
        public object Value { get; set; }

        public IDataTriggerCondition CreateTriggerCondition(FrameworkElement element)
        {
            if (Property == null)
            {
                throw new Granular.Exception("Condition.Property cannot be null");
            }

            DependencyProperty dependencyProperty = Property.GetDependencyProperty(element.GetType());

            object resolvedValue = Value == null || dependencyProperty.PropertyType.IsInstanceOfType(Value) ? Value : TypeConverter.ConvertValue(Value.ToString(), dependencyProperty.PropertyType, XamlNamespaces.Empty);

            FrameworkElement source = SourceName.IsNullOrEmpty() ? element : NameScope.GetTemplateNameScope(element).FindName(SourceName) as FrameworkElement;

            return TriggerCondition.Register(source, dependencyProperty, resolvedValue);
        }

        public IDataTriggerCondition CreateDataTriggerCondition(FrameworkElement element)
        {
            if (Binding == null)
            {
                throw new Granular.Exception("Condition.Binding cannot be null");
            }

            return DataTriggerCondition.Register(element, Binding, Value);
        }
    }
}
