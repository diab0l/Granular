using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xaml;
using Granular.Extensions;

namespace System.Windows
{
    public interface ITrigger
    {
        void Attach(FrameworkElement element, BaseValueSource valueSource);
        void Detach(FrameworkElement element, BaseValueSource valueSource);
    }

    public interface ITriggerAction
    {
        bool IsActionOverlaps(ITriggerAction action);
        void EnterAction(FrameworkElement target, BaseValueSource valueSource);
        void ExitAction(FrameworkElement target, BaseValueSource valueSource);
    }

    [ContentProperty("Setters")]
    public class Trigger : Freezable, ITrigger
    {
        private class TriggerHandler : IDisposable
        {
            private FrameworkElement element;
            private DependencyProperty property;
            private object value;
            private IEnumerable<ITriggerAction> actions;
            private BaseValueSource valueSource;

            private TriggerHandler(FrameworkElement element, DependencyProperty property, object value, IEnumerable<ITriggerAction> actions, BaseValueSource valueSource)
            {
                this.element = element;
                this.property = property;
                this.value = value;
                this.actions = actions;
                this.valueSource = valueSource;
            }

            private void OnPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                if (e.Property != property)
                {
                    return;
                }

                if (Granular.Compatibility.EqualityComparer<object>.Default.Equals(e.NewValue, value))
                {
                    ExecuteEnterActions();
                }

                if (Granular.Compatibility.EqualityComparer<object>.Default.Equals(e.OldValue, value))
                {
                    ExecuteExitActions();
                }
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

            public void Register()
            {
                element.PropertyChanged += OnPropertyChanged;

                if (Granular.Compatibility.EqualityComparer<object>.Default.Equals(element.GetValue(property), value))
                {
                    ExecuteEnterActions();
                }
            }

            public void Dispose()
            {
                element.PropertyChanged -= OnPropertyChanged;
            }

            public static IDisposable Register(FrameworkElement element, DependencyProperty property, object value, IEnumerable<ITriggerAction> actions, BaseValueSource valueSource)
            {
                TriggerHandler handler = new TriggerHandler(element, property, value, actions, valueSource);
                handler.Register();
                return handler;
            }
        }

        public IPropertyPathElement Property { get; set; }
        public object Value { get; set; }
        public string SourceName { get; set; }
        public List<ITriggerAction> Setters { get; private set; }

        private Dictionary<FrameworkElement, IDisposable> handlers;

        public Trigger()
        {
            Setters = new List<ITriggerAction>();
            handlers = new Dictionary<FrameworkElement, IDisposable>();
        }

        public void Attach(FrameworkElement element, BaseValueSource valueSource)
        {
            if (Property == null)
            {
                throw new Granular.Exception("Trigger.Property cannot be null");
            }

            DependencyProperty dependencyProperty = Property.GetDependencyProperty(element.GetType());

            object resolvedValue = Value == null || dependencyProperty.PropertyType.IsInstanceOfType(Value) ? Value : TypeConverter.ConvertValue(Value.ToString(), dependencyProperty.PropertyType, XamlNamespaces.Empty);

            FrameworkElement source = SourceName.IsNullOrEmpty() ? element : NameScope.GetTemplateNameScope(element).FindName(SourceName) as FrameworkElement;
            handlers.Add(element, TriggerHandler.Register(source, dependencyProperty, resolvedValue, Setters, valueSource));
        }

        public void Detach(FrameworkElement element, BaseValueSource valueSource)
        {
            handlers[element].Dispose();
            handlers.Remove(element);
        }
    }
}
