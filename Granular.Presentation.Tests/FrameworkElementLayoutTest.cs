using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Granular.Presentation.Tests.Threading;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class FrameworkElementLayoutTest
    {
        private class TestContainer : Decorator
        {
            public int MeasureCount { get; private set; }
            public int ArrangeCount { get; private set; }

            protected override Size MeasureOverride(Size availableSize)
            {
                MeasureCount++;
                return base.MeasureOverride(availableSize);
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                ArrangeCount++;
                return base.ArrangeOverride(finalSize);
            }
        }

        private class TestElement : FrameworkElement
        {
            public Size MeasureSize { get; set; }
            public Size ArrangeSize { get; set; }

            public int MeasureCount { get; private set; }
            public int ArrangeCount { get; private set; }

            protected override Size MeasureOverride(Size availableSize)
            {
                MeasureCount++;
                return MeasureSize;
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                ArrangeCount++;
                return ArrangeSize;
            }
        }

        [TestMethod]
        public void MeasureVisibleOnlyTest()
        {
            TestTaskScheduler scheduler = (TestTaskScheduler)ApplicationHost.Current.TaskScheduler;
            using (scheduler.DisableImmediateProcessing())
            {
                TestElement child = new TestElement();

                TestContainer parent = new TestContainer();
                parent.Child = child;

                parent.InvalidateMeasure();
                parent.InvalidateArrange();
                child.InvalidateMeasure();
                child.InvalidateArrange();

                scheduler.ProcessDueOperations();

                Assert.AreEqual(0, parent.MeasureCount);
                Assert.AreEqual(0, parent.ArrangeCount);
                Assert.AreEqual(0, child.MeasureCount);
                Assert.AreEqual(0, child.ArrangeCount);

                Assert.IsFalse(parent.IsMeasureValid);
                Assert.IsFalse(parent.IsArrangeValid);
                Assert.IsFalse(child.IsMeasureValid);
                Assert.IsFalse(child.IsArrangeValid);
            }
        }

        [TestMethod]
        public void MeasureInvisibleOnceOnlyTest()
        {
            TestTaskScheduler scheduler = (TestTaskScheduler)ApplicationHost.Current.TaskScheduler;
            using (scheduler.DisableImmediateProcessing())
            {
                TestElement child = new TestElement { MeasureSize = new Size(100, 100), ArrangeSize = new Size(100, 100) };

                TestContainer parent = new TestContainer { IsRootElement = true };
                parent.Child = child;

                parent.Measure(Size.Infinity);
                parent.Arrange(new Rect(parent.DesiredSize));

                Assert.AreEqual(1, parent.MeasureCount);
                Assert.AreEqual(1, parent.ArrangeCount);
                Assert.AreEqual(1, child.MeasureCount);
                Assert.AreEqual(1, child.ArrangeCount);
                Assert.AreEqual(new Size(100, 100), child.DesiredSize);
                Assert.AreEqual(new Size(100, 100), parent.DesiredSize);

                child.Visibility = Visibility.Collapsed;

                scheduler.ProcessDueOperations();

                Assert.AreEqual(2, parent.MeasureCount);
                Assert.AreEqual(2, parent.ArrangeCount);
                Assert.AreEqual(1, child.MeasureCount);
                Assert.AreEqual(1, child.ArrangeCount);
                Assert.AreEqual(Size.Zero, child.DesiredSize);
                Assert.AreEqual(Size.Zero, parent.DesiredSize);

                child.InvalidateMeasure();
                child.InvalidateArrange();

                scheduler.ProcessDueOperations();

                Assert.AreEqual(2, parent.MeasureCount);
                Assert.AreEqual(2, parent.ArrangeCount);
                Assert.AreEqual(1, child.MeasureCount);
                Assert.AreEqual(1, child.ArrangeCount);

                parent.InvalidateMeasure();
                parent.InvalidateArrange();

                scheduler.ProcessDueOperations();

                Assert.AreEqual(3, parent.MeasureCount);
                Assert.AreEqual(3, parent.ArrangeCount);
                Assert.AreEqual(1, child.MeasureCount);
                Assert.AreEqual(1, child.ArrangeCount);
            }
        }

        [TestMethod]
        public void MeasureParentOnceTest()
        {
            TestTaskScheduler scheduler = (TestTaskScheduler)ApplicationHost.Current.TaskScheduler;
            using (scheduler.DisableImmediateProcessing())
            {
                TestContainer parent = new TestContainer();

                TestElement child = new TestElement
                {
                    MeasureSize = new Size(100, 100),
                    ArrangeSize = new Size(100, 100),
                };

                parent.IsRootElement = true;
                parent.Child = child;

                parent.Measure(new Size(100, 100));
                parent.Arrange(new Rect(0, 0, 200, 200));

                scheduler.ProcessDueOperations();

                Assert.AreEqual(1, parent.MeasureCount);
                Assert.AreEqual(1, parent.ArrangeCount);
                Assert.AreEqual(1, child.MeasureCount);
                Assert.AreEqual(1, child.ArrangeCount);

                Assert.IsTrue(parent.IsMeasureValid);
                Assert.IsTrue(parent.IsArrangeValid);
                Assert.IsTrue(child.IsMeasureValid);
                Assert.IsTrue(child.IsArrangeValid);
            }
        }

        [TestMethod]
        public void MeasureParentWhenChildChangedTest()
        {
            TestTaskScheduler scheduler = (TestTaskScheduler)ApplicationHost.Current.TaskScheduler;
            using (scheduler.DisableImmediateProcessing())
            {
                TestContainer parent = new TestContainer();

                TestElement child = new TestElement
                {
                    MeasureSize = new Size(100, 100),
                    ArrangeSize = new Size(100, 100),
                };

                parent.IsRootElement = true;
                parent.Child = child;

                parent.Measure(new Size(100, 100));
                parent.Arrange(new Rect(0, 0, 100, 100));

                scheduler.ProcessDueOperations();

                Assert.AreEqual(1, parent.MeasureCount);
                Assert.AreEqual(1, child.MeasureCount);

                child.InvalidateMeasure();

                scheduler.ProcessDueOperations();

                Assert.AreEqual(1, parent.MeasureCount);
                Assert.AreEqual(2, child.MeasureCount);

                child.MeasureSize = new Size(150, 150);
                child.InvalidateMeasure();

                scheduler.ProcessDueOperations();

                Assert.AreEqual(2, parent.MeasureCount);
                Assert.AreEqual(3, child.MeasureCount);
            }
        }

        [TestMethod]
        public void ArrangeWhenMeasureChangedTest()
        {
            TestTaskScheduler scheduler = (TestTaskScheduler)ApplicationHost.Current.TaskScheduler;
            using (scheduler.DisableImmediateProcessing())
            {
                TestElement element = new TestElement
                {
                    MeasureSize = new Size(100, 100),
                    ArrangeSize = new Size(100, 100),
                };

                element.IsRootElement = true;

                element.Measure(new Size(100, 100));
                element.Arrange(new Rect(0, 0, 100, 100));
                Assert.AreEqual(1, element.MeasureCount);
                Assert.AreEqual(1, element.ArrangeCount);

                element.InvalidateMeasure();
                scheduler.ProcessDueOperations();
                Assert.AreEqual(2, element.MeasureCount);
                Assert.AreEqual(1, element.ArrangeCount);

                element.MeasureSize = new Size(150, 150);
                element.InvalidateMeasure();
                scheduler.ProcessDueOperations();
                Assert.AreEqual(3, element.MeasureCount);
                Assert.AreEqual(2, element.ArrangeCount);
            }
        }
    }
}
