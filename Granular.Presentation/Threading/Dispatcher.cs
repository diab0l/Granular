using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Collections;

namespace System.Windows.Threading
{
    public enum DispatcherPriority
    {
        Invalid = -1,
        Inactive = 0,
        SystemIdle,
        ApplicationIdle,
        ContextIdle,
        Background,
        Input,
        Loaded,
        Render,
        DataBind,
        Normal,
        Send
    }

    // ranges:
    // idle:       { SystemIdle, ApplicationIdle, ContextIdle }
    // background: { Background, Input }
    // foreground: { Loaded, Render, DataBind, Normal, Send }

    public class Dispatcher
    {
        public static readonly Dispatcher CurrentDispatcher = new Dispatcher();

        private PriorityQueue<DispatcherPriority, DispatcherOperation> queue;
        private int disableProcessingRequests;

        public Dispatcher()
        {
            queue = new PriorityQueue<DispatcherPriority, DispatcherOperation>();
        }

        public DispatcherOperation BeginInvoke(Action action)
        {
            return BeginInvoke(DispatcherPriority.Background, action);
        }

        public DispatcherOperation BeginInvoke(Func<object> action)
        {
            return BeginInvoke(DispatcherPriority.Background, action);
        }

        public DispatcherOperation BeginInvoke(DispatcherPriority priority, Func<object> action)
        {
            DispatcherOperation operation = new DispatcherOperation(priority, action);
            BeginInvoke(operation);
            return operation;
        }

        public DispatcherOperation BeginInvoke(DispatcherPriority priority, Action action)
        {
            DispatcherOperation operation = new DispatcherOperation(priority, action);
            BeginInvoke(operation);
            return operation;
        }

        public void BeginInvoke(DispatcherOperation operation)
        {
            queue.Enqueue(operation.Priority, operation);
            ProcessQueue();
        }

        public void ProcessQueue()
        {
            DispatcherOperation operation;

            while (disableProcessingRequests == 0 && queue.TryDequeue(out operation))
            {
                if (operation.Status == DispatcherOperationStatus.Pending)
                {
                    operation.Completed += (sender, e) => ProcessQueue();
                    operation.Aborted += (sender, e) => ProcessQueue();

                    ApplicationHost.Current.TaskScheduler.ScheduleTask(operation.Invoke);
                    return;
                }
            }
        }

        public IDisposable DisableProcessing()
        {
            disableProcessingRequests++;
            return new Disposable(() => { disableProcessingRequests--; ProcessQueue(); });
        }
    }
}
