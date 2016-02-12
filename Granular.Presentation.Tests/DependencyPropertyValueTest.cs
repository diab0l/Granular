using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class DependencyPropertyValueTest
    {
        private class TestObject : DependencyObject
        {
            public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(TestObject), new PropertyMetadata());
        }

        [TestMethod]
        public void GetBaseValueTest()
        {
            IDependencyPropertyValueEntry dependencyPropertyValue = new DependencyPropertyValueEntry(new DependencyObject(), TestObject.ValueProperty);

            dependencyPropertyValue.SetAnimationValue("animation1");

            dependencyPropertyValue.SetBaseValue(0, "value0");
            Assert.AreEqual("value0", dependencyPropertyValue.GetBaseValue(false));

            dependencyPropertyValue.SetBaseValue(2, "value2");
            Assert.AreEqual("value2", dependencyPropertyValue.GetBaseValue(false));

            dependencyPropertyValue.SetBaseValue(1, "value1");
            Assert.AreEqual("value2", dependencyPropertyValue.GetBaseValue(false));

            Assert.AreEqual("value0", dependencyPropertyValue.GetBaseValue(0, false));
            Assert.AreEqual("value1", dependencyPropertyValue.GetBaseValue(1, false));
            Assert.AreEqual("value2", dependencyPropertyValue.GetBaseValue(2, false));
        }

        [TestMethod]
        public void SetBaseValueTest()
        {
            int valueChangedCount = 0;
            IDependencyPropertyValueEntry dependencyPropertyValue = new DependencyPropertyValueEntry(new DependencyObject(), TestObject.ValueProperty);
            dependencyPropertyValue.ValueChanged += (sender, e) => valueChangedCount++;

            ObservableValue observableValue = new ObservableValue("value3");

            dependencyPropertyValue.SetBaseValue(0, "value0");
            Assert.AreEqual("value0", dependencyPropertyValue.GetBaseValue(false));
            Assert.AreEqual(1, valueChangedCount);

            dependencyPropertyValue.SetBaseValue(0, "value0a");
            Assert.AreEqual("value0a", dependencyPropertyValue.GetBaseValue(false));
            Assert.AreEqual(2, valueChangedCount);

            dependencyPropertyValue.SetBaseValue(2, "value2");
            Assert.AreEqual("value2", dependencyPropertyValue.GetBaseValue(false));
            Assert.AreEqual(3, valueChangedCount);

            dependencyPropertyValue.SetBaseValue(1, "value1");
            Assert.AreEqual("value2", dependencyPropertyValue.GetBaseValue(false));
            Assert.AreEqual(3, valueChangedCount);

            dependencyPropertyValue.SetBaseValue(3, observableValue);
            Assert.AreEqual(observableValue, dependencyPropertyValue.GetBaseValue(false));
            Assert.AreEqual(4, valueChangedCount);

            observableValue.BaseValue = "value3a";
            Assert.AreEqual(observableValue, dependencyPropertyValue.GetBaseValue(false));
            Assert.AreEqual(5, valueChangedCount);

            dependencyPropertyValue.SetBaseValue(4, "value4");
            Assert.AreEqual("value4", dependencyPropertyValue.GetBaseValue(false));
            Assert.AreEqual(6, valueChangedCount);

            observableValue.BaseValue = "value3b";
            Assert.AreEqual("value4", dependencyPropertyValue.GetBaseValue(false));
            Assert.AreEqual(6, valueChangedCount);
        }

        [TestMethod]
        public void ClearBaseValueTest()
        {
            int valueChangedCount = 0;
            IDependencyPropertyValueEntry dependencyPropertyValue = new DependencyPropertyValueEntry(new DependencyObject(), TestObject.ValueProperty);
            dependencyPropertyValue.ValueChanged += (sender, e) => valueChangedCount++;

            dependencyPropertyValue.SetBaseValue(0, "value0");
            dependencyPropertyValue.SetBaseValue(1, "value1");
            dependencyPropertyValue.SetBaseValue(2, "value2");
            Assert.AreEqual(3, valueChangedCount);

            dependencyPropertyValue.ClearBaseValue(1);
            Assert.AreEqual("value2", dependencyPropertyValue.GetBaseValue(false));
            Assert.AreEqual(3, valueChangedCount);

            dependencyPropertyValue.ClearBaseValue(2);
            Assert.AreEqual("value0", dependencyPropertyValue.GetBaseValue(false));
            Assert.AreEqual(4, valueChangedCount);
        }

        [TestMethod]
        public void GetAnimationValueTest()
        {
            IDependencyPropertyValueEntry dependencyPropertyValue = new DependencyPropertyValueEntry(new DependencyObject(), TestObject.ValueProperty);

            dependencyPropertyValue.SetAnimationValue("animation1");
            Assert.AreEqual("animation1", dependencyPropertyValue.GetAnimationValue(false));

            dependencyPropertyValue.SetAnimationValue("animation2");
            Assert.AreEqual("animation2", dependencyPropertyValue.GetAnimationValue(false));

            dependencyPropertyValue.SetAnimationValue("animation3");
            Assert.AreEqual("animation3", dependencyPropertyValue.Value);

            dependencyPropertyValue.SetAnimationValue("animation4");
            Assert.AreEqual("animation4", dependencyPropertyValue.Value);
        }

        [TestMethod]
        public void SetAnimationValueTest()
        {
            int valueChangedCount = 0;
            IDependencyPropertyValueEntry dependencyPropertyValue = new DependencyPropertyValueEntry(new DependencyObject(), TestObject.ValueProperty);
            dependencyPropertyValue.ValueChanged += (sender, e) => valueChangedCount++;

            dependencyPropertyValue.SetBaseValue(0, "base1");
            Assert.AreEqual("base1", dependencyPropertyValue.Value);
            Assert.AreEqual(1, valueChangedCount);

            dependencyPropertyValue.SetAnimationValue("animation1");
            Assert.AreEqual("animation1", dependencyPropertyValue.Value);
            Assert.AreEqual(2, valueChangedCount);

            dependencyPropertyValue.SetAnimationValue("animation2");
            Assert.AreEqual("animation2", dependencyPropertyValue.Value);
            Assert.AreEqual(3, valueChangedCount);

            dependencyPropertyValue.SetBaseValue(0, "base2");
            Assert.AreEqual("animation2", dependencyPropertyValue.Value);
            Assert.AreEqual(3, valueChangedCount);
        }

        [TestMethod]
        public void ClearAnimationValueTest()
        {
            int valueChangedCount = 0;
            IDependencyPropertyValueEntry dependencyPropertyValue = new DependencyPropertyValueEntry(new DependencyObject(), TestObject.ValueProperty);
            dependencyPropertyValue.ValueChanged += (sender, e) => valueChangedCount++;

            dependencyPropertyValue.SetBaseValue(0, "base1");

            dependencyPropertyValue.SetAnimationValue("animation1");
            Assert.AreEqual("animation1", dependencyPropertyValue.Value);
            Assert.AreEqual(2, valueChangedCount);

            dependencyPropertyValue.ClearAnimationValue();
            Assert.AreEqual("base1", dependencyPropertyValue.Value);
            Assert.AreEqual(3, valueChangedCount);
        }
    }

    [TestClass]
    public class CoercedDependencyPropertyValueTest
    {
        private class TestObject : DependencyObject
        {
            public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(TestObject), new PropertyMetadata());
        }

        private static object CoerceValueCallback(DependencyObject dependencyObject, object value)
        {
            return String.Format("{0}-coerced", value.ToString());
        }

        [TestMethod]
        public void GetValueTest()
        {
            IDependencyPropertyValueEntry dependencyPropertyValue = new DependencyPropertyValueEntry(new DependencyObject(), TestObject.ValueProperty);
            IDependencyPropertyValueEntry coercedDependencyPropertyValue = new CoercedDependencyPropertyValueEntry(dependencyPropertyValue, null, CoerceValueCallback);

            dependencyPropertyValue.SetBaseValue(0, "base0");
            Assert.AreEqual("base0-coerced", coercedDependencyPropertyValue.Value);
            Assert.AreEqual("base0", coercedDependencyPropertyValue.GetBaseValue(0, false));

            dependencyPropertyValue.SetBaseValue(1, "base1");
            Assert.AreEqual("base1-coerced", coercedDependencyPropertyValue.Value);
            Assert.AreEqual("base1", coercedDependencyPropertyValue.GetBaseValue(1, false));

            dependencyPropertyValue.SetAnimationValue("animation");
            Assert.AreEqual("animation-coerced", coercedDependencyPropertyValue.Value);
            Assert.AreEqual("animation", coercedDependencyPropertyValue.GetAnimationValue(false));
        }

        [TestMethod]
        public void SetValueTest()
        {
            int valueChangedCount = 0;
            IDependencyPropertyValueEntry dependencyPropertyValue = new DependencyPropertyValueEntry(new DependencyObject(), TestObject.ValueProperty);
            dependencyPropertyValue.ValueChanged += (sender, e) => valueChangedCount++;

            int coercedValueChangedCount = 0;
            IDependencyPropertyValueEntry coercedDependencyPropertyValue = new CoercedDependencyPropertyValueEntry(dependencyPropertyValue, null, CoerceValueCallback);
            coercedDependencyPropertyValue.ValueChanged += (sender, e) => coercedValueChangedCount++;

            dependencyPropertyValue.SetBaseValue(0, "base");
            Assert.AreEqual("base-coerced", coercedDependencyPropertyValue.Value);
            Assert.AreEqual(1, valueChangedCount);
            Assert.AreEqual(1, coercedValueChangedCount);

            dependencyPropertyValue.SetAnimationValue("animation");
            Assert.AreEqual("animation-coerced", coercedDependencyPropertyValue.Value);
            Assert.AreEqual(2, valueChangedCount);
            Assert.AreEqual(2, coercedValueChangedCount);
        }

        [TestMethod]
        public void ClearValueTest()
        {
            int valueChangedCount = 0;
            IDependencyPropertyValueEntry dependencyPropertyValue = new DependencyPropertyValueEntry(new DependencyObject(), TestObject.ValueProperty);
            dependencyPropertyValue.ValueChanged += (sender, e) => valueChangedCount++;

            int coercedValueChangedCount = 0;
            IDependencyPropertyValueEntry coercedDependencyPropertyValue = new CoercedDependencyPropertyValueEntry(dependencyPropertyValue, null, CoerceValueCallback);
            coercedDependencyPropertyValue.ValueChanged += (sender, e) => coercedValueChangedCount++;

            dependencyPropertyValue.SetBaseValue(0, "base0");
            dependencyPropertyValue.SetBaseValue(1, "base1");
            dependencyPropertyValue.SetAnimationValue("animation");

            Assert.AreEqual("animation-coerced", coercedDependencyPropertyValue.Value);
            Assert.AreEqual(3, valueChangedCount);
            Assert.AreEqual(3, coercedValueChangedCount);

            dependencyPropertyValue.ClearBaseValue(1);
            Assert.AreEqual("animation-coerced", coercedDependencyPropertyValue.Value);
            Assert.AreEqual(3, valueChangedCount);
            Assert.AreEqual(3, coercedValueChangedCount);

            dependencyPropertyValue.ClearAnimationValue();
            Assert.AreEqual("base0-coerced", coercedDependencyPropertyValue.Value);
            Assert.AreEqual(4, valueChangedCount);
            Assert.AreEqual(4, coercedValueChangedCount);
        }
    }
}
