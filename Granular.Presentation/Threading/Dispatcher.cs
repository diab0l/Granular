using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular;
using Granular.Collections;

namespace System.Windows.Threading
{
    public enum DispatcherPriority
    {
        Invalid = -1,
        Inactive = 0,
        SystemIdle = 1,
        ApplicationIdle = 2,
        ContextIdle = 3,
        Background = 4,
        Input = 5,
        Loaded = 6,
        Render = 7,
        DataBind = 8,
        Normal = 9,
        Send = 10,
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
        private bool isProcessQueueScheduled;
        private IDisposable disableProcessingToken;

        public Dispatcher()
        {
            queue = new PriorityQueue<DispatcherPriority, DispatcherOperation>();
            disableProcessingToken = new Disposable(EnableProcessing);
        }

        public void Invoke(Action callback, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            Invoke(new DispatcherOperation(callback, priority));
        }

        public TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            DispatcherOperation<TResult> dispatcherOperation = new DispatcherOperation<TResult>(callback, priority);
            Invoke(dispatcherOperation);
            return dispatcherOperation.Result;
        }

        private void Invoke(DispatcherOperation operation)
        {
            queue.Enqueue(operation.Priority, operation);

            DispatcherOperation currentOperation;
            while (TryDequeue(out currentOperation))
            {
                currentOperation.Invoke();

                if (currentOperation == operation)
                {
                    return;
                }
            }

            if (disableProcessingRequests > 0)
            {
                throw new Granular.Exception("Can't invoke an operation while the dispatcher processing is disabled");
            }

            throw new Granular.Exception("Can't invoke an inactive or aborted operation");
        }

        public DispatcherOperation InvokeAsync(Action callback, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            DispatcherOperation dispatcherOperation = new DispatcherOperation(callback, priority);
            InvokeAsync(dispatcherOperation);
            return dispatcherOperation;
        }

        public DispatcherOperation<TResult> InvokeAsync<TResult>(Func<TResult> callback, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            DispatcherOperation<TResult> dispatcherOperation = new DispatcherOperation<TResult>(callback, priority);
            InvokeAsync(dispatcherOperation);
            return dispatcherOperation;
        }

        private void InvokeAsync(DispatcherOperation operation)
        {
            queue.Enqueue(operation.Priority, operation);
            ProcessQueueAsync();
        }

        private void ProcessQueueAsync()
        {
            if (isProcessQueueScheduled)
            {
                return;
            }

            isProcessQueueScheduled = true;
            ApplicationHost.Current.TaskScheduler.ScheduleTask(() =>
            {
                isProcessQueueScheduled = false;

                DispatcherOperation operation;
                if (!TryDequeue(out operation))
                {
                    return;
                }

                if (operation.Status == DispatcherOperationStatus.Pending)
                {
                    operation.Invoke();
                    ProcessQueueAsync();
                }
            });
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        private bool TryDequeue(out DispatcherOperation operation)
        {
            while (disableProcessingRequests == 0 && queue.TryPeek(out operation) && operation.Priority != DispatcherPriority.Inactive)
            {
                queue.Dequeue();

                if (operation.Status != DispatcherOperationStatus.Pending)
                {
                    continue;
                }

                return true;
            }

            operation = null;
            return false;
        }

        public IDisposable DisableProcessing()
        {
            disableProcessingRequests++;
            return disableProcessingToken;
        }

        private void EnableProcessing()
        {
            disableProcessingRequests--;

            if (disableProcessingRequests == 0)
            {
                ProcessQueueAsync();
            }
        }
    }
}
