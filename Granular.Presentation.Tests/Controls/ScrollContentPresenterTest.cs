using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Controls
{
    [TestClass]
    public class ScrollContentPresenterTest
    {
        private class ScrollContentPresenterTestElement : FrameworkElement, IScrollInfo
        {
            public Size ExtentSize { get; set; }
            public Size ViewportSize { get; set; }

            public bool CanHorizontallyScroll { get; set; }
            public bool CanVerticallyScroll { get; set; }
            public Point Offset { get; set; }

            public Size MeasureSize { get; set; }

            public Size LastAvailableSize1 { get; private set; }
            public Size LastAvailableSize2 { get; private set; }

            protected override Size MeasureOverride(Size availableSize)
            {
                LastAvailableSize2 = LastAvailableSize1;
                LastAvailableSize1 = availableSize;
                return MeasureSize;
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                return MeasureSize;
            }
        }

        [TestMethod]
        public void ScrollContentPresenterBasicTest()
        {
            Border border = new Border();
            ScrollContentPresenter scrollContentPresenter = new ScrollContentPresenter { Width = 200, Height = 100, Content = border, IsRootElement = true };

            border.Width = 10;
            border.Height = 10;
            Assert.AreEqual(border.VisualSize, scrollContentPresenter.ExtentSize);

            border.Width = 400;
            Assert.AreEqual(border.VisualSize, scrollContentPresenter.ExtentSize);

            scrollContentPresenter.Offset = new Point(100, 0);
            Assert.AreEqual(new Point(-100, 45), border.VisualOffset);

            border.Width = 10;
            Assert.AreEqual(border.VisualSize, scrollContentPresenter.ExtentSize);
            Assert.AreEqual(new Point(95, 45), border.VisualOffset);
        }

        [TestMethod]
        public void ScrollContentPresenterContentScrollTest()
        {
            ScrollContentPresenterTestElement content = new ScrollContentPresenterTestElement { MeasureSize = new Size(100, 50), ExtentSize = new Size(400, 200) };
            ScrollContentPresenter scrollContentPresenter = new ScrollContentPresenter { Width = 200, Height = 100, IsRootElement = true };
            content.ViewportSize = scrollContentPresenter.RenderSize;

            scrollContentPresenter.Content = content;
            Assert.AreEqual(new Size(100, 50), scrollContentPresenter.ExtentSize);

            scrollContentPresenter.CanContentScroll = true;
            Assert.AreEqual(new Size(400, 200), scrollContentPresenter.ExtentSize);
            Assert.AreEqual(new Size(200, 100), content.LastAvailableSize1);
            Assert.IsFalse(content.CanHorizontallyScroll);
            Assert.IsFalse(content.CanVerticallyScroll);

            scrollContentPresenter.CanHorizontallyScroll = true;
            Assert.AreEqual(new Size(Double.PositiveInfinity, 100), content.LastAvailableSize2);
            Assert.IsTrue(content.CanHorizontallyScroll);
            Assert.IsFalse(content.CanVerticallyScroll);

            scrollContentPresenter.CanVerticallyScroll = true;
            Assert.AreEqual(new Size(Double.PositiveInfinity, Double.PositiveInfinity), content.LastAvailableSize2);
            Assert.IsTrue(content.CanHorizontallyScroll);
            Assert.IsTrue(content.CanVerticallyScroll);
        }
    }
}
