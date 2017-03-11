using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Granular.Collections;
using System.Windows;

namespace Granular.Presentation.Tests.Threading
{
    public class TestTaskScheduler : ITaskScheduler
    {
        private class CancellableAction
        {
            public TimeSpan TimeSpan { get; private set; }

            private Action action;
            private bool isCancelled;

            public CancellableAction(TimeSpan timeSpan, Action action)
            {
                this.TimeSpan = timeSpan;
                this.action = action;
            }

            public void Cancel()
            {
                isCancelled = true;
            }

            public void Invoke()
            {
                if (!isCancelled)
                {
                    action();
                }
            }
        }

        public TimeSpan CurrentTime { get; private set; }

        private List<CancellableAction> actions;
        private bool isImmediateProcessingDisabled;
        private bool isProcessing;

        public TestTaskScheduler()
        {
            this.actions = new List<CancellableAction>();
        }

        public IDisposable ScheduleTask(TimeSpan timeSpan, Action action)
        {
            CancellableAction cancellableAction = new CancellableAction(CurrentTime + timeSpan, action);
            EnqueueAction(cancellableAction);

            if (timeSpan == TimeSpan.Zero && !isProcessing && !isImmediateProcessingDisabled)
            {
                ProcessDueOperations();
            }

            return new Disposable(() => cancellableAction.Cancel());
        }

        public void AdvanceBy(TimeSpan timeSpan)
        {
            if (timeSpan < TimeSpan.Zero)
            {
                throw new Granular.Exception("Time must be positive");
            }

            AdvanceTo(CurrentTime + timeSpan);
        }

        public void AdvanceTo(TimeSpan time)
        {
            if (time < CurrentTime)
            {
                throw new Granular.Exception("Time must larger than CurrentTime");
            }

            while (actions.Count > 0 && actions.First().TimeSpan <= time)
            {
                CurrentTime = actions.First().TimeSpan;
                ProcessDueOperations();
            }

            CurrentTime = time;
        }

        public void ProcessDueOperations()
        {
            isProcessing = true;

            while (actions.Count > 0 && actions.First().TimeSpan <= CurrentTime)
            {
                DequeueAction().Invoke();
            }

            isProcessing = false;
        }

        public IDisposable DisableImmediateProcessing()
        {
            if (isImmediateProcessingDisabled)
            {
                throw new Granular.Exception("Immediate processing is already disabled");
            }

            isImmediateProcessingDisabled = true;
            return new Disposable(() =>
            {
                isImmediateProcessingDisabled = false;
                ProcessDueOperations();
            });
        }

        private void EnqueueAction(CancellableAction cancellableAction)
        {
            int i = 0;
            foreach (CancellableAction action in actions)
            {
                if (cancellableAction.TimeSpan < action.TimeSpan)
                {
                    break;
                }

                i++;
            }

            actions.Insert(i, cancellableAction);
        }

        private CancellableAction DequeueAction()
        {
            CancellableAction cancellableAction = actions[0];
            actions.RemoveAt(0);
            return cancellableAction;
        }
    }
}
