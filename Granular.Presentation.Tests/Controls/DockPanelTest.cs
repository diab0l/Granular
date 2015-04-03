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
    public class DockPanelTest
    {
        [TestMethod]
        public void DockPanelLayoutTest()
        {
            DockPanel panel = new DockPanel();

            FrameworkElement child1 = new FrameworkElement { Width = 100 };
            FrameworkElement child2 = new FrameworkElement { Height = 100 };
            FrameworkElement child3 = new FrameworkElement { Width = 100 };
            FrameworkElement child4 = new FrameworkElement { Height = 100 };
            FrameworkElement child5 = new FrameworkElement();

            panel.Children.Add(child1);
            panel.Children.Add(child2);
            panel.Children.Add(child3);
            panel.Children.Add(child4);
            panel.Children.Add(child5);

            DockPanel.SetDock(child1, Dock.Left);
            DockPanel.SetDock(child2, Dock.Top);
            DockPanel.SetDock(child3, Dock.Right);
            DockPanel.SetDock(child4, Dock.Bottom);

            panel.Measure(new Size(1000, 1000));

            Assert.AreEqual(new Size(200, 200), panel.DesiredSize);

            panel.Arrange(new Rect(1000, 1000));

            Assert.AreEqual(new Size(1000, 1000), panel.VisualSize);

            Assert.AreEqual(new Size(100, 1000), child1.VisualSize);
            Assert.AreEqual(new Size(900, 100), child2.VisualSize);
            Assert.AreEqual(new Size(100, 900), child3.VisualSize);
            Assert.AreEqual(new Size(800, 100), child4.VisualSize);
            Assert.AreEqual(new Size(800, 800), child5.VisualSize);

            Assert.AreEqual(new Point(0, 0), child1.VisualOffset);
            Assert.AreEqual(new Point(100, 0), child2.VisualOffset);
            Assert.AreEqual(new Point(900, 100), child3.VisualOffset);
            Assert.AreEqual(new Point(100, 900), child4.VisualOffset);
            Assert.AreEqual(new Point(100, 100), child5.VisualOffset);
        }

        [TestMethod]
        public void DockPanelLayoutParseTest()
        {
            string text = @"
            <DockPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
                <FrameworkElement Width='100' DockPanel.Dock='Left'/>
                <FrameworkElement Height='100' DockPanel.Dock='Top'/>
                <FrameworkElement Width='100' DockPanel.Dock='Right'/>
                <FrameworkElement Height='100' DockPanel.Dock='Bottom'/>
                <FrameworkElement/>
            </DockPanel>";

            XamlElement rootElement = XamlParser.Parse(text);
            DockPanel panel = XamlLoader.Load(rootElement) as DockPanel;

            panel.Measure(new Size(1000, 1000));

            Assert.AreEqual(new Size(200, 200), panel.DesiredSize);

            panel.Arrange(new Rect(1000, 1000));

            Assert.AreEqual(new Size(1000, 1000), panel.VisualSize);

            Assert.AreEqual(new Size(100, 1000), panel.Children[0].VisualSize);
            Assert.AreEqual(new Size(900, 100), panel.Children[1].VisualSize);
            Assert.AreEqual(new Size(100, 900), panel.Children[2].VisualSize);
            Assert.AreEqual(new Size(800, 100), panel.Children[3].VisualSize);
            Assert.AreEqual(new Size(800, 800), panel.Children[4].VisualSize);

            Assert.AreEqual(new Point(0, 0), panel.Children[0].VisualOffset);
            Assert.AreEqual(new Point(100, 0), panel.Children[1].VisualOffset);
            Assert.AreEqual(new Point(900, 100), panel.Children[2].VisualOffset);
            Assert.AreEqual(new Point(100, 900), panel.Children[3].VisualOffset);
            Assert.AreEqual(new Point(100, 100), panel.Children[4].VisualOffset);
        }
    }
}
