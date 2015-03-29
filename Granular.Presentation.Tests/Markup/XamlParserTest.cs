using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Markup
{
    [TestClass]
    public class XamlParserTest
    {
        [TestMethod]
        public void ParseTest()
        {
            XamlElement root1 = XamlParser.Parse(@"
            <root1
                xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                xmlns:ns1='namespace1'
                xmlns:ns2='namespace2'
                attribute1='value1'
                ns2:attribute2='value2'>
                <!-- comment -->
                <ns1:child1/>
                <child2>
                    <child3>
                    </child3>
                    value3
                </child2>
            </root1>");

            Assert.AreEqual("root1", root1.Name.LocalName);

            Assert.AreEqual(2, root1.Attributes.Count());
            Assert.IsTrue(root1.Attributes.Any(attribute => attribute.Name.Equals(new XamlName("attribute1", "http://schemas.microsoft.com/winfx/2006/xaml/presentation")) && (string)attribute.Value == "value1"));
            Assert.IsTrue(root1.Attributes.Any(attribute => attribute.Name.Equals(new XamlName("attribute2", "namespace2")) && (string)attribute.Value == "value2"));

            Assert.AreEqual(2, root1.Children.Count());
            Assert.AreEqual("child1", root1.Children.First().Name.LocalName);
            Assert.AreEqual("namespace1", root1.Children.First().Name.NamespaceName);

            Assert.AreEqual("value3", root1.Children.Last().TextValue);
        }

        [TestMethod]
        public void ParseNamespaceTest()
        {
            XamlElement root1 = XamlParser.Parse(@"
            <root1 xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:ns1='namespace1'>
                <element1 xmlns:ns2='namespace2'/>
            </root1>");

            Assert.AreEqual("http://schemas.microsoft.com/winfx/2006/xaml/presentation", root1.Namespaces.Get(String.Empty));

            Assert.AreEqual("namespace1", root1.Namespaces.Get("ns1"));

            Assert.AreEqual("namespace2", root1.Children.Single().Namespaces.Get("ns2"));
        }
    }
}
