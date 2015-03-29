using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Threading
{
    public interface ITaskScheduler
    {
        void ScheduleTask(TimeSpan timeSpan, Action action);
    }

    public static class TaskSchedulerExtensions
    {
        public static void ScheduleTask(this ITaskScheduler taskScheduler, Action action)
        {
            taskScheduler.ScheduleTask(TimeSpan.Zero, action);
        }
    }
}
