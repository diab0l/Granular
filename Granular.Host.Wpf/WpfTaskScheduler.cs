extern alias wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Host.Wpf
{
    public class WpfTaskScheduler : System.Windows.Threading.ITaskScheduler
    {
        public static readonly WpfTaskScheduler Default = new WpfTaskScheduler();

        private WpfTaskScheduler()
        {
            //
        }

        public void ScheduleTask(TimeSpan timeSpan, Action action)
        {
            if (timeSpan == TimeSpan.Zero)
            {
                wpf::System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(action);
                return;
            }

            wpf::System.Windows.Threading.DispatcherTimer timer = new wpf::System.Windows.Threading.DispatcherTimer();
            timer.Interval = timeSpan;
            timer.Tick += (sender, e) =>
            {
                action();
                timer.Stop();
            };

            timer.Start();
        }
    }
}
