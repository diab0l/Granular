using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;
using Granular.Extensions;
using Granular.Collections;

namespace System.Windows
{
    internal class DataTriggerCondition : Freezable, IDataTriggerCondition, IDisposable
    {
        private static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(DataTriggerCondition), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((DataTriggerCondition)sender).OnValueChanged(e)));

        public event EventHandler IsMatchedChanged;
        private bool isMatched;
        public bool IsMatched
        {
            get { return isMatched; }
            private set
            {
                if (isMatched == value)
                {
                    return;
                }

                isMatched = value;
                IsMatchedChanged.Raise(this);
            }
        }

        private FrameworkElement element;
        private IExpressionProvider expressionProvider;
        private object value;
        private object resolvedValue;
        private bool isDisposed;

        private DataTriggerCondition(FrameworkElement element, IExpressionProvider expressionProvider, object value)
        {
            this.element = element;
            this.expressionProvider = expressionProvider;
            this.value = value;
        }

        private void Register()
        {
            SetInheritanceParent(element);
            SetValue(ValueProperty, expressionProvider);
        }

        public void Dispose()
        {
            isDisposed = true;
            SetInheritanceParent(null);
            ClearValue(ValueProperty);
        }

        private void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (isDisposed)
            {
                return;
            }

            if (e.NewValue != null && value != null)
            {
                resolvedValue = GetResolvedValue(value, e.NewValue.GetType());
                value = null;
            }

            IsMatched = Granular.Compatibility.EqualityComparer.Default.Equals(e.NewValue, resolvedValue);
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

        public static DataTriggerCondition Register(FrameworkElement element, IExpressionProvider expressionProvider, object value)
        {
            DataTriggerCondition condition = new DataTriggerCondition(element, expressionProvider, value);
            condition.Register();
            return condition;
        }
    }

    [ContentProperty("Setters")]
    public class DataTrigger : DataTriggerBase
    {
        public Binding Binding { get; set; }

        public object Value { get; set; }

        public ObservableCollection<ITriggerAction> Setters { get; private set; }

        protected override IEnumerable<ITriggerAction> TriggerActions { get { return Setters; } }

        public DataTrigger()
        {
            Setters = new ObservableCollection<ITriggerAction>();
        }

        public override IDataTriggerCondition CreateDataTriggerCondition(FrameworkElement element)
        {
            if (Binding == null)
            {
                throw new Granular.Exception("DataTrigger.Binding cannot be null");
            }

            return DataTriggerCondition.Register(element, Binding, Value);
        }
    }
}
