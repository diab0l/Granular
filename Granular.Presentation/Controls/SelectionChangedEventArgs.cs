using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Controls
{
    public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);

    public class SelectionChangedEventArgs : RoutedEventArgs
    {
        public IEnumerable<object> AddedItems { get; private set; }
        public IEnumerable<object> RemovedItems { get; private set; }

        public SelectionChangedEventArgs(RoutedEvent routedEvent, object originalSource, IEnumerable<object> removedItems, IEnumerable<object> addedItems) :
            base(routedEvent, originalSource)
        {
            this.RemovedItems = removedItems;
            this.AddedItems = addedItems;
        }

        public override void InvokeEventHandler(System.Delegate handler, object target)
        {
            if (handler is SelectionChangedEventHandler)
            {
                ((SelectionChangedEventHandler)handler)(target, this);
            }
            else
            {
                base.InvokeEventHandler(handler, target);
            }
        }
    }
}