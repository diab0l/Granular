using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class ResourceReferenceExpressionTest
    {
        public static readonly DependencyProperty TestValueProperty = DependencyProperty.RegisterAttached("TestValue", typeof(string), typeof(ResourceReferenceExpressionTest), new FrameworkPropertyMetadata("default"));

        [TestMethod]
        public void ResourceReferenceExpressionBaseTest()
        {
            FrameworkElement root = new FrameworkElement();
            FrameworkElement child1 = new FrameworkElement();
            FrameworkElement child2 = new FrameworkElement();

            child1.AddVisualChild(child2);


            int testValueChanged = 0;
            child2.PropertyChanged += (sender, e) => testValueChanged += e.Property == TestValueProperty ? 1 : 0;

            child2.SetValue(TestValueProperty, new ResourceReferenceExpressionProvider("key1"));

            Assert.AreEqual("default", child2.GetValue(TestValueProperty));
            Assert.AreEqual(0, testValueChanged);

            root.Resources = new ResourceDictionary();
            root.Resources.Add("key1", "value1");
            root.AddVisualChild(child1);

            Assert.AreEqual("value1", child2.GetValue(TestValueProperty));
            Assert.AreEqual(1, testValueChanged);

            root.Resources.Remove("key1");

            Assert.AreEqual("default", child2.GetValue(TestValueProperty));
            Assert.AreEqual(2, testValueChanged);

            ResourceDictionary dictionary = new ResourceDictionary();
            dictionary.Add("key1", "value2");

            root.Resources.MergedDictionaries.Add(dictionary);

            Assert.AreEqual("value2", child2.GetValue(TestValueProperty));
            Assert.AreEqual(3, testValueChanged);

            child1.Resources = new ResourceDictionary();
            child1.Resources.Add("key1", "value3");

            Assert.AreEqual("value3", child2.GetValue(TestValueProperty));
            Assert.AreEqual(4, testValueChanged);

            child1.Resources.Remove("key1");

            Assert.AreEqual("value2", child2.GetValue(TestValueProperty));
            Assert.AreEqual(5, testValueChanged);
        }
    }
}
