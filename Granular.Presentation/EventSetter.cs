using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows
{
    public class EventSetter : ITriggerAction
    {
        public RoutedEvent Event { get; set; }
        public RoutedEventHandler Handler { get; set; }
        public bool HandledEventsToo { get; set; }

        public void EnterAction(FrameworkElement target, BaseValueSource valueSource)
        {
            target.AddHandler(Event, Handler, HandledEventsToo);
        }

        public void ExitAction(FrameworkElement target, BaseValueSource valueSource)
        {
            target.RemoveHandler(Event, Handler);
        }

        public bool IsActionOverlaps(ITriggerAction action)
        {
            return action is EventSetter && this.Event == ((EventSetter)action).Event;
        }
    }
}
