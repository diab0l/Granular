using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Markup
{
    public enum EnumParseTestType
    {
        Value1,
        Value2,
        Value3,
    }

    [TestClass]
    public class TypeParserTest
    {
        [TypeConverter(typeof(Entry2TypeConverter))]
        private class Entry
        {
            public string Value1 { get; private set; }
            public string Value2 { get; private set; }

            public Entry(string value1, string value2)
            {
                this.Value1 = value1;
                this.Value2 = value2;
            }
        }

        private class Entry2TypeConverter : ITypeConverter
        {
            public object ConvertFrom(XamlNamespaces namespaces, object value)
            {
                string text = value.ToString().Trim();

                return new Entry(text.Split(',')[0], text.Split(',')[1]);
            }
        }

        [TestMethod]
        public void ParseTypeConverterTest()
        {
            Entry entry = (Entry)TypeConverter.ConvertValue("A,B", typeof(Entry), XamlNamespaces.Empty);

            Assert.AreEqual("A", entry.Value1);
            Assert.AreEqual("B", entry.Value2);
        }

        [TestMethod]
        public void ParseBooleanTest()
        {
            bool value1 = (bool)TypeConverter.ConvertValue("True", typeof(Boolean), XamlNamespaces.Empty);
            bool value2 = (bool)TypeConverter.ConvertValue("true", typeof(Boolean), XamlNamespaces.Empty);
            bool value3 = (bool)TypeConverter.ConvertValue("False", typeof(Boolean), XamlNamespaces.Empty);
            bool value4 = (bool)TypeConverter.ConvertValue("false", typeof(Boolean), XamlNamespaces.Empty);

            Assert.AreEqual(true, value1);
            Assert.AreEqual(true, value2);
            Assert.AreEqual(false, value3);
            Assert.AreEqual(false, value4);
        }

        [TestMethod]
        public void ParseEnumTest()
        {
            XamlNamespaces defaultNamespace = new XamlNamespaces("Granular.Presentation.Tests.Markup");
            EnumParseTestType value1 = (EnumParseTestType)TypeConverter.ConvertValue("Value1", typeof(EnumParseTestType), defaultNamespace);
            EnumParseTestType value2 = (EnumParseTestType)TypeConverter.ConvertValue("Value2", typeof(EnumParseTestType), defaultNamespace);
            EnumParseTestType value3 = (EnumParseTestType)TypeConverter.ConvertValue("Value3", typeof(EnumParseTestType), defaultNamespace);

            Assert.AreEqual(EnumParseTestType.Value1, value1);
            Assert.AreEqual(EnumParseTestType.Value2, value2);
            Assert.AreEqual(EnumParseTestType.Value3, value3);
        }

        [TestMethod]
        public void ParseTypeTest()
        {
            XamlNamespaces namespaces = new XamlNamespaces(new[]
            {
                new NamespaceDeclaration("clr-namespace:System"),
                new NamespaceDeclaration("s", "clr-namespace:System"),
            });

            Type type1 = (Type)TypeConverter.ConvertValue("Double", typeof(Type), namespaces);
            Type type2 = (Type)TypeConverter.ConvertValue("s:Double", typeof(Type), namespaces);
            Type type3 = TypeParser.ParseType("Double", namespaces);
            Type type4 = TypeParser.ParseType("s:Double", namespaces);

            Assert.AreEqual(typeof(Double), type1);
            Assert.AreEqual(typeof(Double), type2);
            Assert.AreEqual(typeof(Double), type3);
            Assert.AreEqual(typeof(Double), type4);
        }
    }
}
