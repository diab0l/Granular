using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class DependencyPropertyTest
    {
        private class ElementA : DependencyObject
        {
            public static event EventHandler Value1ChangedA;
            public static event EventHandler Value2ChangedA;

            public static readonly DependencyProperty Value0Property = DependencyProperty.Register("Value0", typeof(int), typeof(ElementA));
            public static readonly DependencyProperty Value1Property = DependencyProperty.Register("Value1", typeof(int), typeof(ElementA), new PropertyMetadata(defaultValue: 1, propertyChangedCallback: (sender, e) => { if (Value1ChangedA != null) { Value1ChangedA(sender, e); } }));
            public static readonly DependencyProperty Value2Property = DependencyProperty.RegisterAttached("Value2", typeof(int), typeof(ElementA), new PropertyMetadata(defaultValue: 1, propertyChangedCallback: (sender, e) => { if (Value2ChangedA != null) { Value2ChangedA(sender, e); } }));
            public static readonly DependencyProperty Value3Property = DependencyProperty.Register("Value3", typeof(int), typeof(ElementA), new PropertyMetadata("1"));
        }

        private class ElementB : ElementA
        {
            public static readonly DependencyProperty Value0Property2 = DependencyProperty.Register("Value0", typeof(int), typeof(ElementB));

            public static event EventHandler Value1ChangedB;
            public static event EventHandler Value2ChangedB;

            static ElementB()
            {
                ElementA.Value1Property.OverrideMetadata(typeof(ElementB), new PropertyMetadata(defaultValue: 2, propertyChangedCallback: (sender, e) => { if (Value1ChangedB != null) { Value1ChangedB(sender, e); } }));
                ElementA.Value2Property.OverrideMetadata(typeof(ElementB), new PropertyMetadata(defaultValue: 2, propertyChangedCallback: (sender, e) => { if (Value2ChangedB != null) { Value2ChangedB(sender, e); } }));
            }
        }

        private class ElementC : ElementB
        {
            public static event EventHandler Value1ChangedC;
            public static event EventHandler Value2ChangedC;

            static ElementC()
            {
                ElementA.Value1Property.OverrideMetadata(typeof(ElementC), new PropertyMetadata(defaultValue: 3, propertyChangedCallback: (sender, e) => { if (Value1ChangedC != null) { Value1ChangedC(sender, e); } }));
                ElementA.Value2Property.OverrideMetadata(typeof(ElementC), new PropertyMetadata(defaultValue: 3, propertyChangedCallback: (sender, e) => { if (Value2ChangedC != null) { Value2ChangedC(sender, e); } }));
            }
        }

        private class ElementD : DependencyObject
        {
            public static event EventHandler Value1ChangedD;
            public static event EventHandler Value2ChangedD;

            static ElementD()
            {
                ElementA.Value0Property.AddOwner(typeof(ElementD));
                ElementA.Value1Property.AddOwner(typeof(ElementD));
                ElementA.Value1Property.OverrideMetadata(typeof(ElementD), new PropertyMetadata(defaultValue: 4, propertyChangedCallback: (sender, e) => { if (Value1ChangedD != null) { Value1ChangedD(sender, e); } }));
                ElementA.Value2Property.OverrideMetadata(typeof(ElementD), new PropertyMetadata(defaultValue: 4, propertyChangedCallback: (sender, e) => { if (Value2ChangedD != null) { Value2ChangedD(sender, e); } }));
            }
        }

        private class ReadOnlyElement : DependencyObject
        {
            public static readonly DependencyPropertyKey _private_Value1Key = DependencyProperty.RegisterReadOnly("Value1", typeof(int), typeof(ReadOnlyElement), new PropertyMetadata());
            public static readonly DependencyProperty Value1Property = _private_Value1Key.DependencyProperty;
            public int Value1
            {
                get { return (int)GetValue(Value1Property); }
            }
        }

        [TestMethod]
        public void RegisterTypeDefaultValueTest()
        {
            ElementA element = new ElementA();

            Assert.AreEqual(0, ElementA.Value0Property.GetMetadata(typeof(ElementA)).DefaultValue);
            Assert.AreEqual(0, element.GetValue(ElementA.Value0Property));
        }


        [TestMethod]
        public void RegisterTypeDefaultValueConversionTest()
        {
            ElementA element = new ElementA();

            Assert.AreEqual(1, ElementA.Value3Property.GetMetadata(typeof(ElementA)).DefaultValue);
            Assert.AreEqual(1, element.GetValue(ElementA.Value3Property));
        }

        [TestMethod]
        public void OverrideMetadataCallbackTest()
        {
            ElementA element1 = new ElementA();
            ElementB element2 = new ElementB();
            ElementC element3 = new ElementC();
            ElementD element4 = new ElementD();

            int changeIndex = 1;
            int value1AChangedIndex = 0;
            int value1BChangedIndex = 0;
            int value1CChangedIndex = 0;
            int value1DChangedIndex = 0;

            ElementA.Value1ChangedA += (sender, e) => value1AChangedIndex = changeIndex++;
            ElementB.Value1ChangedB += (sender, e) => value1BChangedIndex = changeIndex++;
            ElementC.Value1ChangedC += (sender, e) => value1CChangedIndex = changeIndex++;
            ElementD.Value1ChangedD += (sender, e) => value1DChangedIndex = changeIndex++;

            element1.SetValue(ElementA.Value1Property, 100);

            Assert.AreEqual(1, value1AChangedIndex);
            Assert.AreEqual(0, value1BChangedIndex);
            Assert.AreEqual(0, value1CChangedIndex);
            Assert.AreEqual(0, value1DChangedIndex);

            element2.SetValue(ElementA.Value1Property, 100);

            Assert.AreEqual(2, value1AChangedIndex);
            Assert.AreEqual(3, value1BChangedIndex);
            Assert.AreEqual(0, value1CChangedIndex);
            Assert.AreEqual(0, value1DChangedIndex);

            element3.SetValue(ElementA.Value1Property, 100);

            Assert.AreEqual(4, value1AChangedIndex);
            Assert.AreEqual(5, value1BChangedIndex);
            Assert.AreEqual(6, value1CChangedIndex);
            Assert.AreEqual(0, value1DChangedIndex);

            element4.SetValue(ElementA.Value1Property, 100);

            Assert.AreEqual(4, value1AChangedIndex);
            Assert.AreEqual(5, value1BChangedIndex);
            Assert.AreEqual(6, value1CChangedIndex);
            Assert.AreEqual(7, value1DChangedIndex);
        }

        [TestMethod]
        public void AttachedOverrideMetadataCallbackTest()
        {
            ElementA element1 = new ElementA();
            ElementB element2 = new ElementB();
            ElementC element3 = new ElementC();
            ElementD element4 = new ElementD();

            int changeIndex = 1;
            int value2AChangedIndex = 0;
            int value2BChangedIndex = 0;
            int value2CChangedIndex = 0;
            int value2DChangedIndex = 0;

            ElementA.Value2ChangedA += (sender, e) => value2AChangedIndex = changeIndex++;
            ElementB.Value2ChangedB += (sender, e) => value2BChangedIndex = changeIndex++;
            ElementC.Value2ChangedC += (sender, e) => value2CChangedIndex = changeIndex++;
            ElementD.Value2ChangedD += (sender, e) => value2DChangedIndex = changeIndex++;

            element1.SetValue(ElementA.Value2Property, 100);

            Assert.AreEqual(1, value2AChangedIndex);
            Assert.AreEqual(0, value2BChangedIndex);
            Assert.AreEqual(0, value2CChangedIndex);
            Assert.AreEqual(0, value2DChangedIndex);

            element2.SetValue(ElementA.Value2Property, 100);

            Assert.AreEqual(2, value2AChangedIndex);
            Assert.AreEqual(3, value2BChangedIndex);
            Assert.AreEqual(0, value2CChangedIndex);
            Assert.AreEqual(0, value2DChangedIndex);

            element3.SetValue(ElementA.Value2Property, 100);

            Assert.AreEqual(4, value2AChangedIndex);
            Assert.AreEqual(5, value2BChangedIndex);
            Assert.AreEqual(6, value2CChangedIndex);
            Assert.AreEqual(0, value2DChangedIndex);

            element4.SetValue(ElementA.Value2Property, 100);

            Assert.AreEqual(7, value2AChangedIndex);
            Assert.AreEqual(5, value2BChangedIndex);
            Assert.AreEqual(6, value2CChangedIndex);
            Assert.AreEqual(8, value2DChangedIndex);
        }

        [TestMethod]
        public void OverrideMetadataDefaultValueTest()
        {
            ElementA element1 = new ElementA();
            ElementB element2 = new ElementB();
            ElementC element3 = new ElementC();
            ElementD element4 = new ElementD();

            int propertyChangedCount = 0;

            element1.PropertyChanged += (sender, e) => propertyChangedCount++;
            element2.PropertyChanged += (sender, e) => propertyChangedCount++;
            element3.PropertyChanged += (sender, e) => propertyChangedCount++;
            element4.PropertyChanged += (sender, e) => propertyChangedCount++;

            Assert.AreEqual(1, element1.GetValue(ElementA.Value1Property));
            Assert.AreEqual(2, element2.GetValue(ElementA.Value1Property));
            Assert.AreEqual(3, element3.GetValue(ElementA.Value1Property));
            Assert.AreEqual(4, element4.GetValue(ElementA.Value1Property));

            Assert.AreEqual(1, element1.GetValue(ElementA.Value2Property));
            Assert.AreEqual(2, element2.GetValue(ElementA.Value2Property));
            Assert.AreEqual(3, element3.GetValue(ElementA.Value2Property));
            Assert.AreEqual(4, element4.GetValue(ElementA.Value2Property));

            element1.SetValue(ElementA.Value1Property, 1);
            element2.SetValue(ElementA.Value1Property, 2);
            element3.SetValue(ElementA.Value1Property, 3);
            element4.SetValue(ElementA.Value1Property, 4);

            Assert.AreEqual(0, propertyChangedCount);

            element1.SetValue(ElementA.Value1Property, 100);

            Assert.AreEqual(1, propertyChangedCount);

            element1.SetValue(ElementA.Value2Property, 1);
            element2.SetValue(ElementA.Value2Property, 2);
            element3.SetValue(ElementA.Value2Property, 3);
            element4.SetValue(ElementA.Value2Property, 4);

            Assert.AreEqual(1, propertyChangedCount);

            element1.SetValue(ElementA.Value2Property, 100);

            Assert.AreEqual(2, propertyChangedCount);
        }

        [TestMethod]
        public void DependencyPropertyAmbiguityTest()
        {
            ElementB element = new ElementB();

            element.SetValue(ElementA.Value0Property, 1);
            element.SetValue(ElementB.Value0Property2, 2);

            Assert.AreEqual(1, element.GetValue(ElementA.Value0Property));
            Assert.AreEqual(2, element.GetValue(ElementB.Value0Property2));

            Assert.AreEqual(ElementA.Value0Property, DependencyProperty.GetProperty(typeof(ElementA), "Value0"));
            Assert.AreEqual(ElementB.Value0Property2, DependencyProperty.GetProperty(typeof(ElementB), "Value0"));
        }

        [TestMethod]
        public void ReadOnlyDependencyPropertiesTest()
        {
            ReadOnlyElement element = new ReadOnlyElement();
            Assert.AreEqual(0, element.Value1);

            element.SetValue(ReadOnlyElement._private_Value1Key, 1);
            Assert.AreEqual(1, element.Value1);

            try
            {
                element.SetValue(ReadOnlyElement.Value1Property, 2);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is Exception);
            }

            try
            {
                IDependencyPropertyValueEntry entry = element.GetValueEntry(ReadOnlyElement.Value1Property);
                entry.SetBaseValue((int)BaseValueSource.Local, 3);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is Exception);
            }

            try
            {
                DependencyPropertyKey fakeKey = new DependencyPropertyKey(ReadOnlyElement.Value1Property);
                element.SetValue(fakeKey, 4);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is Exception);
            }

            try
            {
                DependencyPropertyKey fakeKey = DependencyProperty.RegisterReadOnly("Value1", typeof(int), typeof(ReadOnlyElement), new PropertyMetadata());
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is Exception);
            }
        }
    }
}
