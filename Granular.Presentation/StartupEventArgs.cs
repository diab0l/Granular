using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows
{
    public delegate void StartupEventHandler(object sender, StartupEventArgs e);

    public class StartupEventArgs : EventArgs
    {
        public static new readonly StartupEventArgs Empty = new StartupEventArgs();

        public string[] Args { get; private set; }

        private StartupEventArgs()
        {
            Args = new string[0];
        }
    }

    public static class StartupEventHandlerExtensions
    {
        public static void Raise(this StartupEventHandler handler, object sender, StartupEventArgs e)
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}
