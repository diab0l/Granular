using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Granular.Host
{
    public class TaskScheduler : System.Windows.Threading.ITaskScheduler
    {
        public IDisposable ScheduleTask(TimeSpan timeSpan, Action action)
        {
            int token = Bridge.Html5.Window.SetTimeout(action, (int)timeSpan.TotalMilliseconds);
            return new Disposable(() => Bridge.Html5.Window.ClearTimeout(token));
        }
    }
}
