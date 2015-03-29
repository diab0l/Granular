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
    [DictionaryKeyProperty("TestKey")]
    public class ResourceDictionaryTestElement
    {
        public string TestKey { get; set; }
    }

    [TestClass]
    public class ResourceDictionaryTest
    {
        [TestMethod]
        public void ResourceDictionaryAddTest()
        {
            string text = @"
            <FrameworkElement xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:test='clr-namespace:Granular.Presentation.Tests;assembly=Granular.Presentation.Tests'>
                <FrameworkElement.Resources>
                    <FrameworkElement x:Key='item1'/>
                    <test:ResourceDictionaryTestElement TestKey='item2'/>
                </FrameworkElement.Resources>
            </FrameworkElement>";

            FrameworkElement element = XamlLoader.Load(XamlParser.Parse(text)) as FrameworkElement;

            Assert.IsTrue(element.Resources.GetValue("item1") is FrameworkElement);
            Assert.IsTrue(element.Resources.GetValue("item2") is ResourceDictionaryTestElement);
        }

        [TestMethod]
        public void ResourceDictionaryChangedTest()
        {
            ResourcesChangedEventArgs lastChangedArgs = null;
            int changedCount = 0;

            ResourceDictionary root = new ResourceDictionary();
            root.ResourcesChanged += (sender, e) =>
            {
                lastChangedArgs = e;
                changedCount++;
            };

            root.Add("item1", "value1");

            Assert.AreEqual(1, changedCount);
            Assert.IsTrue(lastChangedArgs.Contains("item1"));
            Assert.IsFalse(lastChangedArgs.Contains("item2"));

            ResourceDictionary child1 = new ResourceDictionary();
            child1.Add("item2", "value2");
            child1.Add("item3", "value3");

            root.MergedDictionaries.Add(child1);

            Assert.AreEqual(2, changedCount);
            Assert.IsTrue(lastChangedArgs.Contains("item2"));
            Assert.IsTrue(lastChangedArgs.Contains("item3"));
            Assert.IsFalse(lastChangedArgs.Contains("item1"));

            child1.Add("item4", "value4");

            Assert.AreEqual(3, changedCount);
            Assert.IsTrue(lastChangedArgs.Contains("item4"));
            Assert.IsFalse(lastChangedArgs.Contains("item3"));

            child1.Remove("item2");

            Assert.AreEqual(4, changedCount);
            Assert.IsTrue(lastChangedArgs.Contains("item2"));
            Assert.IsFalse(lastChangedArgs.Contains("item3"));

            ResourceDictionary child2 = new ResourceDictionary();
            child2.Add("item5", "value5");
            child2.Add("item6", "value6");

            child1.MergedDictionaries.Add(child2);

            Assert.AreEqual(5, changedCount);
            Assert.IsTrue(lastChangedArgs.Contains("item5"));
            Assert.IsTrue(lastChangedArgs.Contains("item6"));
            Assert.IsFalse(lastChangedArgs.Contains("item3"));

            child2.Add("item7", "value7");

            Assert.AreEqual(6, changedCount);
            Assert.IsTrue(lastChangedArgs.Contains("item7"));
            Assert.IsFalse(lastChangedArgs.Contains("item6"));

            child1.MergedDictionaries.Remove(child2);

            Assert.AreEqual(7, changedCount);
            Assert.IsTrue(lastChangedArgs.Contains("item5"));
            Assert.IsTrue(lastChangedArgs.Contains("item6"));
            Assert.IsFalse(lastChangedArgs.Contains("item3"));
        }
    }
}
