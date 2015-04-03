using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Controls
{
    [TestClass]
    public class CanvasTest
    {
        [TestMethod]
        public void CanvasLayoutTest()
        {
            Canvas panel = new Canvas();

            FrameworkElement child1 = new FrameworkElement { Width = 200, Height = 100 };
            FrameworkElement child2 = new FrameworkElement { Width = 200, Height = 100 };
            FrameworkElement child3 = new FrameworkElement { Width = 200, Height = 100 };
            FrameworkElement child4 = new FrameworkElement { Width = 200, Height = 100 };

            Canvas.SetLeft(child1, 20);
            Canvas.SetTop(child1, 10);
            Canvas.SetRight(child2, 20);
            Canvas.SetTop(child2, 10);
            Canvas.SetRight(child3, 20);
            Canvas.SetBottom(child3, 10);
            Canvas.SetLeft(child4, 20);
            Canvas.SetBottom(child4, 10);

            panel.Children.Add(child1);
            panel.Children.Add(child2);
            panel.Children.Add(child3);
            panel.Children.Add(child4);

            panel.Measure(new Size(1000, 1000));

            Assert.AreEqual(Size.Zero, panel.DesiredSize);

            panel.Arrange(new Rect(1000, 1000));

            Assert.AreEqual(new Size(1000, 1000), panel.VisualSize);

            Assert.AreEqual(new Size(200, 100), child1.VisualSize);
            Assert.AreEqual(new Size(200, 100), child2.VisualSize);
            Assert.AreEqual(new Size(200, 100), child3.VisualSize);
            Assert.AreEqual(new Size(200, 100), child4.VisualSize);

            Assert.AreEqual(new Point(20, 10), child1.VisualOffset);
            Assert.AreEqual(new Point(780, 10), child2.VisualOffset);
            Assert.AreEqual(new Point(780, 890), child3.VisualOffset);
            Assert.AreEqual(new Point(20, 890), child4.VisualOffset);
        }

        [TestMethod]
        public void CanvasLayoutParseTest()
        {
            string text = @"
            <Canvas xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
                <FrameworkElement Width='200' Height='100' Canvas.Left='20' Canvas.Top='10'/>
                <FrameworkElement Width='200' Height='100' Canvas.Right='20' Canvas.Top='10'/>
                <FrameworkElement Width='200' Height='100' Canvas.Right='20' Canvas.Bottom='10'/>
                <FrameworkElement Width='200' Height='100' Canvas.Left='20' Canvas.Bottom='10'/>
            </Canvas>";

            XamlElement rootElement = XamlParser.Parse(text);
            Canvas panel = XamlLoader.Load(rootElement) as Canvas;

            panel.Measure(new Size(1000, 1000));

            Assert.AreEqual(Size.Zero, panel.DesiredSize);

            panel.Arrange(new Rect(1000, 1000));

            Assert.AreEqual(new Size(1000, 1000), panel.VisualSize);

            Assert.AreEqual(new Size(200, 100), panel.Children[0].VisualSize);
            Assert.AreEqual(new Size(200, 100), panel.Children[1].VisualSize);
            Assert.AreEqual(new Size(200, 100), panel.Children[2].VisualSize);
            Assert.AreEqual(new Size(200, 100), panel.Children[3].VisualSize);

            Assert.AreEqual(new Point(20, 10), panel.Children[0].VisualOffset);
            Assert.AreEqual(new Point(780, 10), panel.Children[1].VisualOffset);
            Assert.AreEqual(new Point(780, 890), panel.Children[2].VisualOffset);
            Assert.AreEqual(new Point(20, 890), panel.Children[3].VisualOffset);
        }
    }
}
