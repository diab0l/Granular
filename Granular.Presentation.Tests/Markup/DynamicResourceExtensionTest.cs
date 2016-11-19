using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests
{
    public class ResourceTestObject
    {
        public static readonly DependencyProperty TestValueProperty = DependencyProperty.RegisterAttached("TestValue", typeof(ResourceTestObject), typeof(ResourceTestObject), new FrameworkPropertyMetadata());

        public double Value { get; set; }
        public ResourceTestObject Child { get; set; }
    }

    [TestClass]
    public class DynamicResourceExtensionTest
    {
        [TestMethod]
        public void DynamicResourceExtensionBaseTest()
        {
            string text = @"
            <StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:test='clr-namespace:Granular.Presentation.Tests;assembly=Granular.Presentation.Tests' x:Name='panel1'>
                <StackPanel.Resources>
                    <test:ResourceTestObject x:Key='key1' Value='1'/>
                </StackPanel.Resources>

                <FrameworkElement x:Name='child1' test:ResourceTestObject.TestValue='{DynamicResource key1}'/>

                <StackPanel x:Name='panel2'>
                    <FrameworkElement x:Name='child2' test:ResourceTestObject.TestValue='{DynamicResource key1}'/>
                </StackPanel>
            </StackPanel>";

            FrameworkElement panel1 = XamlLoader.Load(XamlParser.Parse(text)) as FrameworkElement;
            FrameworkElement panel2 = NameScope.GetNameScope(panel1).FindName("panel2") as FrameworkElement;
            FrameworkElement child1 = NameScope.GetNameScope(panel1).FindName("child1") as FrameworkElement;
            FrameworkElement child2 = NameScope.GetNameScope(panel1).FindName("child2") as FrameworkElement;

            Assert.AreEqual(1, (child1.GetValue(ResourceTestObject.TestValueProperty) as ResourceTestObject).Value);
            Assert.AreEqual(1, (child2.GetValue(ResourceTestObject.TestValueProperty) as ResourceTestObject).Value);

            panel2.Resources = new ResourceDictionary();
            panel2.Resources.Add("key1", new ResourceTestObject { Value = 2 });

            Assert.AreEqual(1, (child1.GetValue(ResourceTestObject.TestValueProperty) as ResourceTestObject).Value);
            Assert.AreEqual(2, (child2.GetValue(ResourceTestObject.TestValueProperty) as ResourceTestObject).Value);
        }
    }
}
