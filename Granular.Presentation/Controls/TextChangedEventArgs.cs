using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Controls
{
    public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs e);

    public class TextChangedEventArgs : RoutedEventArgs
    {
        public TextChangedEventArgs(RoutedEvent routedEvent, object originalSource) :
            base(routedEvent, originalSource)
        {
            //
        }

        public override void InvokeEventHandler(Delegate handler, object target)
        {
            if (handler is TextChangedEventHandler)
            {
                ((TextChangedEventHandler)handler)(target, this);
            }
            else
            {
                base.InvokeEventHandler(handler, target);
            }
        }
    }
}
