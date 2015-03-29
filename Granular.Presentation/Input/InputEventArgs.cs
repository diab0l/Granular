using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Input
{
    public abstract class InputEventArgs : RoutedEventArgs
    {
        public IInputDevice Device { get; private set; }
        public int Timestamp { get; private set; }

        public InputEventArgs(RoutedEvent routedEvent, object originalSource, IInputDevice inputDevice, int timestamp) :
            base(routedEvent, originalSource)
        {
            this.Device = inputDevice;
            this.Timestamp = timestamp;
        }
    }
}
