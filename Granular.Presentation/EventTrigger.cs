using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Granular.Extensions;

namespace System.Windows
{
    [ContentProperty("Actions")]
    public class EventTrigger : Freezable, ITrigger
    {
        private class EventTriggerHandler : IDisposable
        {
            private FrameworkElement element;
            private RoutedEvent routedEvent;
            private IEnumerable<ITriggerAction> actions;
            private BaseValueSource valueSource;

            private EventTriggerHandler(FrameworkElement element, RoutedEvent routedEvent, IEnumerable<ITriggerAction> actions, BaseValueSource valueSource)
            {
                this.element = element;
                this.routedEvent = routedEvent;
                this.actions = actions;
                this.valueSource = valueSource;
            }

            private void RoutedEventHandler(object sender, RoutedEventArgs e)
            {
                ExecuteEnterActions();
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
                element.AddHandler(routedEvent, (RoutedEventHandler)RoutedEventHandler);
            }

            public void Dispose()
            {
                element.RemoveHandler(routedEvent, (RoutedEventHandler)RoutedEventHandler);
                ExecuteExitActions();
            }

            public static IDisposable Register(FrameworkElement element, RoutedEvent routedEvent, IEnumerable<ITriggerAction> actions, BaseValueSource valueSource)
            {
                EventTriggerHandler handler = new EventTriggerHandler(element, routedEvent, actions, valueSource);
                handler.Register();
                return handler;
            }
        }

        public RoutedEvent RoutedEvent { get; set; }
        public string SourceName { get; set; }
        public List<ITriggerAction> Actions { get; private set; }

        private Dictionary<FrameworkElement, IDisposable> handlers;

        public EventTrigger()
        {
            Actions = new List<ITriggerAction>();
            handlers = new Dictionary<FrameworkElement, IDisposable>();
        }

        public void Attach(FrameworkElement element, BaseValueSource valueSource)
        {
            if (RoutedEvent == null)
            {
                throw new Granular.Exception("EventTrigger.RoutedEvent cannot be null");
            }

            FrameworkElement source = SourceName.IsNullOrEmpty() ? element : (valueSource == BaseValueSource.Local ? NameScope.GetContainingNameScope(element) : NameScope.GetTemplateNameScope(element)).FindName(SourceName) as FrameworkElement;
            handlers.Add(element, EventTriggerHandler.Register(source, RoutedEvent, Actions, valueSource));
        }

        public void Detach(FrameworkElement element, BaseValueSource valueSource)
        {
            handlers[element].Dispose();
            handlers.Remove(element);
        }
    }
}
