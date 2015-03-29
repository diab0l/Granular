using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Controls
{
    [TestClass]
    public class BorderTest
    {
        [TestMethod]
        public void BorderRenderTest()
        {
            Brush backgroundBrush = new LinearGradientBrush(30, Colors.Blue, Colors.Green);
            Brush borderBrush = new LinearGradientBrush(-30, Colors.Red, Colors.Blue);
            Thickness borderThickness = new Thickness(10, 20, 30, 40);

            Border border = new Border
            {
                Child = new FrameworkElement { Width = 200, Height = 100 },
                Background = backgroundBrush,
                BorderBrush = borderBrush,
                BorderThickness = borderThickness,
            };

            border.Measure(Size.Infinity);
            border.Arrange(new Rect(border.DesiredSize));

            IRenderElementFactory factory = TestRenderElementFactory.Default;

            border.GetRenderElement(factory);

            Assert.AreEqual(new Rect(0, 0, 240, 160), border.GetRenderElement(factory).Bounds);

            Assert.AreEqual(2, border.GetRenderElement(factory).Children.Count());

            IBorderRenderElement borderRenderElement = border.GetRenderElement(factory).Children.ElementAt(0) as IBorderRenderElement;
            Assert.IsNotNull(borderRenderElement);
            Assert.AreEqual(backgroundBrush, borderRenderElement.Background);
            Assert.AreEqual(borderBrush, borderRenderElement.BorderBrush);
            Assert.AreEqual(borderThickness, borderRenderElement.BorderThickness);
            Assert.AreEqual(new Rect(0, 0, 240, 160), borderRenderElement.Bounds);

            IVisualRenderElement visualRenderElement = border.GetRenderElement(factory).Children.ElementAt(1) as IVisualRenderElement;
            Assert.IsNotNull(visualRenderElement);
            Assert.AreEqual(new Rect(10, 20, 200, 100), visualRenderElement.Bounds);

            border.Child = null;
            Assert.AreEqual(1, border.GetRenderElement(factory).Children.Count());
        }

        [TestMethod]
        public void BorderHitTestTest()
        {
            Border border = new Border
            {
                Width = 200,
                Height = 100,
                BorderThickness = new Thickness(10, 20, 30, 40),
                IsRootElement = true,
            };

            border.Measure(Size.Infinity);
            border.Arrange(new Rect(border.DesiredSize));

            BackgroundHitTestTest(border, null);
            BorderHitTestTest(border, null);

            border.Background = Brushes.Transparent;
            border.BorderBrush = null;

            BackgroundHitTestTest(border, border);
            BorderHitTestTest(border, border);

            border.Background = null;
            border.BorderBrush = Brushes.Transparent;

            BackgroundHitTestTest(border, null);
            BorderHitTestTest(border, border);

            border.Background = Brushes.Transparent;
            border.BorderBrush = Brushes.Transparent;

            BackgroundHitTestTest(border, border);
            BorderHitTestTest(border, border);
        }

        private static void BackgroundHitTestTest(Border border, Visual hitTestResult)
        {
            Assert.AreEqual(hitTestResult, border.HitTest(new Point(border.BorderThickness.Left + 5, border.BorderThickness.Top + 5)));
            Assert.AreEqual(hitTestResult, border.HitTest(new Point(border.Width - border.BorderThickness.Right - 5, border.BorderThickness.Top + 5)));
            Assert.AreEqual(hitTestResult, border.HitTest(new Point(border.Width - border.BorderThickness.Right - 5, border.Height - border.BorderThickness.Bottom - 5)));
            Assert.AreEqual(hitTestResult, border.HitTest(new Point(border.BorderThickness.Left + 5, border.Height - border.BorderThickness.Bottom - 5)));
        }

        private static void BorderHitTestTest(Border border, Visual hitTestResult)
        {
            Assert.AreEqual(hitTestResult, border.HitTest(new Point(border.BorderThickness.Left / 2, border.BorderThickness.Top / 2)));
            Assert.AreEqual(hitTestResult, border.HitTest(new Point(border.Width - border.BorderThickness.Right / 2, border.BorderThickness.Top / 2)));
            Assert.AreEqual(hitTestResult, border.HitTest(new Point(border.Width - border.BorderThickness.Right / 2, border.Height - border.BorderThickness.Bottom / 2)));
            Assert.AreEqual(hitTestResult, border.HitTest(new Point(border.BorderThickness.Left / 2, border.Height - border.BorderThickness.Bottom / 2)));
        }
    }
}
