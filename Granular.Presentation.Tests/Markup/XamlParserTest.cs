using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
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
                member1='value1'
                ns2:member2='value2'>
                <!-- comment -->
                <ns1:child1/>
                <child2>
                    value3
                    <child3>
                    </child3>
                    value4
                </child2>
            </root1>");

            Assert.AreEqual("root1", root1.Name.LocalName);

            Assert.AreEqual(2, root1.Members.Count());
            Assert.IsTrue(root1.Members.Any(member => member.Name.Equals(new XamlName("member1", "http://schemas.microsoft.com/winfx/2006/xaml/presentation")) && (string)member.Values.Single() == "value1"));
            Assert.IsTrue(root1.Members.Any(member => member.Name.Equals(new XamlName("member2", "namespace2")) && (string)member.Values.Single() == "value2"));

            Assert.AreEqual(2, root1.Values.Count());
            Assert.AreEqual("child1", ((XamlElement)root1.Values.First()).Name.LocalName);
            Assert.AreEqual("namespace1", ((XamlElement)root1.Values.First()).Name.NamespaceName);

            Assert.AreEqual("value3", ((XamlElement)root1.Values.Last()).Values.ElementAt(0));
            Assert.AreEqual("child3", ((XamlElement)((XamlElement)root1.Values.Last()).Values.ElementAt(1)).Name.LocalName);
            Assert.AreEqual("value4", ((XamlElement)root1.Values.Last()).Values.ElementAt(2));
        }

        [TestMethod]
        public void ParseNamespaceTest()
        {
            XamlElement root1 = XamlParser.Parse(@"
            <root1 xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:ns1='namespace1'>
                <element1 xmlns:ns2='namespace2'/>
            </root1>");

            Assert.AreEqual("http://schemas.microsoft.com/winfx/2006/xaml/presentation", root1.Namespaces.GetNamespace(String.Empty));

            Assert.AreEqual("namespace1", root1.Namespaces.GetNamespace("ns1"));

            Assert.AreEqual("namespace2", ((XamlElement)root1.Values.Single()).Namespaces.GetNamespace("ns2"));
        }

        [TestMethod]
        public void MarkupCompatibilityIgnoreMultipleNamespaces()
        {
            XamlElement root = XamlParser.Parse(@"
            <root xmlns='namespace'
                  xmlns:n1='namepsace1'
                  xmlns:n2='namepsace2'
                  xmlns:mc='http://schemas.openxmlformats.org/markup-compatibility/2006'
                  mc:Ignorable='n1 n2'
                  property='value'
                  n1:property='value1'
                  n2:property='value2'>
            </root>");

            Assert.AreEqual(1, root.Members.Count());
        }

        [TestMethod]
        public void MarkupCompatibilityIgnoreDeclaringElement()
        {
            XamlElement root = XamlParser.Parse(@"
            <root xmlns='namespace' xmlns:n1='namepsace1' xmlns:mc='http://schemas.openxmlformats.org/markup-compatibility/2006'>
                <n1:element mc:Ignorable='n1'/>
                <n1:element />
            </root>");

            Assert.AreEqual(1, root.Values.Count());
        }

        [TestMethod]
        public void MarkupCompatibilityNestedIgnoreProperties()
        {
            XamlElement root = XamlParser.Parse(@"
            <root xmlns='namespace'
                  xmlns:n1='namepsace1'
                  xmlns:n2='namepsace2'
                  xmlns:mc='http://schemas.openxmlformats.org/markup-compatibility/2006'
                  property='valuevalue'
                  n1:property='value1'
                  n2:property='value2'>
                <element mc:Ignorable='n1'
                         property='valuevalue'
                         n1:property='value1'
                         n2:property='value2'>
                    <element mc:Ignorable='n2'
                             property='valuevalue'
                             n1:property='value1'
                             n2:property='value2'/>
                </element>
            </root>");

            Assert.AreEqual(3, root.Members.Count());
            Assert.AreEqual(2, ((XamlElement)root.Values.First()).Members.Count());
            Assert.AreEqual(1, ((XamlElement)((XamlElement)root.Values.First()).Values.First()).Members.Count());
        }

        [TestMethod]
        public void MarkupCompatibilityNestedIgnoreElements()
        {
            XamlElement root = XamlParser.Parse(@"
            <root xmlns='namespace'
                  xmlns:n1='namepsace1'
                  xmlns:n2='namepsace2'
                  xmlns:mc='http://schemas.openxmlformats.org/markup-compatibility/2006'>
                <element mc:Ignorable='n1'>
                    <element mc:Ignorable='n2'>
                        <element/>
                        <n1:element/>
                        <n2:element/>
                    </element>
                    <n1:element/>
                    <n2:element/>
                </element>
                <n1:element/>
                <n2:element/>
            </root>");

            Assert.AreEqual(3, root.Values.Count());
            Assert.AreEqual(2, ((XamlElement)root.Values.First()).Values.Count());
            Assert.AreEqual(1, ((XamlElement)((XamlElement)root.Values.First()).Values.First()).Values.Count());
        }
    }
}