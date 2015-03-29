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
    public class StackPanelTest
    {
        [TestMethod]
        public void StackPanelLayoutTest()
        {
            StackPanel panel = new StackPanel();

            FrameworkElement child1 = new FrameworkElement { Height = 100 };
            FrameworkElement child2 = new FrameworkElement { Height = 100 };
            FrameworkElement child3 = new FrameworkElement { Height = 100 };

            panel.Children.Add(child1);
            panel.Children.Add(child2);
            panel.Children.Add(child3);

            panel.Measure(new Size(1000, 1000));

            Assert.AreEqual(new Size(0, 300), panel.DesiredSize);

            panel.Arrange(new Rect(0, 0, 1000, 300));

            Assert.AreEqual(new Size(1000, 300), panel.VisualSize);

            Assert.AreEqual(new Size(1000, 100), child1.VisualSize);
            Assert.AreEqual(new Size(1000, 100), child2.VisualSize);
            Assert.AreEqual(new Size(1000, 100), child3.VisualSize);

            Assert.AreEqual(new Point(0, 0), child1.VisualOffset);
            Assert.AreEqual(new Point(0, 100), child2.VisualOffset);
            Assert.AreEqual(new Point(0, 200), child3.VisualOffset);
        }

        [TestMethod]
        public void StackPanelMarginTest()
        {
            StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };

            FrameworkElement child1 = new FrameworkElement { Width = 80, Margin = new Thickness(10) };
            FrameworkElement child2 = new FrameworkElement { Width = 60, Margin = new Thickness(20) };
            FrameworkElement child3 = new FrameworkElement { Width = 40, Margin = new Thickness(30) };

            panel.Children.Add(child1);
            panel.Children.Add(child2);
            panel.Children.Add(child3);

            panel.Measure(new Size(1000, 1000));

            Assert.AreEqual(new Size(300, 60), panel.DesiredSize);

            panel.Arrange(new Rect(0, 0, 300, 1000));

            Assert.AreEqual(new Size(300, 1000), panel.VisualSize);

            Assert.AreEqual(new Size(80, 980), child1.VisualSize);
            Assert.AreEqual(new Size(60, 960), child2.VisualSize);
            Assert.AreEqual(new Size(40, 940), child3.VisualSize);

            Assert.AreEqual(new Point(10, 10), child1.VisualOffset);
            Assert.AreEqual(new Point(120, 20), child2.VisualOffset);
            Assert.AreEqual(new Point(230, 30), child3.VisualOffset);
        }

        [TestMethod]
        public void StackPanelLayoutParseTest()
        {
            string text = @"
            <StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
                <FrameworkElement Height='100'/>
                <FrameworkElement Height='100'/>
                <FrameworkElement Height='100'/>
            </StackPanel>";

            XamlElement rootElement = XamlParser.Parse(text);
            StackPanel panel = XamlLoader.Load(rootElement) as StackPanel;

            panel.Measure(new Size(1000, 1000));

            Assert.AreEqual(new Size(0, 300), panel.DesiredSize);

            panel.Arrange(new Rect(0, 0, 1000, 300));

            Assert.AreEqual(new Size(1000, 300), panel.VisualSize);

            Assert.AreEqual(new Size(1000, 100), panel.Children[0].VisualSize);
            Assert.AreEqual(new Size(1000, 100), panel.Children[1].VisualSize);
            Assert.AreEqual(new Size(1000, 100), panel.Children[2].VisualSize);

            Assert.AreEqual(new Point(0, 0), panel.Children[0].VisualOffset);
            Assert.AreEqual(new Point(0, 100), panel.Children[1].VisualOffset);
            Assert.AreEqual(new Point(0, 200), panel.Children[2].VisualOffset);
        }
    }
}
