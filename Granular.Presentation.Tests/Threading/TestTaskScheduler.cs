using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Granular.Collections;

namespace Granular.Presentation.Tests.Threading
{
    public class TestTaskScheduler : ITaskScheduler
    {
        public TimeSpan CurrentTime { get; private set; }

        private PriorityQueue<TimeSpan, Action> queue;

        public TestTaskScheduler()
        {
            this.queue = new PriorityQueue<TimeSpan, Action>();
        }

        public void ScheduleTask(TimeSpan timeSpan, Action action)
        {
            queue.Enqueue(CurrentTime + timeSpan, action);
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

            while (queue.Count > 0 && queue.First().Key <= time)
            {
                CurrentTime = queue.First().Key;
                DequeueDueOperations();
            }

            CurrentTime = time;
        }

        private void DequeueDueOperations()
        {
            while (queue.Count > 0 && queue.First().Key <= CurrentTime)
            {
                queue.Dequeue().Invoke();
            }
        }
    }
}
