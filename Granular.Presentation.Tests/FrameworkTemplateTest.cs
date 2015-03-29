using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class FrameworkTemplateTest
    {
        [TestMethod]
        public void TemplateApplyTest()
        {
            string text = @"
            <ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' x:Name='root'>
                <FrameworkElement x:Name='child' Height='400'/>
                <ControlTemplate.Triggers>
                    <Trigger Property='FrameworkElement.Width' Value='100'>
                        <Setter Property='FrameworkElement.Height' Value='100'/>
                    </Trigger>
                    <Trigger Property='FrameworkElement.Width' Value='200'>
                        <Setter Property='FrameworkElement.Height' Value='200'/>
                        <Setter TargetName='child' Property='FrameworkElement.Height' Value='200'/>
                    </Trigger>
                    <EventTrigger RoutedEvent='FrameworkElement.Initialized'>
                        <Setter Property='FrameworkElement.Height' Value='300'/>
                    </EventTrigger>
                </ControlTemplate.Triggers>
            </ControlTemplate>";

            ControlTemplate controlTemplate = XamlLoader.Load(XamlParser.Parse(text)) as ControlTemplate;

            Control control = new Control();
            control.Width = 100;
            control.Template = controlTemplate;

            control.ApplyTemplate();

            Assert.AreEqual(1, control.VisualChildren.Count());

            FrameworkElement child1 = control.TemplateChild as FrameworkElement;
            Assert.IsNotNull(child1);

            FrameworkElement child2 = control.Template.FindName("child", control) as FrameworkElement;
            Assert.AreEqual(child1, child2);

            Assert.AreEqual(control, child1.TemplatedParent);
            Assert.AreEqual(400, child1.Height);
            Assert.AreEqual(BaseValueSource.ParentTemplate, child1.GetValueSource(FrameworkElement.HeightProperty).BaseValueSource);
            Assert.AreEqual(BaseValueSource.ParentTemplate, child1.GetBaseValueSource(FrameworkElement.HeightProperty));

            Assert.AreEqual(100, control.Height);
            Assert.AreEqual(BaseValueSource.TemplateTrigger, control.GetValueSource(FrameworkElement.HeightProperty).BaseValueSource);
            Assert.AreEqual(BaseValueSource.TemplateTrigger, control.GetBaseValueSource(FrameworkElement.HeightProperty));

            control.Width = 200;
            Assert.AreEqual(200, control.Height);
            Assert.AreEqual(BaseValueSource.TemplateTrigger, control.GetValueSource(FrameworkElement.HeightProperty).BaseValueSource);
            Assert.AreEqual(BaseValueSource.TemplateTrigger, control.GetBaseValueSource(FrameworkElement.HeightProperty));

            Assert.AreEqual(200, child1.Height);
            Assert.AreEqual(BaseValueSource.ParentTemplateTrigger, child1.GetValueSource(FrameworkElement.HeightProperty).BaseValueSource);
            Assert.AreEqual(BaseValueSource.ParentTemplateTrigger, child1.GetBaseValueSource(FrameworkElement.HeightProperty));

            control.RaiseEvent(new RoutedEventArgs(FrameworkElement.InitializedEvent, control));
            Assert.AreEqual(300, control.Height);
            Assert.AreEqual(BaseValueSource.TemplateTrigger, control.GetValueSource(FrameworkElement.HeightProperty).BaseValueSource);
            Assert.AreEqual(BaseValueSource.TemplateTrigger, control.GetBaseValueSource(FrameworkElement.HeightProperty));

            control.Template = null;
            control.ApplyTemplate();

            Assert.AreEqual(Double.NaN, control.Height);
        }

        [TestMethod]
        public void TemplateFindDefaultTest()
        {
            string text = @"
            <ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' TargetType='{x:Type Control}'>
                <FrameworkElement/>
            </ControlTemplate>";

            ControlTemplate controlTemplate = XamlLoader.Load(XamlParser.Parse(text)) as ControlTemplate;

            ResourceDictionary resources = new ResourceDictionary();
            resources.Add(new TemplateKey(typeof(Control)), controlTemplate);

            Control control = new Control();
            control.Resources = resources;

            Assert.AreEqual(controlTemplate, control.Template);
        }

        [TestMethod]
        public void TemplateAndStyleFindDefaultTest()
        {
            Style style = new Style { TargetType = typeof(Control) };
            style.Setters.Add(new Setter { Property = new DependencyPropertyPathElement(FrameworkElement.WidthProperty), Value = 100 });

            string text = @"
            <ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' TargetType='{x:Type Control}'>
                <FrameworkElement/>
            </ControlTemplate>";

            ControlTemplate controlTemplate = XamlLoader.Load(XamlParser.Parse(text)) as ControlTemplate;

            ResourceDictionary resources = new ResourceDictionary();
            resources.Add(new StyleKey(typeof(Control)), style);
            resources.Add(new TemplateKey(typeof(Control)), controlTemplate);

            Control control = new Control();
            control.Resources = resources;

            Assert.AreEqual(style, control.Style);
            Assert.AreEqual(controlTemplate, control.Template);
        }
    }
}
