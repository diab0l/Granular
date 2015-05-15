using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xaml;
using Granular.Extensions;
using System.Windows.Data;

namespace System.Windows
{
    public class Setter : ITriggerAction
    {
        private class ValueOverlapExpression : IExpressionProvider, IExpression
        {
            public event EventHandler<ObservableValueChangedArgs> ValueChanged;

            public object Value { get { return observableValue.Value; } }

            private List<Tuple<object, object>> values;
            private ObservableValue observableValue;

            public ValueOverlapExpression()
            {
                values = new List<Tuple<object, object>>();

                observableValue = new ObservableValue();
                observableValue.ValueChanged += (sender, e) => ValueChanged.Raise(this, e);
            }

            public IExpression CreateExpression(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
            {
                return this;
            }

            public bool SetValue(object value)
            {
                return false;
            }

            public void SetValue(object owner, object value)
            {
                values.Remove(values.FirstOrDefault(tuple => tuple.Item1 == owner));
                values.Add(Tuple.Create(owner, value));

                observableValue.Value = value;
            }

            public void ClearValue(object owner)
            {
                values.Remove(values.FirstOrDefault(tuple => tuple.Item1 == owner));

                observableValue.Value = values.Count > 0 ? values.Last().Item2 : ObservableValue.UnsetValue;
            }
        }

        public IPropertyPathElement Property { get; set; }
        public object Value { get; set; }
        public string TargetName { get; set; }

        public void Apply(FrameworkElement target, BaseValueSource valueSource)
        {
            if (Property == null)
            {
                throw new Granular.Exception("Setter.Property cannot be null");
            }

            FrameworkElement resolvedTarget = GetResolvedTarget(target, TargetName, valueSource);
            DependencyProperty resolvedProperty = Property.GetDependencyProperty(resolvedTarget.GetType());
            object resolvedValue = Value == null || Value is IExpressionProvider || resolvedProperty.PropertyType.IsInstanceOfType(Value) ? Value : TypeConverter.ConvertValue(Value.ToString(), resolvedProperty.PropertyType, XamlNamespaces.Empty);
            BaseValueSource resolvedValueSource = GetResolvedValueSource(valueSource, resolvedTarget);

            if (IsStyleValueSource(valueSource)) // no need to use value overlap expression in style setters
            {
                resolvedTarget.SetValue(resolvedProperty, resolvedValue, resolvedValueSource);
            }
            else
            {
                GetInitializedValueOverlapExpression(resolvedTarget, resolvedProperty, resolvedValueSource).SetValue(this, resolvedValue);
            }
        }

        public void Clean(FrameworkElement target, BaseValueSource valueSource)
        {
            if (Property == null)
            {
                throw new Granular.Exception("Setter.Property cannot be null");
            }

            FrameworkElement resolvedTarget = GetResolvedTarget(target, TargetName, valueSource);
            DependencyProperty resolvedProperty = Property.GetDependencyProperty(resolvedTarget.GetType());
            BaseValueSource resolvedValueSource = GetResolvedValueSource(valueSource, resolvedTarget);

            if (IsStyleValueSource(valueSource))
            {
                resolvedTarget.ClearValue(resolvedProperty, resolvedValueSource);
            }
            else
            {
                GetInitializedValueOverlapExpression(resolvedTarget, resolvedProperty, resolvedValueSource).ClearValue(this);
            }
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

        private static ValueOverlapExpression GetInitializedValueOverlapExpression(FrameworkElement target, DependencyProperty property, BaseValueSource valueSource)
        {
            ValueOverlapExpression valueOverlapExpression = target.GetValueEntry(property).GetBaseValue((int)valueSource, false) as ValueOverlapExpression;

            if (valueOverlapExpression == null)
            {
                valueOverlapExpression = new ValueOverlapExpression();
                target.SetValue(property, valueOverlapExpression, valueSource);
            }

            return valueOverlapExpression;
        }

        private static bool IsStyleValueSource(BaseValueSource valueSource)
        {
            return valueSource == BaseValueSource.ThemeStyle || valueSource == BaseValueSource.Style;
        }
    }
}
