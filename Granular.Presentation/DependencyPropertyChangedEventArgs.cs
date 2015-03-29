using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows
{
    public delegate void DependencyPropertyChangedEventHandler(object sender, DependencyPropertyChangedEventArgs e);

    public class DependencyPropertyChangedEventArgs : EventArgs
    {
        public DependencyProperty Property { get; private set; }
        public object NewValue { get; private set; }
        public object OldValue { get; private set; }

        public DependencyPropertyChangedEventArgs(DependencyProperty property, object oldValue, object newValue)
        {
            this.Property = property;
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }

    public static class DependencyPropertyChangedEventHandlerExtensions
    {
        static public void Raise(this DependencyPropertyChangedEventHandler handler, object sender, DependencyPropertyChangedEventArgs e)
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}
