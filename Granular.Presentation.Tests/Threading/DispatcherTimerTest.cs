using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Threading
{
    [TestClass]
    public class DispatcherTimerTest
    {
        [TestMethod]
        public void DispatcherTimerStartStopTest()
        {
            TestTaskScheduler scheduler = new TestTaskScheduler();
            DispatcherTimer timer = new DispatcherTimer(Dispatcher.CurrentDispatcher, scheduler, TimeSpan.FromMilliseconds(10), DispatcherPriority.Normal);

            int ticksCount = 0;

            timer.Tick += (sender, e) => ticksCount++;

            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));
            Assert.AreEqual(0, ticksCount);

            timer.Start();

            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));
            Assert.AreEqual(1, ticksCount);

            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(5));
            Assert.AreEqual(1, ticksCount);

            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(5));
            Assert.AreEqual(2, ticksCount);

            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(20));
            Assert.AreEqual(4, ticksCount);

            timer.Stop();

            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));
            Assert.AreEqual(4, ticksCount);
        }
    }
}
