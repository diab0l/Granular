using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Granular.Collections;

namespace Granular.Presentation.Tests.Markup
{
    [TypeConverter(typeof(LoaderTestElementConverter))]
    [ContentProperty("Children")]
    public class LoaderTestElement : FrameworkElement
    {
        public event EventHandler Action1;

        public static readonly RoutedEvent Action2Event = EventManager.RegisterRoutedEvent("Action2", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(LoaderTestElement));

        public int Value1 { get; set; }

        public static readonly DependencyProperty Value2Property = DependencyProperty.Register("Value2", typeof(int), typeof(LoaderTestElement), new PropertyMetadata(0));
        public int Value2
        {
            get { return (int)GetValue(Value2Property); }
            set { SetValue(Value2Property, value); }
        }


        public object Value3 { get; set; }

        public ObservableCollection<object> Children { get; private set; }

        public LoaderTestElement()
        {
            Children = new ObservableCollection<object>();
        }

        public void RaiseAction1()
        {
            if (Action1 != null)
            {
                Action1(this, EventArgs.Empty);
            }
        }

        public void RaiseAction2()
        {
            RaiseEvent(new RoutedEventArgs(Action2Event, this));
        }
    }

    public class LoaderTestTopElement : LoaderTestElement
    {
        public int Handler1Calls { get; private set; }
        public int Handler2Calls { get; private set; }
        public int Handler3Calls { get; private set; }
        public int Handler4Calls { get; private set; }

        private void Handler1(object sender, EventArgs e) { Handler1Calls++; }
        private void Handler2(object sender, EventArgs e) { Handler2Calls++; }
        private void Handler3(object sender, EventArgs e) { Handler3Calls++; }
        private void Handler4(object sender, EventArgs e) { Handler4Calls++; }
    }

    [ContentProperty("Content1")]
    public class LoaderTestContentElement
    {
        public int Content1 { get; set; }
    }

    [ContentProperty("Factory")]
    public class LoaderTestFactoryElement
    {
        public IFrameworkElementFactory Factory { get; set; }
    }

    public class TestCollection<T> : IList<T>
    {
        public int Count { get { return list.Count; } }

        public T this[int index]
        {
            get { return list[index]; }
            set { list[index] = value; }
        }

        public bool IsReadOnly { get { return false; } }

        private List<T> list;

        public TestCollection()
        {
            list = new List<T>();
        }

        public void Add(T item)
        {
            list.Add(item);
        }

        public void Insert(int index, T item)
        {
            list.Insert(index, item);
        }

        public bool Remove(T item)
        {
            return list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class LoaderTestCollection : TestCollection<object>
    {
        //
    }

    [ContentProperty("Items")]
    public class LoaderTestCollectionElement
    {
        public LoaderTestCollection Items { get; set; }

        public LoaderTestCollectionElement()
        {
            Items = new LoaderTestCollection();
            Items.Add("item0");
        }
    }

    public class LoaderTestBindingExtension : IMarkupExtension
    {
        public string Path { get; set; }

        public object ProvideValue(InitializeContext context)
        {
            return new LoaderTestBindingExpression { Path = this.Path };
        }
    }

    public class LoaderTestBindingExpression
    {
        public string Path { get; set; }
    }

    [TypeConverter(typeof(LoaderTestContentConverter))]
    public class LoaderTestConvertContentElement
    {
        public int Value { get; private set; }

        public LoaderTestConvertContentElement(int value)
        {
            this.Value = value;
        }
    }

    public class LoaderTestContentConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, Uri sourceUri, object value)
        {
            if (value is LoaderTestElement)
            {
                return new LoaderTestConvertContentElement(((LoaderTestElement)value).Value1);
            }

            return new LoaderTestConvertContentElement(Int32.Parse((string)value));
        }
    }

    public class LoaderTestConvertElement
    {
        public LoaderTestElement Element { get; set; }
    }

