using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Data;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Xaml;

namespace Granular.Presentation.Tests.Data
{
    [TestClass]
    public class BindingSourceObserverTest
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached("Value", typeof(object), typeof(BindingSourceObserverTest), new PropertyMetadata());

        [TestMethod]
        public void ScopeElementSourceObserverTest()
        {
            FrameworkElement element = new FrameworkElement();
            NameScope nameScope = new NameScope();
            NameScope.SetNameScope(element, nameScope);
            nameScope.RegisterName("element", element);

            Freezable value = new Freezable();

            ScopeElementSourceObserver observer = new ScopeElementSourceObserver(value, "element");
            Assert.AreEqual(ObservableValue.UnsetValue, observer.Value);

            element.SetValue(ValueProperty, value);
            Assert.AreEqual(element, observer.Value);

            element.SetValue(ValueProperty, null);
            Assert.AreEqual(ObservableValue.UnsetValue, observer.Value);
        }

        [TestMethod]
        public void FindAncestorSourceObserverTest()
        {
            FrameworkElement parent = new FrameworkElement();
            FrameworkElement child = new FrameworkElement();
            parent.AddVisualChild(child);

            Freezable value = new Freezable();

            FindAncestorSourceObserver observer1 = new FindAncestorSourceObserver(value, typeof(FrameworkElement), 0);
            FindAncestorSourceObserver observer2 = new FindAncestorSourceObserver(value, typeof(FrameworkElement), 2);
            FindAncestorSourceObserver observer3 = new FindAncestorSourceObserver(value, null, 2);
            Assert.AreEqual(ObservableValue.UnsetValue, observer1.Value);
            Assert.AreEqual(ObservableValue.UnsetValue, observer2.Value);
            Assert.AreEqual(ObservableValue.UnsetValue, observer3.Value);

            child.SetValue(ValueProperty, value);
            Assert.AreEqual(child, observer1.Value);
            Assert.AreEqual(parent, observer2.Value);
            Assert.AreEqual(parent, observer3.Value);

            child.SetValue(ValueProperty, null);
            Assert.AreEqual(ObservableValue.UnsetValue, observer1.Value);
            Assert.AreEqual(ObservableValue.UnsetValue, observer2.Value);
            Assert.AreEqual(ObservableValue.UnsetValue, observer3.Value);
        }

        [TestMethod]
        public void TemplatedParentSourceObserverTest()
        {
            string text = @"
            <ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                <FrameworkElement x:Name='child'/>
            </ControlTemplate>";

            ControlTemplate template = XamlLoader.Load(XamlParser.Parse(text)) as ControlTemplate;
            Control control = new Control();

            control.Template = template;
            control.ApplyTemplate();

            FrameworkElement child = NameScope.GetTemplateNameScope(control).FindName("child") as FrameworkElement;

            Freezable value = new Freezable();

            TemplatedParentSourceObserver observer = new TemplatedParentSourceObserver(value);
            Assert.AreEqual(ObservableValue.UnsetValue, observer.Value);

            child.SetValue(ValueProperty, value);
            Assert.AreEqual(control, observer.Value);

            child.SetValue(ValueProperty, null);
            Assert.AreEqual(ObservableValue.UnsetValue, observer.Value);
        }

        [TestMethod]
        public void DataContextSourceObserverTest()
        {
            FrameworkElement element = new FrameworkElement();
            element.DataContext = "data-context";

            Freezable value = new Freezable();

            DataContextSourceObserver observer = new DataContextSourceObserver(value);
            Assert.AreEqual(ObservableValue.UnsetValue, observer.Value);

            element.SetValue(ValueProperty, value);
            Assert.AreEqual("data-context", observer.Value);

            element.DataContext = null;
            Assert.AreEqual(null, observer.Value);

            element.SetValue(ValueProperty, null);
            Assert.AreEqual(ObservableValue.UnsetValue, observer.Value);
        }
    }
}
