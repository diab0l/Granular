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
    public class ContentPresenterTestData
    {
        public double Value { get; set; }
    }

    [TestClass]
    public class ContentPresenterTest
    {

        [TestMethod]
        public void ContentPresenterBasicTest()
        {
            string text = @"
            <ContentPresenter xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:test='clr-namespace:Granular.Presentation.Tests.Controls;assembly=Granular.Presentation.Tests'>
                <ContentPresenter.Resources>
                    <DataTemplate DataType='{x:Type test:ContentPresenterTestData}'>
                        <FrameworkElement Width='{Binding Value}'/>
                    </DataTemplate>
                </ContentPresenter.Resources>
            </ContentPresenter>";

            XamlElement rootElement = XamlParser.Parse(text);
            ContentPresenter contentPresenter = XamlLoader.Load(rootElement) as ContentPresenter;

            ContentPresenterTestData data = new ContentPresenterTestData { Value = 100 };

            Assert.AreEqual(0, contentPresenter.VisualChildren.Count());

            contentPresenter.Content = data;

            Assert.AreEqual(1, contentPresenter.VisualChildren.Count());

            FrameworkElement templateChild = contentPresenter.VisualChildren.First() as FrameworkElement;

            Assert.IsNotNull(templateChild);
            Assert.AreEqual(data, templateChild.DataContext);
            Assert.AreEqual(100, templateChild.Width);

            contentPresenter.Content = null;

            Assert.AreEqual(0, contentPresenter.VisualChildren.Count());

            object content = new object();
            contentPresenter.Content = content;

            Assert.AreEqual(1, contentPresenter.VisualChildren.Count());

            TextBlock templateChild2 = contentPresenter.VisualChildren.First() as TextBlock;

            Assert.IsNotNull(templateChild2);
            Assert.AreEqual(content.ToString(), templateChild2.Text);
        }
    }
}
