using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;

namespace Granular.Presentation.Tests.Threading
{
    [TestClass]
    public class DispatcherTest
    {
        [TestMethod]
        public void DispatcherInvokeAsyncBasicTest()
        {
            TestTaskScheduler scheduler = (TestTaskScheduler)ApplicationHost.Current.TaskScheduler;
            using (scheduler.DisableImmediateProcessing())
            {
                Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

                int index = 1;
                int task1 = 0;
                int task2 = 0;
                int task3 = 0;

                dispatcher.InvokeAsync(() => task3 = index++, (DispatcherPriority)1);
                dispatcher.InvokeAsync(() => task1 = index++, (DispatcherPriority)3);
                dispatcher.InvokeAsync(() => task2 = index++, (DispatcherPriority)2);

                Assert.AreEqual(0, task1);
                Assert.AreEqual(0, task2);
                Assert.AreEqual(0, task3);

                scheduler.ProcessDueOperations();

                Assert.AreEqual(1, task1);
                Assert.AreEqual(2, task2);
                Assert.AreEqual(3, task3);
            }
        }

        [TestMethod]
        public void DispatcherInvokeAsyncPriorityTest()
        {
            TestTaskScheduler scheduler = (TestTaskScheduler)ApplicationHost.Current.TaskScheduler;
            using (scheduler.DisableImmediateProcessing())
            {
                Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

                int index = 1;
                int task1 = 0;
                int task2 = 0;
                int task3 = 0;

                dispatcher.InvokeAsync(() => task3 = index++, (DispatcherPriority)1);
                dispatcher.InvokeAsync(() =>
                {
                    task1 = index++;
                    dispatcher.InvokeAsync(() => task2 = index++, (DispatcherPriority)2);
                }, (DispatcherPriority)3);

                Assert.AreEqual(0, task1);
                Assert.AreEqual(0, task2);
                Assert.AreEqual(0, task3);

                scheduler.ProcessDueOperations();

                Assert.AreEqual(1, task1);
                Assert.AreEqual(2, task2);
                Assert.AreEqual(3, task3);
            }
        }

        [TestMethod]
        public void DispatcherInvokeBasicTest()
        {
            TestTaskScheduler scheduler = (TestTaskScheduler)ApplicationHost.Current.TaskScheduler;
            using (scheduler.DisableImmediateProcessing())
            {
                Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

                int index = 1;
                int task1 = 0;
                int task2 = 0;
                int task3 = 0;

                dispatcher.InvokeAsync(() => task3 = index++, (DispatcherPriority)1);
                dispatcher.InvokeAsync(() => task1 = index++, (DispatcherPriority)3);
                dispatcher.Invoke(() => task2 = index++, (DispatcherPriority)2);

                Assert.AreEqual(1, task1);
                Assert.AreEqual(2, task2);
                Assert.AreEqual(0, task3);

                scheduler.ProcessDueOperations();

                Assert.AreEqual(1, task1);
                Assert.AreEqual(2, task2);
                Assert.AreEqual(3, task3);
            }
        }

        [TestMethod]
        public void DispatcherDisableProcessingTest()
        {
            TestTaskScheduler scheduler = (TestTaskScheduler)ApplicationHost.Current.TaskScheduler;
            using (scheduler.DisableImmediateProcessing())
            {
                Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

                int index = 1;
                int task1 = 0;
                int task2 = 0;
                IDisposable dispatcherProcessingDisabled = null;

                dispatcher.InvokeAsync(() =>
                {
                    task1 = index++;
                    dispatcherProcessingDisabled = dispatcher.DisableProcessing();
                });

                dispatcher.InvokeAsync(() => task2 = index++);

                Assert.AreEqual(0, task1);
                Assert.AreEqual(0, task2);
                Assert.AreEqual(null, dispatcherProcessingDisabled);

                scheduler.ProcessDueOperations();
                Assert.AreEqual(1, task1);
                Assert.AreEqual(0, task2);
                Assert.AreNotEqual(null, dispatcherProcessingDisabled);

                dispatcherProcessingDisabled.Dispose();
                Assert.AreEqual(1, task1);
                Assert.AreEqual(0, task2);

                scheduler.ProcessDueOperations();
                Assert.AreEqual(1, task1);
                Assert.AreEqual(2, task2);
            }
        }
    }
}
