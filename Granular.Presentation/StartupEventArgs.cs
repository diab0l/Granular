using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows
{
    public class StartupEventArgs : EventArgs
    {
        public static new readonly StartupEventArgs Empty = new StartupEventArgs();

        private StartupEventArgs()
        {
            //
        }
    }
}
