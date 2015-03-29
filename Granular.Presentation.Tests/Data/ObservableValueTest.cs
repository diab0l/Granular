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

            observableValue.Value = "value1";
            Assert.AreEqual(1, valueChangedCount);

            observableValue.Value = "value1";
            Assert.AreEqual(1, valueChangedCount);

            observableValue.Value = "value1a";
            Assert.AreEqual(2, valueChangedCount);

            observableValue.Value = "value2";
            Assert.AreEqual(3, valueChangedCount);
        }

        [TestMethod]
        public void ReadOnlyObservableValueChangedTest()
        {
            int valueChangedCount = 0;
            ObservableValue observableValue = new ObservableValue();
            ReadOnlyObservableValue readOnlyObservableValue = new ReadOnlyObservableValue(observableValue);
            readOnlyObservableValue.ValueChanged += (sender, e) => valueChangedCount++;

            Assert.AreEqual(ObservableValue.UnsetValue, readOnlyObservableValue.Value);

            observableValue.Value = "value1";
            Assert.AreEqual(1, valueChangedCount);

            observableValue.Value = "value1";
            Assert.AreEqual(1, valueChangedCount);

            observableValue.Value = "value2";
            Assert.AreEqual(2, valueChangedCount);
        }
    }
}
