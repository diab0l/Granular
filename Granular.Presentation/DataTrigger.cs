using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;

namespace System.Windows
{
    [ContentProperty("Setters")]
    public class DataTrigger : Freezable, ITrigger
    {
        private class DataTriggerHandler : DependencyObject, IDisposable
        {
            public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(DataTriggerHandler), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((DataTriggerHandler)sender).OnValueChanged(e)));

            private FrameworkElement element;
            private object value;
            private object resolvedValue;
            private IEnumerable<ITriggerAction> actions;
            private BaseValueSource valueSource;

            public DataTriggerHandler(FrameworkElement element, IExpressionProvider expressionProvider, object value, IEnumerable<ITriggerAction> actions, BaseValueSource valueSource)
            {
                this.element = element;
                this.value = value;
                this.actions = actions;
                this.valueSource = valueSource;

                SetInheritanceParent(element);

                SetValue(ValueProperty, expressionProvider);
            }

            public void Dispose()
            {
                SetInheritanceParent(null);
            }

            private void ExecuteEnterActions()
            {
                foreach (ITriggerAction action in actions)
                {
                    action.EnterAction(element, valueSource);
                }
            }

            private void ExecuteExitActions()
            {
                foreach (ITriggerAction action in actions)
                {
                    action.ExitAction(element, valueSource);
                }
            }

            private void OnValueChanged(DependencyPropertyChangedEventArgs e)
            {
                if (e.NewValue != null && value != null)
                {
                    resolvedValue = GetResolvedValue(value, e.NewValue.GetType());
                    value = null;
                }

                if (Granular.Compatibility.EqualityComparer<object>.Default.Equals(e.NewValue, resolvedValue))
                {
                    ExecuteEnterActions();
                }

                if (Granular.Compatibility.EqualityComparer<object>.Default.Equals(e.OldValue, resolvedValue))
                {
                    ExecuteExitActions();
                }
            }

            private static object GetResolvedValue(object value, Type type)
            {
                if (type.IsInstanceOfType(value))
                {
                    return value;
                }

                object resolvedValue;
                if (TypeConverter.TryConvertValue(value.ToString(), type, XamlNamespaces.Empty, out resolvedValue))
                {
                    return resolvedValue;
                }

                return null;
            }
        }

        public Binding Binding { get; set; }

        public object Value { get; set; }

        public List<ITriggerAction> Setters { get; private set; }

        private Dictionary<FrameworkElement, IDisposable> handlers;

        public DataTrigger()
        {
            Setters = new List<ITriggerAction>();
            handlers = new Dictionary<FrameworkElement, IDisposable>();
        }

        public void Attach(FrameworkElement element, BaseValueSource valueSource)
        {
            if (Binding == null)
            {
                throw new Granular.Exception("DataTrigger.Binding cannot be null");
            }

            handlers.Add(element, new DataTriggerHandler(element, Binding, Value, Setters, valueSource));
        }

        public void Detach(FrameworkElement element, BaseValueSource valueSource)
        {
            handlers[element].Dispose();
            handlers.Remove(element);
        }
    }
}
