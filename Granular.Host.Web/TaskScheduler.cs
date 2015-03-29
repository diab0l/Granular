using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Host
{
    public class TaskScheduler : System.Windows.Threading.ITaskScheduler
    {
        public static readonly TaskScheduler Default = new TaskScheduler();

        private TaskScheduler()
        {
            //
        }

        public void ScheduleTask(TimeSpan timeSpan, Action action)
        {
            System.Html.Window.SetTimeout(action, (int)timeSpan.TotalMilliseconds);
        }
    }
}
