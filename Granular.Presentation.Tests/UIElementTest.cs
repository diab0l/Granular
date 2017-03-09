using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Granular.Presentation.Tests.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class UIElementTest
    {
        [TestMethod]
        public void UIElementIsVisibleTest()
        {
            UIElement element1 = new UIElement();
            UIElement element2 = new UIElement();
            UIElement element3 = new UIElement();

            element1.AddVisualChild(element2);
            element2.AddVisualChild(element3);

            Assert.IsFalse(element1.IsVisible);
            Assert.IsFalse(element2.IsVisible);
            Assert.IsFalse(element3.IsVisible);

            element1.IsRootElement = true;
            Assert.IsTrue(element1.IsVisible);
            Assert.IsTrue(element2.IsVisible);
            Assert.IsTrue(element3.IsVisible);

            element2.Visibility = Visibility.Collapsed;
            Assert.IsTrue(element1.IsVisible);
            Assert.IsFalse(element2.IsVisible);
            Assert.IsFalse(element3.IsVisible);

            element3.Visibility = Visibility.Hidden;
            Assert.IsTrue(element1.IsVisible);
            Assert.IsFalse(element2.IsVisible);
            Assert.IsFalse(element3.IsVisible);

            element2.Visibility = Visibility.Visible;
            Assert.IsTrue(element1.IsVisible);
            Assert.IsTrue(element2.IsVisible);
            Assert.IsFalse(element3.IsVisible);

            element1.IsRootElement = false;
            Assert.IsFalse(element1.IsVisible);
            Assert.IsFalse(element2.IsVisible);
            Assert.IsFalse(element3.IsVisible);

            element1.IsRootElement = true;
            element1.RemoveVisualChild(element2);
            Assert.IsTrue(element1.IsVisible);
            Assert.IsFalse(element2.IsVisible);
            Assert.IsFalse(element3.IsVisible);
        }

        [TestMethod]
        public void UIElementIsEnabledTest()
        {
            UIElement element1 = new UIElement();
            UIElement element2 = new UIElement();
            UIElement element3 = new UIElement();

            element1.AddVisualChild(element2);
            element2.AddVisualChild(element3);

            Assert.IsTrue(element1.IsEnabled);
            Assert.IsTrue(element2.IsEnabled);
            Assert.IsTrue(element3.IsEnabled);

            element1.IsEnabled = false;
            Assert.IsFalse(element1.IsEnabled);
            Assert.IsFalse(element2.IsEnabled);
            Assert.IsFalse(element3.IsEnabled);

            element2.IsEnabled = false;
            element1.IsEnabled = true;
            Assert.IsTrue(element1.IsEnabled);
            Assert.IsFalse(element2.IsEnabled);
            Assert.IsFalse(element3.IsEnabled);

            element3.IsEnabled = true;
            Assert.IsFalse(element3.IsEnabled);

            element2.RemoveVisualChild(element3);
            Assert.IsTrue(element3.IsEnabled);
        }

        [TestMethod]
        public void UIElementIsHitTestVisibleTest()
        {
            UIElement element1 = new UIElement();
            UIElement element2 = new UIElement();
            UIElement element3 = new UIElement();

            element1.AddVisualChild(element2);
            element2.AddVisualChild(element3);

            Assert.IsTrue(element1.IsHitTestVisible);
            Assert.IsTrue(element2.IsHitTestVisible);
            Assert.IsTrue(element3.IsHitTestVisible);

            element1.IsHitTestVisible = false;
            Assert.IsFalse(element1.IsHitTestVisible);
            Assert.IsFalse(element2.IsHitTestVisible);
            Assert.IsFalse(element3.IsHitTestVisible);

            element2.IsHitTestVisible = false;
            element1.IsHitTestVisible = true;
            Assert.IsTrue(element1.IsHitTestVisible);
            Assert.IsFalse(element2.IsHitTestVisible);
            Assert.IsFalse(element3.IsHitTestVisible);

            element3.IsHitTestVisible = true;
            Assert.IsFalse(element3.IsHitTestVisible);

            element2.RemoveVisualChild(element3);
            Assert.IsTrue(element3.IsHitTestVisible);
        }

        [TestMethod]
        public void LayoutInvalidationTest()
        {
            TestTaskScheduler scheduler = (TestTaskScheduler)ApplicationHost.Current.TaskScheduler;
            using (scheduler.DisableImmediateProcessing())
            {
                UIElement root = new UIElement { IsRootElement = true };
                UIElement child = new UIElement();

                root.Measure(Size.Infinity);
                root.Arrange(new Rect(root.DesiredSize));

                Assert.IsTrue(root.IsMeasureValid);
                Assert.IsTrue(root.IsArrangeValid);

                root.AddVisualChild(child);

                Assert.IsFalse(root.IsMeasureValid);
                Assert.IsTrue(root.IsArrangeValid);

                scheduler.ProcessDueOperations();

                Assert.IsTrue(root.IsMeasureValid);
                Assert.IsTrue(root.IsArrangeValid);

                root.RemoveVisualChild(child);

                Assert.IsFalse(root.IsMeasureValid);
                Assert.IsTrue(root.IsArrangeValid);

                scheduler.ProcessDueOperations();

                Assert.IsTrue(root.IsMeasureValid);
                Assert.IsTrue(root.IsArrangeValid);
            }
        }
    }
}
