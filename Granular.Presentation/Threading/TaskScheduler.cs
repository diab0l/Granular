using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Threading
{
    public interface ITaskScheduler
    {
        IDisposable ScheduleTask(TimeSpan timeSpan, Action action);
    }

    public static class TaskSchedulerExtensions
    {
        public static IDisposable ScheduleTask(this ITaskScheduler taskScheduler, Action action)
        {
            return taskScheduler.ScheduleTask(TimeSpan.Zero, action);
        }
    }
}
