using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Granular.Extensions;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class DependencyPropertyValueEntryTest
    {
        private class TestObject : DependencyObject
        {
            public const int InvalidValue = 1234;
            public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(TestObject), new PropertyMetadata());
        }

        [TestMethod]
        public void SetValueTest()
        {
            DependencyPropertyValueEntry entry = new DependencyPropertyValueEntry(new TestObject(), TestObject.ValueProperty, null);

            int valueChangedCount = 0;
            entry.ValueChanged += (sender, e) => valueChangedCount++;

            entry.SetValue(1, "value1");
            Assert.AreEqual("value1", entry.Value);
            Assert.AreEqual(1, entry.ValuePriority);
            Assert.AreEqual(1, valueChangedCount);
        }

        [TestMethod]
        public void SetHiddenValueTest()
        {
            DependencyPropertyValueEntry entry = new DependencyPropertyValueEntry(new TestObject(), TestObject.ValueProperty, null);

            int valueChangedCount = 0;
            entry.ValueChanged += (sender, e) => valueChangedCount++;

            entry.SetValue(2, "value1");
            Assert.AreEqual("value1", entry.Value);
            Assert.AreEqual(2, entry.ValuePriority);
            Assert.AreEqual(1, valueChangedCount);

            entry.SetValue(1, "value2");
            Assert.AreEqual("value1", entry.Value);
            Assert.AreEqual(2, entry.ValuePriority);
            Assert.AreEqual(1, valueChangedCount);
        }

        [TestMethod]
        public void SetHidingValueTest()
        {
            DependencyPropertyValueEntry entry = new DependencyPropertyValueEntry(new TestObject(), TestObject.ValueProperty, null);

            int valueChangedCount = 0;
            entry.ValueChanged += (sender, e) => valueChangedCount++;

            entry.SetValue(2, "value1");
            Assert.AreEqual("value1", entry.Value);
            Assert.AreEqual(2, entry.ValuePriority);
            Assert.AreEqual(1, valueChangedCount);

            entry.SetValue(3, "value1");
            Assert.AreEqual("value1", entry.Value);
            Assert.AreEqual(3, entry.ValuePriority);
            Assert.AreEqual(1, valueChangedCount);

            entry.SetValue(3, "value2");
            Assert.AreEqual("value2", entry.Value);
            Assert.AreEqual(3, entry.ValuePriority);
            Assert.AreEqual(2, valueChangedCount);
        }

        [TestMethod]
        public void SetInvalidValueTest()
        {
            DependencyPropertyValueEntry entry = new DependencyPropertyValueEntry(new TestObject(), TestObject.ValueProperty, null);

            int valueChangedCount = 0;
            entry.ValueChanged += (sender, e) => valueChangedCount++;

            entry.SetValue(1, "value1");
            entry.SetValue(2, "value2");
            Assert.AreEqual("value2", entry.Value);
            Assert.AreEqual(2, entry.ValuePriority);
            Assert.AreEqual(2, valueChangedCount);

            entry.SetValue(3, TestObject.InvalidValue);
            Assert.AreEqual("value2", entry.Value);
            Assert.AreEqual(2, entry.ValuePriority);
            Assert.AreEqual(2, valueChangedCount);

            entry.SetValue(2, TestObject.InvalidValue);
            Assert.AreEqual("value1", entry.Value);
            Assert.AreEqual(1, entry.ValuePriority);
            Assert.AreEqual(3, valueChangedCount);
        }

        [TestMethod]
        public void ClearValueTest()
        {
            DependencyPropertyValueEntry entry = new DependencyPropertyValueEntry(new TestObject(), TestObject.ValueProperty, null);

            int valueChangedCount = 0;
            entry.ValueChanged += (sender, e) => valueChangedCount++;

            entry.SetValue(2, "value1");
            Assert.AreEqual("value1", entry.Value);
            Assert.AreEqual(2, entry.ValuePriority);
            Assert.AreEqual(1, valueChangedCount);

            entry.SetValue(2, ObservableValue.UnsetValue);
            Assert.AreEqual(ObservableValue.UnsetValue, entry.Value);
            Assert.AreEqual(0, entry.ValuePriority);
            Assert.AreEqual(2, valueChangedCount);
        }

        [TestMethod]
        public void ClearHiddenValueTest()
        {
            DependencyPropertyValueEntry entry = new DependencyPropertyValueEntry(new TestObject(), TestObject.ValueProperty, null);

            int valueChangedCount = 0;
            entry.ValueChanged += (sender, e) => valueChangedCount++;

            entry.SetValue(2, "value1");
            entry.SetValue(1, "value2");
            Assert.AreEqual("value1", entry.Value);
            Assert.AreEqual(2, entry.ValuePriority);
            Assert.AreEqual(1, valueChangedCount);

            entry.SetValue(2, ObservableValue.UnsetValue);
            entry.SetValue(1, "value2");
            Assert.AreEqual("value2", entry.Value);
            Assert.AreEqual(1, entry.ValuePriority);
            Assert.AreEqual(2, valueChangedCount);
        }

        [TestMethod]
        public void ClearHidingValueTest()
        {
            DependencyPropertyValueEntry entry = new DependencyPropertyValueEntry(new TestObject(), TestObject.ValueProperty, null);

            int valueChangedCount = 0;
            entry.ValueChanged += (sender, e) => valueChangedCount++;

            entry.SetValue(1, "value1");
            entry.SetValue(2, "value2");
            Assert.AreEqual("value2", entry.Value);
            Assert.AreEqual(2, entry.ValuePriority);
            Assert.AreEqual(2, valueChangedCount);

            entry.SetValue(2, ObservableValue.UnsetValue);
            Assert.AreEqual("value1", entry.Value);
            Assert.AreEqual(1, entry.ValuePriority);
            Assert.AreEqual(3, valueChangedCount);
        }

        [TestMethod]
        public void GetBaseValuePriorityTest()
        {
            DependencyPropertyValueEntry entry = new DependencyPropertyValueEntry(new TestObject(), TestObject.ValueProperty, null);

            int valueChangedCount = 0;
            entry.ValueChanged += (sender, e) => valueChangedCount++;

            entry.SetValue(1, "value1");
            Assert.AreEqual("value1", entry.Value);
            Assert.AreEqual(1, entry.ValuePriority);
            Assert.AreEqual(1, entry.GetBaseValuePriority());
            Assert.AreEqual(1, valueChangedCount);

            entry.SetValue(2, "value2");
            Assert.AreEqual("value2", entry.Value);
            Assert.AreEqual(2, entry.ValuePriority);
            Assert.AreEqual(2, entry.GetBaseValuePriority());
            Assert.AreEqual(2, valueChangedCount);

            entry.SetValue(DependencyPropertyValueEntry.BaseValueHighestPriority, "value3");
            Assert.AreEqual("value3", entry.Value);
            Assert.AreEqual(DependencyPropertyValueEntry.BaseValueHighestPriority, entry.ValuePriority);
            Assert.AreEqual(DependencyPropertyValueEntry.BaseValueHighestPriority, entry.GetBaseValuePriority());
            Assert.AreEqual(3, valueChangedCount);

            entry.SetValue(DependencyPropertyValueEntry.BaseValueHighestPriority + 1, "value4");
            Assert.AreEqual("value4", entry.Value);
            Assert.AreEqual(DependencyPropertyValueEntry.BaseValueHighestPriority + 1, entry.ValuePriority);
            Assert.AreEqual(DependencyPropertyValueEntry.BaseValueHighestPriority, entry.GetBaseValuePriority());
            Assert.AreEqual(4, valueChangedCount);

            entry.SetValue(DependencyPropertyValueEntry.BaseValueHighestPriority, TestObject.InvalidValue);
            Assert.AreEqual("value4", entry.Value);
            Assert.AreEqual(DependencyPropertyValueEntry.BaseValueHighestPriority + 1, entry.ValuePriority);
            Assert.AreEqual(2, entry.GetBaseValuePriority());
            Assert.AreEqual(4, valueChangedCount);

            entry.SetValue(DependencyPropertyValueEntry.BaseValueHighestPriority + 1, TestObject.InvalidValue);
            Assert.AreEqual("value2", entry.Value);
            Assert.AreEqual(2, entry.ValuePriority);
            Assert.AreEqual(2, entry.GetBaseValuePriority());
            Assert.AreEqual(5, valueChangedCount);
        }

        [TestMethod]
        public void CoerceValueTest()
        {
            string coercionSuffix = "-coerced";
            DependencyPropertyValueEntry entry = new DependencyPropertyValueEntry(new TestObject(), TestObject.ValueProperty, (dependencyObject, value) => (string)value + coercionSuffix);

            int valueChangedCount = 0;
            entry.ValueChanged += (sender, e) => valueChangedCount++;

            entry.SetValue(1, "value1");
            Assert.AreEqual("value1-coerced", entry.Value);
            Assert.AreEqual(1, entry.ValuePriority);
            Assert.AreEqual(1, valueChangedCount);

            entry.CoerceValue();
            Assert.AreEqual("value1-coerced", entry.Value);
            Assert.AreEqual(1, entry.ValuePriority);
            Assert.AreEqual(1, valueChangedCount);

            coercionSuffix = "-coerced2";
            entry.CoerceValue();
            Assert.AreEqual("value1-coerced2", entry.Value);
            Assert.AreEqual(1, entry.ValuePriority);
            Assert.AreEqual(2, valueChangedCount);
        }

        [TestMethod]
        public void SetObservableValueTest()
        {
            DependencyPropertyValueEntry entry = new DependencyPropertyValueEntry(new TestObject(), TestObject.ValueProperty, null);

            int valueChangedCount = 0;
            entry.ValueChanged += (sender, e) => valueChangedCount++;

            ObservableValue observableValue1 = new ObservableValue();
            ObservableValue innerObservableValue = new ObservableValue();

            entry.SetValue(1, "value1");
            entry.SetValue(2, observableValue1);
            Assert.AreEqual("value1", entry.Value);
            Assert.AreEqual(1, entry.ValuePriority);
            Assert.AreEqual(1, valueChangedCount);

            observableValue1.BaseValue = "value2";
            Assert.AreEqual("value2", entry.Value);
            Assert.AreEqual(2, entry.ValuePriority);
            Assert.AreEqual(2, valueChangedCount);

            observableValue1.BaseValue = innerObservableValue;
            Assert.AreEqual("value1", entry.Value);
            Assert.AreEqual(1, entry.ValuePriority);
            Assert.AreEqual(3, valueChangedCount);

            innerObservableValue.BaseValue = "value3";
            Assert.AreEqual("value3", entry.Value);
            Assert.AreEqual(2, entry.ValuePriority);
            Assert.AreEqual(4, valueChangedCount);

            innerObservableValue.BaseValue = ObservableValue.UnsetValue;
            Assert.AreEqual("value1", entry.Value);
            Assert.AreEqual(1, entry.ValuePriority);
            Assert.AreEqual(5, valueChangedCount);
        }
    }
}