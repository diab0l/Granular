using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using Granular.Presentation.Tests.Markup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests
{
    public class PropertyPathTestElement
    {
        public int this[int index1, int index2] { get { return Value * index1 * index2; } }
        public int Value { get; set; }
        public PropertyPathTestElement Child { get; set; }
        public TestCollection<PropertyPathTestElement> Children { get; set; }
    }

    [TestClass]
    public class PropertyPathTest
    {
        [TestMethod]
        public void PropertyPathBasicTest()
        {
            XamlNamespaces namespaces = new XamlNamespaces(new[]
            {
                new NamespaceDeclaration("clr-namespace:Granular.Presentation.Tests;assembly=Granular.Presentation.Tests"),
                new NamespaceDeclaration("test", "clr-namespace:Granular.Presentation.Tests;assembly=Granular.Presentation.Tests")
            });

            PropertyPath path1 = PropertyPath.Parse("(test:PropertyPathTestElement.Child).PropertyName", namespaces);
            Assert.AreEqual(2, path1.Elements.Count());
            Assert.IsTrue(path1.Elements.ElementAt(0) is PropertyPathElement);
            Assert.AreEqual(new XamlName("PropertyPathTestElement.Child", "clr-namespace:Granular.Presentation.Tests;assembly=Granular.Presentation.Tests"), ((PropertyPathElement)path1.Elements.ElementAt(0)).PropertyName);
            Assert.IsTrue(path1.Elements.ElementAt(1) is PropertyPathElement);
            Assert.AreEqual(new XamlName("PropertyName", String.Empty), ((PropertyPathElement)path1.Elements.ElementAt(1)).PropertyName);

            PropertyPath path2 = PropertyPath.Parse("PropertyName[indexValue1, indexValue2]");
            Assert.IsTrue(path2.Elements.ElementAt(0) is IndexPropertyPathElement);
            Assert.AreEqual(new XamlName("PropertyName", String.Empty), ((IndexPropertyPathElement)path2.Elements.ElementAt(0)).PropertyName);
            CollectionAssert.AreEqual(new string[] { "indexValue1", "indexValue2" }, ((IndexPropertyPathElement)path2.Elements.ElementAt(0)).IndexRawValues.ToArray());

            PropertyPath path3 = PropertyPath.Parse("[indexValue1, indexValue2]");
            Assert.IsTrue(path3.Elements.ElementAt(0) is IndexPropertyPathElement);
            Assert.IsTrue(((IndexPropertyPathElement)path3.Elements.ElementAt(0)).PropertyName.IsEmpty);
            CollectionAssert.AreEqual(new string[] { "indexValue1", "indexValue2" }, ((IndexPropertyPathElement)path3.Elements.ElementAt(0)).IndexRawValues.ToArray());
        }

        [TestMethod]
        public void PropertyPathGetValueTest()
        {
            PropertyPathTestElement child2 = new PropertyPathTestElement { Value = 1 };
            PropertyPathTestElement child1 = new PropertyPathTestElement { Child = child2, Children = new TestCollection<PropertyPathTestElement> { child2 } };
            PropertyPathTestElement root = new PropertyPathTestElement { Child = child1, Children = new TestCollection<PropertyPathTestElement> { child1, child2 } };

            XamlNamespaces namespaces = new XamlNamespaces("clr-namespace:Granular.Presentation.Tests;assembly=Granular.Presentation.Tests");

            object value;

            Assert.IsTrue(PropertyPath.Parse("Value").TryGetValue(child2, out value));
            Assert.AreEqual(child2.Value, value);

            Assert.IsTrue(PropertyPath.Parse("[2, 3]").TryGetValue(child2, out value));
            Assert.AreEqual(child2.Value * 2 * 3, value);

            Assert.IsTrue(PropertyPath.Parse("Child.Child.Value").TryGetValue(root, out value));
            Assert.AreEqual(child2.Value, value);

            Assert.IsTrue(PropertyPath.Parse("Child.(PropertyPathTestElement.Child).Value", namespaces).TryGetValue(root, out value));
            Assert.AreEqual(child2.Value, value);

            Assert.IsTrue(PropertyPath.Parse("Children[1].Value").TryGetValue(root, out value));
            Assert.AreEqual(child2.Value, value);

            Assert.IsTrue(PropertyPath.Parse("(PropertyPathTestElement.Children)[1].Value", namespaces).TryGetValue(root, out value));
            Assert.AreEqual(child2.Value, value);
        }
    }
}
