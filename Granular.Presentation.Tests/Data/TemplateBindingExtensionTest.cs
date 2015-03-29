using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Data
{
    [TestClass]
    public class TemplateBindingExtensionTest
    {
        [TestMethod]
        public void BindingExtensionTemplatedParentTest()
        {
            string text = @"
            <ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' x:Name='root'>
                <FrameworkElement x:Name='child' Width='{TemplateBinding FrameworkElement.Height}'/>
            </ControlTemplate>";

            ControlTemplate template = XamlLoader.Load(XamlParser.Parse(text)) as ControlTemplate;

            Control control = new Control();

            control.Template = template;
            control.ApplyTemplate();

            FrameworkElement child = NameScope.GetTemplateNameScope(control).FindName("child") as FrameworkElement;

            Assert.AreEqual(control, child.TemplatedParent);

            control.Height = 100;
            Assert.AreEqual(100, child.Width);
        }
    }
}
