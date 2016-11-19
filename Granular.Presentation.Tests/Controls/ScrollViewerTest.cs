using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Controls
{
    [TestClass]
    public class ScrollViewerTest
    {
        [TestMethod]
        public void ScrollViewerComputedVisibilityTest()
        {
            string text = @"
<ScrollViewer xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
              Width='200' Height='100' HorizontalScrollBarVisibility='Auto' VerticalScrollBarVisibility='Auto'>
    <ScrollViewer.Template>
        <ControlTemplate TargetType='{x:Type ScrollViewer}'>
            <ScrollContentPresenter x:Name='PART_ScrollContentPresenter' Content='{TemplateBinding Content}'/>
        </ControlTemplate>
    </ScrollViewer.Template>

    <Border x:Name='border'/>
</ScrollViewer>";

            ScrollViewer scrollViewer = XamlLoader.Load(XamlParser.Parse(text)) as ScrollViewer;
            scrollViewer.IsRootElement = true;

            Border border = NameScope.GetNameScope(scrollViewer).FindName("border") as Border;

            border.Width = 10;
            border.Height = 10;

            Assert.AreEqual(Visibility.Collapsed, scrollViewer.ComputedHorizontalScrollBarVisibility);
            Assert.AreEqual(Visibility.Collapsed, scrollViewer.ComputedVerticalScrollBarVisibility);
            Assert.AreEqual(border.Width, scrollViewer.ExtentWidth);
            Assert.AreEqual(border.Height, scrollViewer.ExtentHeight);

            border.Width = 1000;
            border.Height = 1000;

            Assert.AreEqual(Visibility.Visible, scrollViewer.ComputedHorizontalScrollBarVisibility);
            Assert.AreEqual(Visibility.Visible, scrollViewer.ComputedVerticalScrollBarVisibility);
            Assert.AreEqual(border.Width, scrollViewer.ExtentWidth);
            Assert.AreEqual(border.Height, scrollViewer.ExtentHeight);

            border.Width = 10;
            border.Height = 1000;

            Assert.AreEqual(Visibility.Collapsed, scrollViewer.ComputedHorizontalScrollBarVisibility);
            Assert.AreEqual(Visibility.Visible, scrollViewer.ComputedVerticalScrollBarVisibility);
            Assert.AreEqual(border.Width, scrollViewer.ExtentWidth);
            Assert.AreEqual(border.Height, scrollViewer.ExtentHeight);

            border.Width = 1000;
            border.Height = 10;

            Assert.AreEqual(Visibility.Visible, scrollViewer.ComputedHorizontalScrollBarVisibility);
            Assert.AreEqual(Visibility.Collapsed, scrollViewer.ComputedVerticalScrollBarVisibility);
            Assert.AreEqual(border.Width, scrollViewer.ExtentWidth);
            Assert.AreEqual(border.Height, scrollViewer.ExtentHeight);
        }
    }
}