using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class FrameworkElementTest
    {
        public class LayoutTestElement : FrameworkElement
        {
            public Size MeasureSize { get; set; }
            public Size ArrangeSize { get; set; }

            public Size LastAvailableSize { get; private set; }
            public Size LastFinalSize { get; private set; }

            protected override Size MeasureOverride(Size availableSize)
            {
                LastAvailableSize = availableSize;
                return MeasureSize;
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                LastFinalSize = finalSize;
                return ArrangeSize.DefaultIfNullOrEmpty(finalSize);
            }
        }

        [TestMethod]
        public void MeasureCoreTest()
        {
            LayoutTestElement element = new LayoutTestElement { MeasureSize = new Size(200, 100) };

            element.Measure(new Size(200, 100));
            Assert.AreEqual(new Size(200, 100), element.DesiredSize);

            element.Measure(Size.Infinity);
            Assert.AreEqual(new Size(200, 100), element.DesiredSize);

            element.Measure(Size.Zero);
            Assert.AreEqual(new Size(200, 100), element.DesiredSize);

            element.Margin = new Thickness(10, 20, 30, 40);
            element.Measure(new Size(200, 100));
            Assert.AreEqual(new Size(240, 160), element.DesiredSize);

            element.Measure(Size.Infinity);
            Assert.AreEqual(new Size(240, 160), element.DesiredSize);

            element.Measure(Size.Zero);
            Assert.AreEqual(new Size(240, 160), element.DesiredSize);

            element.Width = 400;
            element.Height = 300;
            element.Measure(new Size(200, 100));
            Assert.AreEqual(new Size(440, 360), element.DesiredSize);

            element.Measure(Size.Infinity);
            Assert.AreEqual(new Size(440, 360), element.DesiredSize);

            element.Measure(Size.Zero);
            Assert.AreEqual(new Size(440, 360), element.DesiredSize);
        }

        [TestMethod]
        public void ArrangeCoreTest()
        {
            LayoutTestElement element = new LayoutTestElement { MeasureSize = new Size(200, 100) };

            element.Arrange(new Rect(100, 50));
            Assert.AreEqual(new Size(100, 50), element.LastAvailableSize);
            Assert.AreEqual(new Size(100, 50), element.VisualSize);
            Assert.AreEqual(new Point(0, 0), element.VisualOffset);
            Assert.AreEqual(100, element.ActualWidth);
            Assert.AreEqual(50, element.ActualHeight);

            element.Width = 200;
            element.Height = 100;
            element.Arrange(new Rect(100, 50));
            Assert.AreEqual(new Size(200, 100), element.LastAvailableSize);
            Assert.AreEqual(new Size(200, 100), element.VisualSize);
            Assert.AreEqual(new Point(-50, -25), element.VisualOffset);
            Assert.AreEqual(200, element.ActualWidth);
            Assert.AreEqual(100, element.ActualHeight);

            element.Width = Double.NaN;
            element.Height = Double.NaN;
            element.Arrange(new Rect(200, 100));
            Assert.AreEqual(new Size(200, 100), element.LastAvailableSize);
            Assert.AreEqual(new Size(200, 100), element.VisualSize);
            Assert.AreEqual(new Point(0, 0), element.VisualOffset);
            Assert.AreEqual(200, element.ActualWidth);
            Assert.AreEqual(100, element.ActualHeight);

            element.HorizontalAlignment = HorizontalAlignment.Left;
            element.VerticalAlignment = VerticalAlignment.Top;
            element.Arrange(new Rect(300, 200));
            Assert.AreEqual(new Size(300, 200), element.LastAvailableSize);
            Assert.AreEqual(new Size(200, 100), element.VisualSize);
            Assert.AreEqual(new Point(0, 0), element.VisualOffset);
            Assert.AreEqual(200, element.ActualWidth);
            Assert.AreEqual(100, element.ActualHeight);

            element.HorizontalAlignment = HorizontalAlignment.Center;
            element.VerticalAlignment = VerticalAlignment.Bottom;
            element.Arrange(new Rect(300, 200));
            Assert.AreEqual(new Size(300, 200), element.LastAvailableSize);
            Assert.AreEqual(new Size(200, 100), element.VisualSize);
            Assert.AreEqual(new Point(50, 100), element.VisualOffset);
            Assert.AreEqual(200, element.ActualWidth);
            Assert.AreEqual(100, element.ActualHeight);

            element.HorizontalAlignment = HorizontalAlignment.Right;
            element.VerticalAlignment = VerticalAlignment.Center;
            element.Arrange(new Rect(300, 200));
            Assert.AreEqual(new Size(300, 200), element.LastAvailableSize);
            Assert.AreEqual(new Size(200, 100), element.VisualSize);
            Assert.AreEqual(new Point(100, 50), element.VisualOffset);
            Assert.AreEqual(200, element.ActualWidth);
            Assert.AreEqual(100, element.ActualHeight);

            element.HorizontalAlignment = HorizontalAlignment.Stretch;
            element.VerticalAlignment = VerticalAlignment.Stretch;
            element.Arrange(new Rect(300, 200));
            Assert.AreEqual(new Size(300, 200), element.LastAvailableSize);
            Assert.AreEqual(new Size(300, 200), element.VisualSize);
            Assert.AreEqual(new Point(0, 0), element.VisualOffset);
            Assert.AreEqual(300, element.ActualWidth);
            Assert.AreEqual(200, element.ActualHeight);

            element.ArrangeSize = new Size(200, 100);
            element.InvalidateArrange();

            element.HorizontalAlignment = HorizontalAlignment.Stretch;
            element.VerticalAlignment = VerticalAlignment.Stretch;
            element.Arrange(new Rect(300, 200));
            Assert.AreEqual(new Size(300, 200), element.LastAvailableSize);
            Assert.AreEqual(new Size(200, 100), element.VisualSize);
            Assert.AreEqual(new Point(50, 50), element.VisualOffset);
            Assert.AreEqual(200, element.ActualWidth);
            Assert.AreEqual(100, element.ActualHeight);
        }

        [TestMethod]
        public void ArrangeCoreMarginTest()
        {
            LayoutTestElement element = new LayoutTestElement { MeasureSize = new Size(200, 100), Margin = new Thickness(10, 20, 30, 40) };

            element.Arrange(new Rect(200, 100));
            Assert.AreEqual(new Size(160, 40), element.LastAvailableSize);
            Assert.AreEqual(new Size(160, 40), element.VisualSize);
            Assert.AreEqual(new Point(10, 20), element.VisualOffset);
            Assert.AreEqual(160, element.ActualWidth);
            Assert.AreEqual(40, element.ActualHeight);

            element.Width = 200;
            element.Height = 100;
            element.InvalidateArrange();
            element.Arrange(new Rect(200, 100));
            Assert.AreEqual(new Size(200, 100), element.LastAvailableSize);
            Assert.AreEqual(new Size(200, 100), element.VisualSize);
            Assert.AreEqual(new Point(-10, -10), element.VisualOffset);
            Assert.AreEqual(200, element.ActualWidth);
            Assert.AreEqual(100, element.ActualHeight);

            element.Width = Double.NaN;
            element.Height = Double.NaN;
            element.Arrange(new Rect(240, 160));
            Assert.AreEqual(new Size(200, 100), element.LastAvailableSize);
            Assert.AreEqual(new Size(200, 100), element.VisualSize);
            Assert.AreEqual(new Point(10, 20), element.VisualOffset);
            Assert.AreEqual(200, element.ActualWidth);
            Assert.AreEqual(100, element.ActualHeight);

            element.HorizontalAlignment = HorizontalAlignment.Left;
            element.VerticalAlignment = VerticalAlignment.Top;
            element.Arrange(new Rect(340, 260));
            Assert.AreEqual(new Size(300, 200), element.LastAvailableSize);
            Assert.AreEqual(new Size(200, 100), element.VisualSize);
            Assert.AreEqual(new Point(10, 20), element.VisualOffset);
            Assert.AreEqual(200, element.ActualWidth);
            Assert.AreEqual(100, element.ActualHeight);

            element.HorizontalAlignment = HorizontalAlignment.Center;
            element.VerticalAlignment = VerticalAlignment.Bottom;
            element.Arrange(new Rect(340, 260));
            Assert.AreEqual(new Size(300, 200), element.LastAvailableSize);
            Assert.AreEqual(new Size(200, 100), element.VisualSize);
            Assert.AreEqual(new Point(60, 120), element.VisualOffset);
            Assert.AreEqual(200, element.ActualWidth);
            Assert.AreEqual(100, element.ActualHeight);

            element.HorizontalAlignment = HorizontalAlignment.Right;
            element.VerticalAlignment = VerticalAlignment.Center;
            element.Arrange(new Rect(340, 260));
            Assert.AreEqual(new Size(300, 200), element.LastAvailableSize);
            Assert.AreEqual(new Size(200, 100), element.VisualSize);
            Assert.AreEqual(new Point(110, 70), element.VisualOffset);
            Assert.AreEqual(200, element.ActualWidth);
            Assert.AreEqual(100, element.ActualHeight);

            element.HorizontalAlignment = HorizontalAlignment.Stretch;
            element.VerticalAlignment = VerticalAlignment.Stretch;
            element.Arrange(new Rect(340, 260));
            Assert.AreEqual(new Size(300, 200), element.LastAvailableSize);
            Assert.AreEqual(new Size(300, 200), element.VisualSize);
            Assert.AreEqual(new Point(10, 20), element.VisualOffset);
            Assert.AreEqual(300, element.ActualWidth);
            Assert.AreEqual(200, element.ActualHeight);

            element.ArrangeSize = new Size(200, 100);
            element.InvalidateArrange();

            element.HorizontalAlignment = HorizontalAlignment.Stretch;
            element.VerticalAlignment = VerticalAlignment.Stretch;
            element.Arrange(new Rect(340, 260));
            Assert.AreEqual(new Size(300, 200), element.LastAvailableSize);
            Assert.AreEqual(new Size(200, 100), element.VisualSize);
            Assert.AreEqual(new Point(60, 70), element.VisualOffset);
            Assert.AreEqual(200, element.ActualWidth);
            Assert.AreEqual(100, element.ActualHeight);
        }

        [TestMethod]
        public void ResourceInheritanceTest()
        {
            object value;

            FrameworkElement root = new FrameworkElement();
            root.Resources = new ResourceDictionary();
            root.Resources.Add("key1", "value1");

            Assert.IsTrue(root.TryGetResource("key1", out value));
            Assert.AreEqual("value1", value);

            FrameworkElement child1 = new FrameworkElement();
            root.AddVisualChild(child1);
            root.AddLogicalChild(child1);

            Assert.IsTrue(child1.TryGetResource("key1", out value));
            Assert.AreEqual("value1", value);

            child1.Resources = new ResourceDictionary();
            child1.Resources.Add("key1", "value2");

            FrameworkElement child2 = new FrameworkElement();
            child1.AddVisualChild(child2);

            Assert.IsTrue(child2.TryGetResource("key1", out value));
            Assert.AreEqual("value2", value);

            root.AddLogicalChild(child2);

            Assert.IsTrue(child2.TryGetResource("key1", out value));
            Assert.AreEqual("value1", value);
        }
    }
}
