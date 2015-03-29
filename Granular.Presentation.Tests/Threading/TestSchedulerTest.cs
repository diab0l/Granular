using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Threading
{
    [TestClass]
    public class TestSchedulerTest
    {
        [TestMethod]
        public void SchedulingTest()
        {
            int dueOperationsCount = 0;

            TestTaskScheduler scheduler = new TestTaskScheduler();

            scheduler.ScheduleTask(TimeSpan.FromMilliseconds(1), () => dueOperationsCount++);
            scheduler.ScheduleTask(TimeSpan.FromMilliseconds(2), () => dueOperationsCount++);
            scheduler.ScheduleTask(TimeSpan.FromMilliseconds(2), () => dueOperationsCount++);
            scheduler.ScheduleTask(TimeSpan.FromMilliseconds(4), () => dueOperationsCount++);
            scheduler.ScheduleTask(TimeSpan.FromMilliseconds(4), () => dueOperationsCount++);
            scheduler.ScheduleTask(TimeSpan.FromMilliseconds(5), () => dueOperationsCount++);

            scheduler.AdvanceTo(TimeSpan.FromMilliseconds(1));
            Assert.AreEqual(1, dueOperationsCount);

            scheduler.AdvanceTo(TimeSpan.FromMilliseconds(3));
            Assert.AreEqual(3, dueOperationsCount);

            scheduler.AdvanceTo(TimeSpan.FromMilliseconds(6));
            Assert.AreEqual(6, dueOperationsCount);
        }
    }
}
