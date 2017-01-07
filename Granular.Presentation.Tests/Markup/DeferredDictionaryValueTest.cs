using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Markup
{
    [SupportsValueProvider]
    public class DeferredDictionary : IDictionary<object, object>, ICollection<KeyValuePair<object, object>>
    {
        public object this[object key]
        {
            get { return dictionary[key]; }
            set { dictionary[key] = value; }
        }

        ICollection<object> IDictionary<object, object>.Keys { get; }
        ICollection<object> IDictionary<object, object>.Values { get; }

        public int Count { get { return dictionary.Count; } }

        public bool IsReadOnly { get { return false; } }

        private Dictionary<object, object> dictionary;

        public DeferredDictionary()
        {
            dictionary = new Dictionary<object, object>();
        }

        public void Add(object key, object value)
        {
            dictionary.Add(key, value);
        }

        public bool ContainsKey(object key)
        {
            return dictionary.ContainsKey(key);
        }

        public bool Remove(object key)
        {
            return dictionary.Remove(key);
        }

        public bool TryGetValue(object key, out object value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        void ICollection<KeyValuePair<object, object>>.Add(KeyValuePair<object, object> item)
        {
            ((ICollection<KeyValuePair<object, object>>)dictionary).Add(item);
        }

        bool ICollection<KeyValuePair<object, object>>.Contains(KeyValuePair<object, object> item)
        {
            return ((ICollection<KeyValuePair<object, object>>)dictionary).Contains(item);
        }

        void ICollection<KeyValuePair<object, object>>.CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<object, object>>)dictionary).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<object, object>>.Remove(KeyValuePair<object, object> item)
        {
            return ((ICollection<KeyValuePair<object, object>>)dictionary).Remove(item);
        }

        IEnumerator<KeyValuePair<object, object>> IEnumerable<KeyValuePair<object, object>>.GetEnumerator()
        {
            return ((ICollection<KeyValuePair<object, object>>)dictionary).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }
    }

    [DictionaryKeyProperty("Key")]
    [DeferredValueKeyProvider(typeof(DeferredTestDictionaryValueKeyProvider))]
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class TestDictionaryValue
    {
        private string partialKey;
        public string PartialKey
        {
            get { return partialKey; }
            set
            {
                partialKey = value;
                Key = CreateKey(partialKey);
            }
        }

        public object Key { get; set; }

        public TestDictionaryValue()
        {

        }

        public static object CreateKey(string partialKey)
        {
            return "key" + partialKey;
        }
    }

    public class DeferredTestDictionaryValueKeyProvider : IDeferredValueKeyProvider
    {
        public object GetValueKey(XamlElement element)
        {
            XamlMember keyMember = element.Members.SingleOrDefault(member => member.Name.LocalName == "Key");
            if (keyMember != null)
            {
                object value = keyMember.Values.Single();
                return value is XamlElement ? XamlLoader.Load((XamlElement)value) : value;
            }

            XamlMember partialKeyMember = element.Members.SingleOrDefault(member => member.Name.LocalName == "PartialKey");
            if (partialKeyMember != null)
            {
                return TestDictionaryValue.CreateKey(partialKeyMember.Values.Single().ToString());
            }

            throw new Granular.Exception($"Can't create value key from \"{element.Name}\"");
        }
    }

    [TestClass]
    public class DeferredDictionaryValueTest
    {
        [TestMethod]
        public void DeferredValueTest()
        {
            string text = @"
            <test:DeferredDictionary xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                                     xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                                     xmlns:test='clr-namespace:Granular.Presentation.Tests.Markup;assembly=Granular.Presentation.Tests'>
                <test:TestDictionaryValue x:Key='key1'/>
            </test:DeferredDictionary>";

            DeferredDictionary dictionary = (DeferredDictionary)XamlLoader.Load(XamlParser.Parse(text));

            Assert.IsTrue(dictionary.ContainsKey("key1"));

            ValueProvider valueProvider1 = dictionary["key1"] as ValueProvider;

            Assert.IsNotNull(valueProvider1);

            TestDictionaryValue value1 = valueProvider1.ProvideValue() as TestDictionaryValue;

            Assert.IsNotNull(value1);
        }

        [TestMethod]
        public void DeferredValueKeyProviderTest()
        {
            string text = @"
            <test:DeferredDictionary xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                                     xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                                     xmlns:test='clr-namespace:Granular.Presentation.Tests.Markup;assembly=Granular.Presentation.Tests'>
                <test:TestDictionaryValue Key='key1' />
                <test:TestDictionaryValue PartialKey='2' />
            </test:DeferredDictionary>";

            DeferredDictionary dictionary = (DeferredDictionary)XamlLoader.Load(XamlParser.Parse(text));

            Assert.IsTrue(dictionary.ContainsKey("key1"));
            Assert.IsTrue(dictionary.ContainsKey("key2"));

            ValueProvider valueProvider1 = dictionary["key1"] as ValueProvider;
            ValueProvider valueProvider2 = dictionary["key2"] as ValueProvider;

            Assert.IsNotNull(valueProvider1);
            Assert.IsNotNull(valueProvider2);

            TestDictionaryValue value1 = valueProvider1.ProvideValue() as TestDictionaryValue;
            TestDictionaryValue value2 = valueProvider2.ProvideValue() as TestDictionaryValue;

            Assert.IsNotNull(value1);
            Assert.IsNotNull(value2);

            Assert.AreEqual(value1.Key, "key1");

            Assert.AreEqual(value2.PartialKey, "2");
            Assert.AreEqual(value2.Key, "key2");
        }

        [TestMethod]
        public void DeferredValueSharedTest()
        {
            string text = @"
            <test:DeferredDictionary xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                                     xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                                     xmlns:test='clr-namespace:Granular.Presentation.Tests.Markup;assembly=Granular.Presentation.Tests'>
                <test:TestDictionaryValue x:Key='key1'/>
                <test:TestDictionaryValue x:Key='key2' x:Shared='false'/>
            </test:DeferredDictionary>";

            DeferredDictionary dictionary = (DeferredDictionary)XamlLoader.Load(XamlParser.Parse(text));

            Assert.IsTrue(dictionary.ContainsKey("key1"));
            Assert.IsTrue(dictionary.ContainsKey("key2"));

            ValueProvider valueProvider1 = dictionary["key1"] as ValueProvider;
            ValueProvider valueProvider2 = dictionary["key2"] as ValueProvider;

            Assert.IsNotNull(valueProvider1);
            Assert.IsNotNull(valueProvider2);

            TestDictionaryValue value1 = valueProvider1.ProvideValue() as TestDictionaryValue;
            TestDictionaryValue value2 = valueProvider2.ProvideValue() as TestDictionaryValue;

            Assert.IsNotNull(value1);
            Assert.IsNotNull(value2);

            TestDictionaryValue value1a = valueProvider1.ProvideValue() as TestDictionaryValue;
            TestDictionaryValue value2a = valueProvider2.ProvideValue() as TestDictionaryValue;

            Assert.IsTrue(ReferenceEquals(value1, value1a));
            Assert.IsFalse(ReferenceEquals(value2, value2a));
        }
    }
}
