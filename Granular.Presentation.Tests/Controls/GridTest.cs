using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xaml;
using Granular.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Controls
{
    [TestClass]
    public class GridTest
    {
        [TestMethod]
        public void GridLayoutTest()
        {
            //  1* |    2*   |      3*
            // [c1]|[--c2---]|[-----c3-----]
            // [-----c4-----]|
            //     |[----------c5----------]

            Grid panel = new Grid();

            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.FromStars(1) });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.FromStars(2) });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.FromStars(3) });

            panel.RowDefinitions.Add(new RowDefinition());
            panel.RowDefinitions.Add(new RowDefinition());
            panel.RowDefinitions.Add(new RowDefinition());

            FrameworkElement child1 = new FrameworkElement { Height = 100 };
            FrameworkElement child2 = new FrameworkElement { Height = 100 };
            FrameworkElement child3 = new FrameworkElement { Height = 100 };
            FrameworkElement child4 = new FrameworkElement { Height = 100 };
            FrameworkElement child5 = new FrameworkElement { Height = 100 };

            Grid.SetColumn(child1, 0);
            Grid.SetColumn(child2, 1);
            Grid.SetColumn(child3, 2);
            Grid.SetColumn(child4, 0);
            Grid.SetColumn(child5, 1);
            Grid.SetRow(child4, 1);
            Grid.SetRow(child5, 2);
            Grid.SetColumnSpan(child4, 2);
            Grid.SetColumnSpan(child5, 2);

            panel.Children.Add(child1);
            panel.Children.Add(child2);
            panel.Children.Add(child3);
            panel.Children.Add(child4);
            panel.Children.Add(child5);

            panel.Measure(new Size(600, 600));

            Assert.AreEqual(new Size(0, 300), panel.DesiredSize);

            panel.Arrange(new Rect(600, 300));

            Assert.AreEqual(new Size(600, 300), panel.VisualSize);

            Assert.AreEqual(new Size(100, 100), child1.VisualSize);
            Assert.AreEqual(new Size(200, 100), child2.VisualSize);
            Assert.AreEqual(new Size(300, 100), child3.VisualSize);
            Assert.AreEqual(new Size(300, 100), child4.VisualSize);
            Assert.AreEqual(new Size(500, 100), child5.VisualSize);

            Assert.AreEqual(new Point(0, 0), child1.VisualOffset);
            Assert.AreEqual(new Point(100, 0), child2.VisualOffset);
            Assert.AreEqual(new Point(300, 0), child3.VisualOffset);
            Assert.AreEqual(new Point(0, 100), child4.VisualOffset);
            Assert.AreEqual(new Point(100, 200), child5.VisualOffset);
        }

        [TestMethod]
        public void GridLayoutParseTest()
        {
            string text = @"
            <Grid xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width='*'/>
                    <ColumnDefinition Width='2*'/>
                    <ColumnDefinition Width='3*'/>
                </Grid.ColumnDefinitions>
                <Grid.RowDefinitions>
                    <RowDefinition/>
                    <RowDefinition/>
                    <RowDefinition/>
                </Grid.RowDefinitions>
                <FrameworkElement Height='100'/>
                <FrameworkElement Height='100' Grid.Column='1'/>
                <FrameworkElement Height='100' Grid.Column='2'/>
                <FrameworkElement Height='100' Grid.Row='1' Grid.ColumnSpan='2'/>
                <FrameworkElement Height='100' Grid.Row='2' Grid.Column='1' Grid.ColumnSpan='2'/>
            </Grid>";

            XamlElement rootElement = XamlParser.Parse(text);
            Grid panel = XamlLoader.Load(rootElement) as Grid;

            panel.Measure(new Size(600, 600));

            Assert.AreEqual(new Size(0, 300), panel.DesiredSize);

            panel.Arrange(new Rect(600, 300));

            Assert.AreEqual(new Size(600, 300), panel.VisualSize);

            Assert.AreEqual(new Size(100, 100), panel.Children[0].VisualSize);
            Assert.AreEqual(new Size(200, 100), panel.Children[1].VisualSize);
            Assert.AreEqual(new Size(300, 100), panel.Children[2].VisualSize);
            Assert.AreEqual(new Size(300, 100), panel.Children[3].VisualSize);
            Assert.AreEqual(new Size(500, 100), panel.Children[4].VisualSize);

            Assert.AreEqual(new Point(0, 0), panel.Children[0].VisualOffset);
            Assert.AreEqual(new Point(100, 0), panel.Children[1].VisualOffset);
            Assert.AreEqual(new Point(300, 0), panel.Children[2].VisualOffset);
            Assert.AreEqual(new Point(0, 100), panel.Children[3].VisualOffset);
            Assert.AreEqual(new Point(100, 200), panel.Children[4].VisualOffset);

        }

        [TestMethod]
        public void GridStarsTest()
        {
            Grid panel = new Grid();

            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.FromStars(1) });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.FromStars(2), MaxWidth = 200 });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.FromStars(3), MaxWidth = 100 });

            panel.RowDefinitions.Add(new RowDefinition());

            panel.Arrange(new Rect(60, 10));
            Assert.IsTrue(panel.ColumnDefinitions[0].ActualWidth.IsClose(10));
            Assert.IsTrue(panel.ColumnDefinitions[1].ActualWidth.IsClose(20));
            Assert.IsTrue(panel.ColumnDefinitions[2].ActualWidth.IsClose(30));
            Assert.IsTrue(panel.ColumnDefinitions.Sum(column => column.ActualWidth).IsClose(60));

            panel.Arrange(new Rect(200, 10));
            Assert.IsTrue(panel.ColumnDefinitions[0].ActualWidth.IsClose(200.0 / 6));
            Assert.IsTrue(panel.ColumnDefinitions[1].ActualWidth.IsClose(200.0 / 3));
            Assert.IsTrue(panel.ColumnDefinitions[2].ActualWidth.IsClose(100));
            Assert.IsTrue(panel.ColumnDefinitions.Sum(column => column.ActualWidth).IsClose(200));

            panel.Arrange(new Rect(400, 10));
            Assert.IsTrue(panel.ColumnDefinitions[0].ActualWidth.IsClose(100));
            Assert.IsTrue(panel.ColumnDefinitions[1].ActualWidth.IsClose(200));
            Assert.IsTrue(panel.ColumnDefinitions[2].ActualWidth.IsClose(100));
            Assert.IsTrue(panel.ColumnDefinitions.Sum(column => column.ActualWidth).IsClose(400));

            panel.Arrange(new Rect(600, 10));
            Assert.IsTrue(panel.ColumnDefinitions[0].ActualWidth.IsClose(300));
            Assert.IsTrue(panel.ColumnDefinitions[1].ActualWidth.IsClose(200));
            Assert.IsTrue(panel.ColumnDefinitions[2].ActualWidth.IsClose(100));
            Assert.IsTrue(panel.ColumnDefinitions.Sum(column => column.ActualWidth).IsClose(600));
        }
    }
}
