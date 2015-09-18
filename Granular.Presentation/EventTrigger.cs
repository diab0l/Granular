using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Granular.Extensions;

namespace System.Windows
{
    [ContentProperty("Actions")]
    public class EventTrigger : EventTriggerBase
    {
        private class EventTriggerCondition : IEventTriggerCondition, IDisposable
        {
            public event EventHandler EventRaised;

            private FrameworkElement element;
            private RoutedEvent routedEvent;

            private EventTriggerCondition(FrameworkElement element, RoutedEvent routedEvent)
            {
                this.element = element;
                this.routedEvent = routedEvent;
            }

            private void Register()
            {
                element.AddHandler(routedEvent, (RoutedEventHandler)RoutedEventHandler);
            }

            public void Dispose()
            {
                element.RemoveHandler(routedEvent, (RoutedEventHandler)RoutedEventHandler);
            }

            private void RoutedEventHandler(object sender, RoutedEventArgs e)
            {
                EventRaised.Raise(this);
            }

            public static EventTriggerCondition Register(FrameworkElement element, RoutedEvent routedEvent)
            {
                EventTriggerCondition condition = new EventTriggerCondition(element, routedEvent);
                condition.Register();
                return condition;
            }
        }

        public RoutedEvent RoutedEvent { get; set; }
        public string SourceName { get; set; }
        public List<ITriggerAction> Actions { get; private set; }

        protected override IEnumerable<ITriggerAction> TriggerActions { get { return Actions; } }

        public EventTrigger()
        {
            Actions = new List<ITriggerAction>();
        }

        public override IEventTriggerCondition CreateEventTriggerCondition(FrameworkElement element)
        {
            if (RoutedEvent == null)
            {
                throw new Granular.Exception("EventTrigger.RoutedEvent cannot be null");
            }

            return EventTriggerCondition.Register(element, RoutedEvent);
        }
    }
}
