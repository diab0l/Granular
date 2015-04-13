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
    public class WrapPanelTest
    {
        [TestMethod]
        public void WrapPanelLayoutTest()
        {
            WrapPanel panel = new WrapPanel();
            panel.IsRootElement = true;

            FrameworkElement child1 = new FrameworkElement { Width = 200, Height = 100 };
            FrameworkElement child2 = new FrameworkElement { Width = 200, Height = 100 };
            FrameworkElement child3 = new FrameworkElement { Width = 200, Height = 60, Margin = new Thickness(0, 10) };
            FrameworkElement child4 = new FrameworkElement { Width = 200, Height = 60, Margin = new Thickness(0, 10) };
            FrameworkElement child5 = new FrameworkElement { Width = 200, Height = 60 };

            panel.Children.Add(child1);
            panel.Children.Add(child2);
            panel.Children.Add(child3);
            panel.Children.Add(child4);
            panel.Children.Add(child5);

            panel.Measure(new Size(1000, 1000));

            Assert.AreEqual(new Size(1000, 100), panel.DesiredSize);

            panel.Arrange(new Rect(1000, 100));

            Assert.AreEqual(new Size(1000, 100), panel.RenderSize);

            Assert.AreEqual(new Point(0, 0), child1.VisualOffset);
            Assert.AreEqual(new Point(200, 0), child2.VisualOffset);
            Assert.AreEqual(new Point(400, 20), child3.VisualOffset);
            Assert.AreEqual(new Point(600, 20), child4.VisualOffset);
            Assert.AreEqual(new Point(800, 20), child5.VisualOffset);

            panel.Measure(new Size(500, 1000));

            Assert.AreEqual(new Size(400, 240), panel.DesiredSize);

            panel.Arrange(new Rect(500, 240));

            Assert.AreEqual(new Point(0, 0), child1.VisualOffset);
            Assert.AreEqual(new Point(200, 0), child2.VisualOffset);
            Assert.AreEqual(new Point(0, 110), child3.VisualOffset);
            Assert.AreEqual(new Point(200, 110), child4.VisualOffset);
            Assert.AreEqual(new Point(0, 180), child5.VisualOffset);
        }

        [TestMethod]
        public void WrapPanelLayoutParseTest()
        {
            string text = @"
            <WrapPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
                <FrameworkElement Width='200' Height='100'/>
                <FrameworkElement Width='200' Height='100'/>
                <FrameworkElement Width='200' Height='60' Margin='0,10'/>
                <FrameworkElement Width='200' Height='60' Margin='0,10'/>
                <FrameworkElement Width='200' Height='60'/>
            </WrapPanel>";

            XamlElement rootElement = XamlParser.Parse(text);
            WrapPanel panel = XamlLoader.Load(rootElement) as WrapPanel;
            panel.IsRootElement = true;

            panel.Measure(new Size(1000, 1000));

            Assert.AreEqual(new Size(1000, 100), panel.DesiredSize);

            panel.Arrange(new Rect(1000, 100));

            Assert.AreEqual(new Size(1000, 100), panel.RenderSize);

            Assert.AreEqual(new Point(0, 0), panel.Children[0].VisualOffset);
            Assert.AreEqual(new Point(200, 0), panel.Children[1].VisualOffset);
            Assert.AreEqual(new Point(400, 20), panel.Children[2].VisualOffset);
            Assert.AreEqual(new Point(600, 20), panel.Children[3].VisualOffset);
            Assert.AreEqual(new Point(800, 20), panel.Children[4].VisualOffset);

            panel.Measure(new Size(500, 1000));

            Assert.AreEqual(new Size(400, 240), panel.DesiredSize);

            panel.Arrange(new Rect(500, 240));

            Assert.AreEqual(new Point(0, 0), panel.Children[0].VisualOffset);
            Assert.AreEqual(new Point(200, 0), panel.Children[1].VisualOffset);
            Assert.AreEqual(new Point(0, 110), panel.Children[2].VisualOffset);
            Assert.AreEqual(new Point(200, 110), panel.Children[3].VisualOffset);
            Assert.AreEqual(new Point(0, 180), panel.Children[4].VisualOffset);
        }
    }
}
