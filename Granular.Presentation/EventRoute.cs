using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows
{
    public class EventRouteItem
    {
        public RoutedEventHandlerItem handler;
        public object originalSource;
        public object source;
        public object target;

        public EventRouteItem(RoutedEventHandlerItem handler, object originalSource, object source, object target)
        {
            this.handler = handler;
            this.originalSource = originalSource;
            this.source = source;
            this.target = target;
        }

        public void InvokeHandler(RoutedEventArgs e)
        {
            if (!e.Handled || handler.HandledEventsToo)
            {
                e.Source = source;
                e.InvokeEventHandler(handler.Handler, target);
            }
        }
    }

    public class EventRoute
    {
        private RoutedEvent routedEvent;
        private IEnumerable<EventRouteItem> items;

        public EventRoute(RoutedEvent routedEvent, IEnumerable<EventRouteItem> items)
        {
            this.routedEvent = routedEvent;
            this.items = items;
        }

        public void InvokeHandlers(RoutedEventArgs e)
        {
            foreach (EventRouteItem item in items)
            {
                item.InvokeHandler(e);
            }
        }
    }
}
