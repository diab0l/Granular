using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace System.Windows.Threading
{
    public enum DispatcherOperationStatus
    {
        Pending,
        Aborted,
        Completed,
        Executing
    }

    public class DispatcherOperation
    {
        public event EventHandler Completed;
        public event EventHandler Aborted;

        public DispatcherPriority Priority { get; private set; }
        public DispatcherOperationStatus Status { get; private set; }
        public object Result { get; private set; }

        private Func<object> action;

        public DispatcherOperation(Action action) :
            this(DispatcherPriority.Background, action)
        {
            //
        }

        public DispatcherOperation(Func<object> action) :
            this(DispatcherPriority.Background, action)
        {
            //
        }

        public DispatcherOperation(DispatcherPriority priority, Action action) :
            this(priority, () => { action(); return null; })
        {
            //
        }

        public DispatcherOperation(DispatcherPriority priority, Func<object> action)
        {
            this.Priority = priority;
            this.action = action;
        }

        public void Abort()
        {
            if (Status != DispatcherOperationStatus.Pending)
            {
                throw new Granular.Exception("Operation is \"{0}\" and cannot be aborted", Status);
            }

            Status = DispatcherOperationStatus.Aborted;
            Aborted.Raise(this);
        }

        public void Invoke()
        {
            if (Status != DispatcherOperationStatus.Pending)
            {
                throw new Granular.Exception("Operation is \"{0}\" and cannot be invoked", Status);
            }

            Status = DispatcherOperationStatus.Executing;
            Result = action();
            Status = DispatcherOperationStatus.Completed;
            Completed.Raise(this);
        }
    }
}
