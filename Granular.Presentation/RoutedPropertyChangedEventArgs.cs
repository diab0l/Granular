using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows
{
    public delegate void RoutedPropertyChangedEventHandler<T>(object sender, RoutedPropertyChangedEventArgs<T> e);

    public class RoutedPropertyChangedEventArgs<T> : RoutedEventArgs
    {
        public T OldValue { get; private set; }
        public T NewValue { get; private set; }

        public RoutedPropertyChangedEventArgs(RoutedEvent routedEvent, object originalSource, T oldValue, T newValue) :
            base(routedEvent, originalSource)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public override void InvokeEventHandler(Delegate handler, object target)
        {
            if (handler is RoutedPropertyChangedEventHandler<T>)
            {
                ((RoutedPropertyChangedEventHandler<T>)handler)(target, this);
            }
            else
            {
                base.InvokeEventHandler(handler, target);
            }
        }
    }
}
