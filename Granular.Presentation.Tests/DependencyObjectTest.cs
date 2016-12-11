using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Data;
using Granular.Extensions;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class DependencyObjectTest
    {
        private class TestObject : DependencyObject
        {
            public event EventHandler Value1Changed;
            public event EventHandler Value2Changed;
            public event EventHandler Value3Changed;

            public static readonly DependencyProperty Value1Property = DependencyProperty.Register("Value1", typeof(string), typeof(TestObject), new PropertyMetadata(defaultValue: "defaultValue1", propertyChangedCallback: Value1PropertyChanged));
            public string Value1
            {
                get { return (string)GetValue(Value1Property); }
                set { SetValue(Value1Property, value); }
            }

            public static readonly DependencyProperty Value2Property = DependencyProperty.Register("Value2", typeof(string), typeof(TestObject), new PropertyMetadata(defaultValue: "defaultValue2", propertyChangedCallback: Value2PropertyChanged) { Inherits = true });
            public string Value2
            {
                get { return (string)GetValue(Value2Property); }
                set { SetValue(Value2Property, value); }
            }

            public static readonly DependencyProperty Value3Property = DependencyProperty.Register("Value3", typeof(int), typeof(TestObject), new PropertyMetadata(propertyChangedCallback: Value3PropertyChanged));
            public int Value3
            {
                get { return (int)GetValue(Value3Property); }
                set { SetValue(Value3Property, value); }
            }

            public void SetParent(TestObject parent)
            {
                SetInheritanceParent(parent);
            }

            private static void Value1PropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
            {
                if ((dependencyObject as TestObject).Value1Changed != null)
                {
                    (dependencyObject as TestObject).Value1Changed(dependencyObject as TestObject, e);
                }
            }

            private static void Value2PropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
            {
                if ((dependencyObject as TestObject).Value2Changed != null)
                {
                    (dependencyObject as TestObject).Value2Changed(dependencyObject as TestObject, e);
                }
            }

            private static void Value3PropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
            {
                if ((dependencyObject as TestObject).Value3Changed != null)
                {
                    (dependencyObject as TestObject).Value3Changed(dependencyObject as TestObject, e);
                }
            }
        }

        private class TestExpression : IExpressionProvider, IExpression
        {
            public event ObservableValueChangedEventHandler ValueChanged;

            public object Value { get { return observableValue.Value; } }

            private ObservableValue observableValue;
            private bool isReadOnly;

            public TestExpression(object value, bool isReadOnly = false)
            {
                observableValue = new ObservableValue(value);
                observableValue.ValueChanged += (sender, oldValue, newValue) => ValueChanged.Raise(this, oldValue, newValue);

                this.isReadOnly = isReadOnly;
            }

            public IExpression CreateExpression(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
            {
                return this;
            }

            public bool SetValue(object value)
            {
                if (isReadOnly)
                {
                    return false;
                }

                observableValue.BaseValue = value;
                return true;
            }
        }

        [TestMethod]
        public void SetCurrentValueOnLocalValue()
        {
            TestObject element = new TestObject();
            Assert.AreEqual("defaultValue1", element.GetValue(TestObject.Value1Property));

            element.SetValue(TestObject.Value1Property, "value1", BaseValueSource.Local);
            Assert.AreEqual("value1", element.GetValue(TestObject.Value1Property));
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: false, isExpression: false, baseValueSource: BaseValueSource.Local);

            element.SetCurrentValue(TestObject.Value1Property, "currentValue1");
            Assert.AreEqual("currentValue1", element.GetValue(TestObject.Value1Property));
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: true, isExpression: false, baseValueSource: BaseValueSource.Local);

            element.SetValue(TestObject.Value1Property, "value1b", BaseValueSource.Local);
            Assert.AreEqual("value1b", element.GetValue(TestObject.Value1Property));
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: false, isExpression: false, baseValueSource: BaseValueSource.Local);

            element.ClearValue(TestObject.Value1Property, BaseValueSource.Local);
            Assert.AreEqual("defaultValue1", element.GetValue(TestObject.Value1Property));
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: false, isExpression: false, baseValueSource: BaseValueSource.Default);
        }

        [TestMethod]
        public void SetCurrentValueOnStyleValue()
        {
            TestObject element = new TestObject();
            Assert.AreEqual("defaultValue1", element.GetValue(TestObject.Value1Property));

            element.SetValue(TestObject.Value1Property, "value1", BaseValueSource.Style);
            Assert.AreEqual("value1", element.GetValue(TestObject.Value1Property));
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: false, isExpression: false, baseValueSource: BaseValueSource.Style);

            element.SetCurrentValue(TestObject.Value1Property, "value2");
            Assert.AreEqual("value2", element.GetValue(TestObject.Value1Property));
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: true, isExpression: false, baseValueSource: BaseValueSource.Style);

            element.ClearValue(TestObject.Value1Property, BaseValueSource.Local);
            Assert.AreEqual("value1", element.GetValue(TestObject.Value1Property));
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: false, isExpression: false, baseValueSource: BaseValueSource.Style);
        }

        [TestMethod]
        public void SetCurrentValueOnLocalExpression()
        {
            TestObject element = new TestObject();
            Assert.AreEqual("defaultValue1", element.GetValue(TestObject.Value1Property));

            TestExpression expression = new TestExpression("value1", isReadOnly: false);
            Assert.AreEqual("value1", expression.Value);

            element.SetValue(TestObject.Value1Property, expression, BaseValueSource.Local);
            Assert.AreEqual("value1", element.GetValue(TestObject.Value1Property));
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: false, isExpression: true, baseValueSource: BaseValueSource.Local);

            element.SetCurrentValue(TestObject.Value1Property, "value2");
            Assert.AreEqual("value2", element.GetValue(TestObject.Value1Property));
            Assert.AreEqual("value2", expression.Value);
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: false, isExpression: true, baseValueSource: BaseValueSource.Local);

            element.ClearValue(TestObject.Value1Property, BaseValueSource.Local);
            Assert.AreEqual("defaultValue1", element.GetValue(TestObject.Value1Property));
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: false, isExpression: false, baseValueSource: BaseValueSource.Default);
        }

        [TestMethod]
        public void SetCurrentValueOnLocalReadOnlyExpression()
        {
            TestObject element = new TestObject();
            Assert.AreEqual("defaultValue1", element.GetValue(TestObject.Value1Property));

            element.SetValue(TestObject.Value1Property, new TestExpression("value1", isReadOnly: true), BaseValueSource.Local);
            Assert.AreEqual("value1", element.GetValue(TestObject.Value1Property));
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: false, isExpression: true, baseValueSource: BaseValueSource.Local);

            element.SetCurrentValue(TestObject.Value1Property, "value2");
            Assert.AreEqual("value2", element.GetValue(TestObject.Value1Property));
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: true, isExpression: true, baseValueSource: BaseValueSource.Local);

            element.ClearValue(TestObject.Value1Property, BaseValueSource.Local);
            Assert.AreEqual("defaultValue1", element.GetValue(TestObject.Value1Property));
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: false, isExpression: false, baseValueSource: BaseValueSource.Default);
        }

        [TestMethod]
        public void SetCurrentValueOnStyleExpression()
        {
            TestObject element = new TestObject();
            Assert.AreEqual("defaultValue1", element.GetValue(TestObject.Value1Property));

            TestExpression expression = new TestExpression("value1", isReadOnly: false);
            Assert.AreEqual("value1", expression.Value);

            element.SetValue(TestObject.Value1Property, expression, BaseValueSource.Style);
            Assert.AreEqual("value1", element.GetValue(TestObject.Value1Property));
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: false, isExpression: true, baseValueSource: BaseValueSource.Style);

            element.SetCurrentValue(TestObject.Value1Property, "value2");
            Assert.AreEqual("value2", element.GetValue(TestObject.Value1Property));
            Assert.AreEqual("value2", expression.Value);
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: false, isExpression: true, baseValueSource: BaseValueSource.Style);

            element.ClearValue(TestObject.Value1Property, BaseValueSource.Local);
            Assert.AreEqual("value2", element.GetValue(TestObject.Value1Property));
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: false, isExpression: true, baseValueSource: BaseValueSource.Style);
        }

        [TestMethod]
        public void SetCurrentValueOnStyleReadOnlyExpression()
        {
            TestObject element = new TestObject();
            Assert.AreEqual("defaultValue1", element.GetValue(TestObject.Value1Property));

            element.SetValue(TestObject.Value1Property, new TestExpression("value1", isReadOnly: true), BaseValueSource.Style);
            Assert.AreEqual("value1", element.GetValue(TestObject.Value1Property));
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: false, isExpression: true, baseValueSource: BaseValueSource.Style);

            element.SetCurrentValue(TestObject.Value1Property, "value2");
            Assert.AreEqual("value2", element.GetValue(TestObject.Value1Property));
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: true, isExpression: true, baseValueSource: BaseValueSource.Style);

            element.ClearValue(TestObject.Value1Property, BaseValueSource.Local);
            Assert.AreEqual("value1", element.GetValue(TestObject.Value1Property));
            AssertValueSource(element.GetValueSource(TestObject.Value1Property), isCurrent: false, isExpression: true, baseValueSource: BaseValueSource.Style);
        }

        [TestMethod]
        public void DependencyObjectValueInheritanceTest()
        {
            TestObject parent = new TestObject();
            TestObject child = new TestObject();

            child.SetParent(parent);

            parent.Value2 = "value2";

            Assert.AreEqual("value2", parent.Value2);
            Assert.AreEqual("value2", child.Value2);
        }

        [TestMethod]
        public void DependencyObjectInheritedValueChangedTest()
        {
            TestObject parent = new TestObject();
            int parentValue1Changed = 0;
            int parentValue2Changed = 0;
            int parentValue3Changed = 0;

            parent.Value1Changed += (sender, e) => parentValue1Changed++;
            parent.Value2Changed += (sender, e) => parentValue2Changed++;
            parent.Value3Changed += (sender, e) => parentValue3Changed++;

            TestObject child = new TestObject();
            int childValue2Changed = 0;

            child.Value2Changed += (sender, e) => childValue2Changed++;

            Assert.AreEqual("defaultValue1", parent.Value1);
            Assert.AreEqual("defaultValue2", parent.Value2);
            Assert.AreEqual(0, parent.Value3);

            parent.Value1 = "value1";
            parent.Value2 = "value2";
            parent.Value2 = "value2";
            parent.Value3 = 1;
            parent.Value3 = 1;

            Assert.AreEqual(1, parentValue1Changed);
            Assert.AreEqual(1, parentValue2Changed);
            Assert.AreEqual(1, parentValue3Changed);

            child.SetParent(parent);

            Assert.AreEqual(1, childValue2Changed);

            child.SetParent(null);

            Assert.AreEqual(2, childValue2Changed);

            child.Value2 = "value2a";

            Assert.AreEqual(3, childValue2Changed);

            child.SetParent(parent);

            Assert.AreEqual(3, childValue2Changed);

            child.ClearValue(TestObject.Value2Property);

            Assert.AreEqual(4, childValue2Changed);
            Assert.AreEqual(parent.Value2, child.Value2);

            child.SetParent(null);

            Assert.AreEqual(5, childValue2Changed);
            Assert.AreEqual("defaultValue2", child.Value2);

            parent.Value2 = "value2b";

            Assert.AreEqual(2, parentValue2Changed);
            Assert.AreEqual("value2b", parent.Value2);

            child.SetParent(parent);

            Assert.AreEqual(6, childValue2Changed);
            Assert.AreEqual(parent.Value2, child.Value2);
        }

        private static void AssertValueSource(ValueSource valueSource, bool isCurrent = false, bool isExpression = false, BaseValueSource baseValueSource = BaseValueSource.Local)
        {
            Assert.AreEqual(isCurrent, valueSource.IsCurrent);
            Assert.AreEqual(isExpression, valueSource.IsExpression);
            Assert.AreEqual(baseValueSource, valueSource.BaseValueSource);
        }
    }
}
