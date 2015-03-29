using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Input
{
    public interface IInputDevice
    {
        void Activate();
        void Deactivate();
    }
}
