using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class StaticResourceExtensionTest
    {
        [TestMethod]
        public void StaticResourceExtensionBaseTest()
        {
            string text = @"
            <StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:test='clr-namespace:Granular.Presentation.Tests;assembly=Granular.Presentation.Tests' x:Name='panel1'>
                <StackPanel.Resources>
                    <test:ResourceTestObject x:Key='key1' Value='1'/>
                    <test:ResourceTestObject x:Key='key2' Child='{StaticResource key1}'/>
                </StackPanel.Resources>

                <FrameworkElement x:Name='child1' test:ResourceTestObject.TestValue='{StaticResource key1}'/>
                <FrameworkElement x:Name='child2' test:ResourceTestObject.TestValue='{StaticResource key2}'/>
            </StackPanel>";

            FrameworkElement panel1 = XamlLoader.Load(XamlParser.Parse(text)) as FrameworkElement;
            FrameworkElement child1 = NameScope.GetNameScope(panel1).FindName("child1") as FrameworkElement;
            FrameworkElement child2 = NameScope.GetNameScope(panel1).FindName("child2") as FrameworkElement;

            ResourceTestObject testValue1 = (ResourceTestObject)child1.GetValue(ResourceTestObject.TestValueProperty);
            ResourceTestObject testValue2 = (ResourceTestObject)child2.GetValue(ResourceTestObject.TestValueProperty);

            Assert.AreEqual(1, testValue1.Value);
            Assert.AreEqual(testValue1, testValue2.Child);

            panel1.Resources.Clear();

            Assert.AreEqual(1, testValue1.Value);
            Assert.AreEqual(testValue1, testValue2.Child);
        }
    }
}