    public class LoaderTestElementConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, Uri sourceUri, object value)
        {
            if (value is LoaderTestContentElement)
            {
                return new LoaderTestElement { Value1 = ((LoaderTestContentElement)value).Content1 };
            }

            throw new Granular.Exception("Can't convert to \"LoaderTestElement\" from type \"{0}\"", value.GetType());
        }
    }

    [ContentProperty("Elements")]
    public class LoaderTestConvertElementCollection
    {
        public TestCollection<LoaderTestElement> Elements { get; set; }
    }

    [TestClass]
    public class XamlLoaderTest
    {
        [TestMethod]
        public void XamlLoadTest()
        {
            string text = @"
            <test:LoaderTestElement xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:test='clr-namespace:Granular.Presentation.Tests.Markup;assembly=Granular.Presentation.Tests'>
                <test:LoaderTestElement.Value1>1</test:LoaderTestElement.Value1>
                <test:LoaderTestElement.Value2>2</test:LoaderTestElement.Value2>
                <test:LoaderTestElement Value1='3' Value2='4'/>
                <test:LoaderTestElement test:LoaderTestElement.Value1='5' test:LoaderTestElement.Value2='6'/>
            </test:LoaderTestElement>";

            XamlElement rootElement = XamlParser.Parse(text);
            object root1 = XamlLoader.Load(rootElement);

            Assert.IsTrue(root1 is LoaderTestElement);
            Assert.AreEqual(2, (root1 as LoaderTestElement).Children.Count);
            Assert.AreEqual(1, (root1 as LoaderTestElement).Value1);
            Assert.AreEqual(2, (root1 as LoaderTestElement).Value2);
            Assert.AreEqual(3, ((root1 as LoaderTestElement).Children[0] as LoaderTestElement).Value1);
            Assert.AreEqual(4, ((root1 as LoaderTestElement).Children[0] as LoaderTestElement).Value2);
            Assert.AreEqual(5, ((root1 as LoaderTestElement).Children[1] as LoaderTestElement).Value1);
            Assert.AreEqual(6, ((root1 as LoaderTestElement).Children[1] as LoaderTestElement).Value2);
        }

        [TestMethod]
        public void XamlMarkupExtensionTest()
        {
            string text = @"<test:LoaderTestElement xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:test='clr-namespace:Granular.Presentation.Tests.Markup;assembly=Granular.Presentation.Tests' Value3='{test:LoaderTestBinding Path=A.B.C}'/>";

            XamlElement rootElement = XamlParser.Parse(text);
            object root1 = XamlLoader.Load(rootElement);

            Assert.IsTrue((root1 as LoaderTestElement).Value3 is LoaderTestBindingExpression);
            Assert.AreEqual("A.B.C", ((root1 as LoaderTestElement).Value3 as LoaderTestBindingExpression).Path);
        }

        [TestMethod]
        public void XamlTypesTest()
        {
            object element1 = XamlLoader.Load(XamlParser.Parse("<x:Null xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'/>"));
            Assert.IsNull(element1);

            object element2 = XamlLoader.Load(XamlParser.Parse("<x:Type xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:s='clr-namespace:System' Type='s:Double'/>"));
            Assert.AreEqual(typeof(Double), element2);
        }

        [TestMethod]
        public void XamlLoadContentTest()
        {
            string text = "<test:LoaderTestContentElement xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:test='clr-namespace:Granular.Presentation.Tests.Markup;assembly=Granular.Presentation.Tests'>1</test:LoaderTestContentElement>";

            object element1 = XamlLoader.Load(XamlParser.Parse(text));

            Assert.IsTrue(element1 is LoaderTestContentElement);
            Assert.AreEqual(1, (element1 as LoaderTestContentElement).Content1);
        }

        [TestMethod]
        public void XamlLoadNameDirectiveTest()
        {
            string text = @"
            <test:LoaderTestElement xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:test='clr-namespace:Granular.Presentation.Tests.Markup;assembly=Granular.Presentation.Tests'>
                <test:LoaderTestElement x:Name='element1' Value1='1'/>
                <test:LoaderTestElement x:Name='element2' Value1='2'/>
            </test:LoaderTestElement>";

            LoaderTestElement root = XamlLoader.Load(XamlParser.Parse(text)) as LoaderTestElement;

            LoaderTestElement element1 = NameScope.GetNameScope(root).FindName("element1") as LoaderTestElement;
            LoaderTestElement element2 = NameScope.GetNameScope(root).FindName("element2") as LoaderTestElement;

            Assert.IsTrue(element1 != null);
            Assert.IsTrue(element2 != null);
            Assert.AreEqual(1, element1.Value1);
            Assert.AreEqual(2, element2.Value1);
        }

        [TestMethod]
        public void XamlLoadEventsTest()
        {
            string text = @"
            <test:LoaderTestElement
                xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                xmlns:test='clr-namespace:Granular.Presentation.Tests.Markup;assembly=Granular.Presentation.Tests'>

                <test:LoaderTestElement x:Name='element1' Action1='Handler1' Action2='Handler2'/>
                <test:LoaderTestElement x:Name='element2' test:LoaderTestElement.Action1='Handler3' test:LoaderTestElement.Action2='Handler4'/>

            </test:LoaderTestElement>";

            LoaderTestTopElement root = new LoaderTestTopElement();
            XamlLoader.Load(root, XamlParser.Parse(text));

            LoaderTestElement element1 = NameScope.GetNameScope(root).FindName("element1") as LoaderTestElement;
            LoaderTestElement element2 = NameScope.GetNameScope(root).FindName("element2") as LoaderTestElement;

            element1.RaiseAction1();
            element1.RaiseAction2();

            element2.RaiseAction1();
            element2.RaiseAction1();
            element2.RaiseAction2();
            element2.RaiseAction2();

            Assert.IsTrue(root != null);
            Assert.AreEqual(1, root.Handler1Calls);
            //Assert.AreEqual(1, root.Handler2Calls);
            Assert.AreEqual(2, root.Handler3Calls);
            //Assert.AreEqual(2, root.Handler4Calls);
        }

        [TestMethod]
        public void XamlLoadFrameworkElementFactoryTest()
        {
            string text = @"
            <test:LoaderTestFactoryElement xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:test='clr-namespace:Granular.Presentation.Tests.Markup;assembly=Granular.Presentation.Tests'>
                <test:LoaderTestElement Value1='1' Value2='2'/>
            </test:LoaderTestFactoryElement>";

            XamlElement rootElement = XamlParser.Parse(text);
            object root1 = XamlLoader.Load(rootElement);

            Assert.IsTrue(root1 is LoaderTestFactoryElement);
            Assert.IsTrue(((LoaderTestFactoryElement)root1).Factory != null);

            LoaderTestElement contentElement = ((LoaderTestFactoryElement)root1).Factory.CreateElement(null) as LoaderTestElement;

            Assert.IsNotNull(contentElement);
            Assert.AreEqual(1, contentElement.Value1);
            Assert.AreEqual(2, contentElement.Value2);
        }

        [TestMethod]
        public void XamlLoadSharedTest()
        {
            string text = @"
            <ResourceDictionary xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:test='clr-namespace:Granular.Presentation.Tests.Markup;assembly=Granular.Presentation.Tests'>
                <test:LoaderTestElement x:Shared='false' x:Key='item1' Value1='1' Value2='2'/>
                <test:LoaderTestElement x:Shared='true' x:Key='item2' Value1='3' Value2='4'/>
            </ResourceDictionary>";

            XamlElement rootElement = XamlParser.Parse(text);
            ResourceDictionary dictionary = XamlLoader.Load(rootElement) as ResourceDictionary;

            Assert.IsNotNull(dictionary);
            Assert.IsTrue(dictionary.Contains("item1"));
            Assert.IsTrue(dictionary.Contains("item2"));

            IValueProvider valueProvider = dictionary.GetValue("item1") as IValueProvider;
            Assert.IsNotNull(valueProvider);

            LoaderTestElement item1a = valueProvider.ProvideValue() as LoaderTestElement;
            Assert.IsNotNull(item1a);
            Assert.AreEqual(1, ((LoaderTestElement)item1a).Value1);
            Assert.AreEqual(2, ((LoaderTestElement)item1a).Value2);

            LoaderTestElement item1b = valueProvider.ProvideValue() as LoaderTestElement;
            Assert.IsNotNull(item1b);
            Assert.AreNotEqual(item1a, item1b);
            Assert.AreEqual(1, ((LoaderTestElement)item1b).Value1);
            Assert.AreEqual(2, ((LoaderTestElement)item1b).Value2);

            LoaderTestElement item2 = dictionary.GetValue("item2") as LoaderTestElement;
            Assert.IsNotNull(item2);
            Assert.AreEqual(3, ((LoaderTestElement)item2).Value1);
            Assert.AreEqual(4, ((LoaderTestElement)item2).Value2);
        }

        [TestMethod]
        public void XamlLoadCollectionAddPropertyTest()
        {
            string text = @"
            <test:LoaderTestCollectionElement xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:test='clr-namespace:Granular.Presentation.Tests.Markup;assembly=Granular.Presentation.Tests'>
                <test:LoaderTestCollectionElement.Items>
                    <test:LoaderTestElement Value1='1'/>
                    <test:LoaderTestElement Value1='2'/>
                    <test:LoaderTestElement Value1='3'/>
                </test:LoaderTestCollectionElement.Items>
            </test:LoaderTestCollectionElement>";

            XamlElement rootElement = XamlParser.Parse(text);
            object root1 = XamlLoader.Load(rootElement);

            Assert.IsTrue(root1 is LoaderTestCollectionElement);
            Assert.AreEqual(4, (root1 as LoaderTestCollectionElement).Items.Count);
            Assert.AreEqual("item0", (root1 as LoaderTestCollectionElement).Items[0]);
            Assert.IsTrue((root1 as LoaderTestCollectionElement).Items[1] is LoaderTestElement);
            Assert.IsTrue((root1 as LoaderTestCollectionElement).Items[2] is LoaderTestElement);
            Assert.IsTrue((root1 as LoaderTestCollectionElement).Items[3] is LoaderTestElement);
            Assert.AreEqual(1, ((root1 as LoaderTestCollectionElement).Items[1] as LoaderTestElement).Value1);
            Assert.AreEqual(2, ((root1 as LoaderTestCollectionElement).Items[2] as LoaderTestElement).Value1);
            Assert.AreEqual(3, ((root1 as LoaderTestCollectionElement).Items[3] as LoaderTestElement).Value1);
        }

        [TestMethod]
        public void XamlLoadCollectionReplacePropertyTest()
        {
            string text = @"
            <test:LoaderTestCollectionElement xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:test='clr-namespace:Granular.Presentation.Tests.Markup;assembly=Granular.Presentation.Tests'>
                <test:LoaderTestCollectionElement.Items>
                    <test:LoaderTestCollection>
                        <test:LoaderTestElement Value1='1'/>
                        <test:LoaderTestElement Value1='2'/>
                        <test:LoaderTestElement Value1='3'/>
                    </test:LoaderTestCollection>
                </test:LoaderTestCollectionElement.Items>
            </test:LoaderTestCollectionElement>";

            XamlElement rootElement = XamlParser.Parse(text);
            object root1 = XamlLoader.Load(rootElement);

            Assert.IsTrue(root1 is LoaderTestCollectionElement);
            Assert.AreEqual(3, (root1 as LoaderTestCollectionElement).Items.Count);
            Assert.IsTrue((root1 as LoaderTestCollectionElement).Items[0] is LoaderTestElement);
            Assert.IsTrue((root1 as LoaderTestCollectionElement).Items[1] is LoaderTestElement);
            Assert.IsTrue((root1 as LoaderTestCollectionElement).Items[2] is LoaderTestElement);
            Assert.AreEqual(1, ((root1 as LoaderTestCollectionElement).Items[0] as LoaderTestElement).Value1);
            Assert.AreEqual(2, ((root1 as LoaderTestCollectionElement).Items[1] as LoaderTestElement).Value1);
            Assert.AreEqual(3, ((root1 as LoaderTestCollectionElement).Items[2] as LoaderTestElement).Value1);
        }

        [TestMethod]
        public void XamlLoadFromContentTest()
        {
            string text = @"
            <test:LoaderTestCollectionElement xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:test='clr-namespace:Granular.Presentation.Tests.Markup;assembly=Granular.Presentation.Tests'>
                <test:LoaderTestConvertContentElement>1</test:LoaderTestConvertContentElement>
                <test:LoaderTestConvertContentElement><test:LoaderTestElement Value1='2'/></test:LoaderTestConvertContentElement>
            </test:LoaderTestCollectionElement>";

            LoaderTestCollectionElement collection = XamlLoader.Load(XamlParser.Parse(text)) as LoaderTestCollectionElement;
            collection.Items.RemoveAt(0);

            Assert.IsTrue(collection.Items[0] is LoaderTestConvertContentElement);
            Assert.AreEqual(1, ((LoaderTestConvertContentElement)collection.Items[0]).Value);

            Assert.IsTrue(collection.Items[1] is LoaderTestConvertContentElement);
            Assert.AreEqual(2, ((LoaderTestConvertContentElement)collection.Items[1]).Value);
        }

        [TestMethod]
        public void XamlLoadConverterTest()
        {
            string text = @"
            <test:LoaderTestConvertElement xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:test='clr-namespace:Granular.Presentation.Tests.Markup;assembly=Granular.Presentation.Tests'>
                <test:LoaderTestConvertElement.Element>
                    <test:LoaderTestContentElement>1</test:LoaderTestContentElement>
                </test:LoaderTestConvertElement.Element>
            </test:LoaderTestConvertElement>";

            LoaderTestConvertElement root = XamlLoader.Load(XamlParser.Parse(text)) as LoaderTestConvertElement;

            Assert.IsNotNull(root);
            Assert.IsNotNull(root.Element);
            Assert.AreEqual(1, root.Element.Value1);
        }

        [TestMethod]
        public void XamlLoadCollectionConverterTest()
        {
            string text = @"
            <test:LoaderTestConvertElementCollection xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:test='clr-namespace:Granular.Presentation.Tests.Markup;assembly=Granular.Presentation.Tests'>
                <test:LoaderTestContentElement>1</test:LoaderTestContentElement>
                <test:LoaderTestContentElement>2</test:LoaderTestContentElement>
                <test:LoaderTestContentElement>3</test:LoaderTestContentElement>
            </test:LoaderTestConvertElementCollection>";

            LoaderTestConvertElementCollection root = XamlLoader.Load(XamlParser.Parse(text)) as LoaderTestConvertElementCollection;

            Assert.IsNotNull(root);
            Assert.IsNotNull(root.Elements);
            Assert.AreEqual(3, root.Elements.Count);
            Assert.AreEqual(1, root.Elements[0].Value1);
            Assert.AreEqual(2, root.Elements[1].Value1);
            Assert.AreEqual(3, root.Elements[2].Value1);
        }

        [TestMethod]
        public void XamlLoadKeyElementTest()
        {
            string text = @"
            <ResourceDictionary xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:test='clr-namespace:Granular.Presentation.Tests.Markup;assembly=Granular.Presentation.Tests'>
                <test:LoaderTestElement Value1='1' Value2='2'>
                    <x:Key>
                        <test:LoaderTestElement Value1='3' Value2='4'/>
                    </x:Key>
                </test:LoaderTestElement>
            </ResourceDictionary>";

            XamlElement rootElement = XamlParser.Parse(text);
            ResourceDictionary dictionary = XamlLoader.Load(rootElement) as ResourceDictionary;

            Assert.IsNotNull(dictionary);
            Assert.AreEqual(1, dictionary.Count);

            Assert.AreEqual(1, ((LoaderTestElement)dictionary.Values.First()).Value1);
            Assert.AreEqual(2, ((LoaderTestElement)dictionary.Values.First()).Value2);

            Assert.AreEqual(3, ((LoaderTestElement)dictionary.Keys.First()).Value1);
            Assert.AreEqual(4, ((LoaderTestElement)dictionary.Keys.First()).Value2);
        }
    }
}
