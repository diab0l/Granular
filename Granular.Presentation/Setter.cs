using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xaml;
using Granular.Extensions;

namespace System.Windows
{
    public class Setter : ITriggerAction
    {
        public IPropertyPathElement Property { get; set; }
        public object Value { get; set; }
        public string TargetName { get; set; }

        public void EnterAction(FrameworkElement target, BaseValueSource valueSource)
        {
            if (Property == null)
            {
                throw new Granular.Exception("Setter.Property cannot be null");
            }

            FrameworkElement resolvedTarget = GetResolvedTarget(target, TargetName, valueSource);
            DependencyProperty resolvedProperty = Property.GetDependencyProperty(resolvedTarget.GetType());
            object resolvedValue = Value == null || Value is IExpressionProvider || resolvedProperty.PropertyType.IsInstanceOfType(Value) ? Value : TypeConverter.ConvertValue(Value.ToString(), resolvedProperty.PropertyType, XamlNamespaces.Empty);
            BaseValueSource resolvedValueSource = GetResolvedValueSource(valueSource, resolvedTarget);

            resolvedTarget.SetValue(resolvedProperty, resolvedValue, resolvedValueSource);
        }

        public void ExitAction(FrameworkElement target, BaseValueSource valueSource)
        {
            if (Property == null)
            {
                throw new Granular.Exception("Setter.Property cannot be null");
            }

            FrameworkElement resolvedTarget = GetResolvedTarget(target, TargetName, valueSource);
            DependencyProperty resolvedProperty = Property.GetDependencyProperty(resolvedTarget.GetType());
            BaseValueSource resolvedValueSource = GetResolvedValueSource(valueSource, resolvedTarget);

            resolvedTarget.ClearValue(resolvedProperty, resolvedValueSource);
        }

        public bool IsActionOverlaps(ITriggerAction action)
        {
            if (Property == null)
            {
                throw new Granular.Exception("Setter.Property cannot be null");
            }

            return action is Setter && this.TargetName == ((Setter)action).TargetName && this.Property.Equals(((Setter)action).Property);
        }

        private static FrameworkElement GetResolvedTarget(FrameworkElement target, string targetName, BaseValueSource valueSource)
        {
            return targetName.IsNullOrEmpty() ? target : (valueSource == BaseValueSource.Local ? NameScope.GetContainingNameScope(target) : NameScope.GetTemplateNameScope(target)).FindName(targetName) as FrameworkElement;
        }

        private static BaseValueSource GetResolvedValueSource(BaseValueSource valueSource, FrameworkElement target)
        {
            return valueSource == BaseValueSource.TemplateTrigger && target.TemplatedParent != null ? BaseValueSource.ParentTemplateTrigger : valueSource;
        }
    }
}
