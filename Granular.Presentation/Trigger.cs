using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xaml;
using Granular.Extensions;
using Granular.Collections;

namespace System.Windows
{
    internal class TriggerCondition : IDataTriggerCondition, IDisposable
    {
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
        private DependencyProperty property;
        private object value;

        private TriggerCondition(FrameworkElement element, DependencyProperty property, object value)
        {
            this.element = element;
            this.property = property;
            this.value = value;
        }

        private void Register()
        {
            IsMatched = Granular.Compatibility.EqualityComparer.Default.Equals(element.GetValue(property), value);

            element.PropertyChanged += OnPropertyChanged;
        }

        public void Dispose()
        {
            element.PropertyChanged -= OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property != property)
            {
                return;
            }

            IsMatched = Granular.Compatibility.EqualityComparer.Default.Equals(e.NewValue, value);
        }

        public static TriggerCondition Register(FrameworkElement element, DependencyProperty property, object value)
        {
            TriggerCondition condition = new TriggerCondition(element, property, value);
            condition.Register();

            return condition;
        }
    }

    [ContentProperty("Setters")]
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class Trigger : DataTriggerBase
    {
        public IPropertyPathElement Property { get; set; }
        public object Value { get; set; }
        public string SourceName { get; set; }
        public ObservableCollection<ITriggerAction> Setters { get; private set; }

        protected override IEnumerable<ITriggerAction> TriggerActions { get { return Setters; } }

        public Trigger()
        {
            Setters = new ObservableCollection<ITriggerAction>();
        }

        public override IDataTriggerCondition CreateDataTriggerCondition(FrameworkElement element)
        {
            if (Property == null)
            {
                throw new Granular.Exception("Trigger.Property cannot be null");
            }

            DependencyProperty dependencyProperty = Property.GetDependencyProperty(element.GetType());

            object resolvedValue = Value == null || dependencyProperty.PropertyType.IsInstanceOfType(Value) ? Value : TypeConverter.ConvertValue(Value.ToString(), dependencyProperty.PropertyType, XamlNamespaces.Empty);

            FrameworkElement source = SourceName.IsNullOrEmpty() ? element : NameScope.GetTemplateNameScope(element).FindName(SourceName) as FrameworkElement;

            return TriggerCondition.Register(source, dependencyProperty, resolvedValue);
        }
    }
}
