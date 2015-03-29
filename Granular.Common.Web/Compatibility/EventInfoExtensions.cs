using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Reflection
{
    public static class EventInfoExtensions
    {
        public static Type GetEventHandlerType(this EventInfo eventInfo)
        {
            return typeof(Delegate);
        }
    }
}
