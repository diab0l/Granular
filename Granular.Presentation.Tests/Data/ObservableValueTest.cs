using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Data
{
    [TestClass]
    public class ObservableValueTest
    {
        [TestMethod]
        public void ObservableValueChangedTest()
        {
            int valueChangedCount = 0;
            ObservableValue observableValue = new ObservableValue();
            observableValue.ValueChanged += (sender, e) => valueChangedCount++;

            Assert.AreEqual(ObservableValue.UnsetValue, observableValue.Value);
            Assert.IsTrue(ObservableValue.IsNullOrUnset(observableValue.Value));

            observableValue.BaseValue = "value1";
            Assert.AreEqual(1, valueChangedCount);

            observableValue.BaseValue = "value1";
            Assert.AreEqual(1, valueChangedCount);

            observableValue.BaseValue = "value1a";
            Assert.AreEqual(2, valueChangedCount);

            observableValue.BaseValue = "value2";
            Assert.AreEqual(3, valueChangedCount);

            Assert.IsFalse(ObservableValue.IsNullOrUnset(observableValue.Value));
        }

        [TestMethod]
        public void ObservableValueBaseChangedTest()
        {
            int valueChangedCount = 0;
            ObservableValue observableValue = new ObservableValue();
            observableValue.ValueChanged += (sender, e) => valueChangedCount++;

            Assert.AreEqual(ObservableValue.UnsetValue, observableValue.Value);
            Assert.IsTrue(ObservableValue.IsNullOrUnset(observableValue.Value));

            ObservableValue baseObservableValue = new ObservableValue();
            observableValue.BaseValue = baseObservableValue;
            Assert.AreEqual(0, valueChangedCount);

            baseObservableValue.BaseValue = "value1";
            Assert.AreEqual(1, valueChangedCount);

            baseObservableValue.BaseValue = "value1";
            Assert.AreEqual(1, valueChangedCount);

            baseObservableValue.BaseValue = "value1a";
            Assert.AreEqual(2, valueChangedCount);

            observableValue.BaseValue = "value2";
            Assert.AreEqual(3, valueChangedCount);

            baseObservableValue.BaseValue = "value1b";
            Assert.AreEqual(3, valueChangedCount);
        }
    }
}
