using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class DependencyPropertyCallbackTest
    {
        private class Element1 : DependencyObject
        {
            public static readonly DependencyProperty Value1Property = DependencyProperty.Register("Value1", typeof(int), typeof(Element1), new FrameworkPropertyMetadata(0, propertyChangedCallback: PropertyChangedCallback, coerceValueCallback: (d, baseValue) => CoerceValueCallback(d, Element1.Value1Property, baseValue)));
            public static readonly DependencyProperty Value1AttachedProperty = DependencyProperty.RegisterAttached("Value1Attached", typeof(int), typeof(Element1), new FrameworkPropertyMetadata(0, propertyChangedCallback: PropertyChangedCallback, coerceValueCallback: (d, baseValue) => CoerceValueCallback(d, Element1.Value1AttachedProperty, baseValue)));
            public static readonly DependencyProperty Value1InheritsProperty = DependencyProperty.Register("Value1Inherits", typeof(int), typeof(Element1), new FrameworkPropertyMetadata(0, inherits: true, propertyChangedCallback: PropertyChangedCallback, coerceValueCallback: (d, baseValue) => CoerceValueCallback(d, Element1.Value1InheritsProperty, baseValue)));
            public static readonly DependencyProperty Value1InheritsAttachedProperty = DependencyProperty.RegisterAttached("Value1InheritsAttached", typeof(int), typeof(Element1), new FrameworkPropertyMetadata(0, inherits: true, propertyChangedCallback: PropertyChangedCallback, coerceValueCallback: (d, baseValue) => CoerceValueCallback(d, Element1.Value1InheritsAttachedProperty, baseValue)));

            private HashSet<DependencyProperty> instancePropertyChanged;
            private HashSet<DependencyProperty> propertyChanged;
            private HashSet<DependencyProperty> propertyCoerced;

            public Element1()
            {
                instancePropertyChanged = new HashSet<DependencyProperty>();
                propertyChanged = new HashSet<DependencyProperty>();
                propertyCoerced = new HashSet<DependencyProperty>();
            }

            public void SetParent(DependencyObject parent)
            {
                base.SetInheritanceParent(parent);
            }

            protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
            {
                base.OnPropertyChanged(e);
                instancePropertyChanged.Add(e.Property);
            }

            protected static object CoerceValueCallback(DependencyObject d, DependencyProperty dependencyProperty, object baseValue)
            {
                ((Element1)d).propertyCoerced.Add(dependencyProperty);
                return baseValue;
            }

            protected static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                ((Element1)d).propertyChanged.Add(e.Property);
            }

            public void AssertValue(DependencyProperty property, object value)
            {
                Assert.AreEqual(value, GetValue(property));
            }

            public void AssertChanged(params DependencyProperty[] properties)
            {
                CollectionAssert.AreEquivalent(properties, propertyChanged.ToArray());
            }

            public void AssertCoerced(params DependencyProperty[] properties)
            {
                CollectionAssert.AreEquivalent(properties, propertyCoerced.ToArray());
            }

            public void AssertInstanceChanged(params DependencyProperty[] properties)
            {
                CollectionAssert.AreEquivalent(properties, instancePropertyChanged.ToArray());
            }

            public void ClearStatistics()
            {
                propertyChanged.Clear();
                propertyCoerced.Clear();
                instancePropertyChanged.Clear();
            }
        }

        private class Element2 : Element1
        {
            public static readonly DependencyProperty Value2Property = DependencyProperty.Register("Value2", typeof(int), typeof(Element2), new FrameworkPropertyMetadata(0, propertyChangedCallback: PropertyChangedCallback, coerceValueCallback: (d, baseValue) => CoerceValueCallback(d, Element2.Value2Property, baseValue)));
            public static readonly DependencyProperty Value2AttachedProperty = DependencyProperty.RegisterAttached("Value2Attached", typeof(int), typeof(Element2), new FrameworkPropertyMetadata(0, propertyChangedCallback: PropertyChangedCallback, coerceValueCallback: (d, baseValue) => CoerceValueCallback(d, Element2.Value2AttachedProperty, baseValue)));
            public static readonly DependencyProperty Value2InheritsProperty = DependencyProperty.Register("Value2Inherits", typeof(int), typeof(Element2), new FrameworkPropertyMetadata(0, inherits: true, propertyChangedCallback: PropertyChangedCallback, coerceValueCallback: (d, baseValue) => CoerceValueCallback(d, Element2.Value2InheritsProperty, baseValue)));
            public static readonly DependencyProperty Value2InheritsAttachedProperty = DependencyProperty.RegisterAttached("Value2InheritsAttached", typeof(int), typeof(Element2), new FrameworkPropertyMetadata(0, inherits: true, propertyChangedCallback: PropertyChangedCallback, coerceValueCallback: (d, baseValue) => CoerceValueCallback(d, Element2.Value2InheritsAttachedProperty, baseValue)));
        }

        [TestMethod]
        public void ContainedCallbackTest()
        {
            Element1 element1 = new Element1();
            Element2 element2 = new Element2();
            element2.SetParent(element1);

            element1.SetValue(Element1.Value1Property, 1);

            element1.AssertInstanceChanged(Element1.Value1Property);
            element1.AssertChanged(Element1.Value1Property);
            element1.AssertCoerced(Element1.Value1Property);
            element1.AssertValue(Element1.Value1Property, 1);

            element2.AssertInstanceChanged();
            element2.AssertChanged();
            element2.AssertCoerced();
            element2.AssertValue(Element1.Value1Property, 0);
        }

        [TestMethod]
        public void ContainedAttachedCallbackTest()
        {
            Element1 element1 = new Element1();
            Element2 element2 = new Element2();
            element2.SetParent(element1);

            element1.SetValue(Element1.Value1AttachedProperty, 1);

            element1.AssertInstanceChanged(Element1.Value1AttachedProperty);
            element1.AssertChanged(Element1.Value1AttachedProperty);
            element1.AssertCoerced(Element1.Value1AttachedProperty);
            element1.AssertValue(Element1.Value1AttachedProperty, 1);

            element2.AssertInstanceChanged();
            element2.AssertChanged();
            element2.AssertCoerced();
            element2.AssertValue(Element1.Value1AttachedProperty, 0);
        }

        [TestMethod]
        public void ContainedInheritsCallbackTest()
        {
            Element1 element1 = new Element1();
            Element2 element2 = new Element2();
            element2.SetParent(element1);

            element1.SetValue(Element1.Value1InheritsProperty, 1);

            element1.AssertInstanceChanged(Element1.Value1InheritsProperty);
            element1.AssertChanged(Element1.Value1InheritsProperty);
            element1.AssertCoerced(Element1.Value1InheritsProperty);
            element1.AssertValue(Element1.Value1InheritsProperty, 1);

            element2.AssertInstanceChanged(Element1.Value1InheritsProperty);
            element2.AssertChanged(Element1.Value1InheritsProperty);
            element2.AssertCoerced(Element1.Value1InheritsProperty);
            element2.AssertValue(Element1.Value1InheritsProperty, 1);
        }

        [TestMethod]
        public void ContainedInheritsAttachedCallbackTest()
        {
            Element1 element1 = new Element1();
            Element2 element2 = new Element2();
            element2.SetParent(element1);

            element1.SetValue(Element1.Value1InheritsAttachedProperty, 1);

            element1.AssertInstanceChanged(Element1.Value1InheritsAttachedProperty);
            element1.AssertChanged(Element1.Value1InheritsAttachedProperty);
            element1.AssertCoerced(Element1.Value1InheritsAttachedProperty);
            element1.AssertValue(Element1.Value1InheritsAttachedProperty, 1);

            element2.AssertInstanceChanged(Element1.Value1InheritsAttachedProperty);
            element2.AssertChanged(Element1.Value1InheritsAttachedProperty);
            element2.AssertCoerced(Element1.Value1InheritsAttachedProperty);
            element2.AssertValue(Element1.Value1InheritsAttachedProperty, 1);
        }

        [TestMethod]
        public void NonContainedCallbackTest()
        {
            Element1 element1 = new Element1();
            Element2 element2 = new Element2();
            element2.SetParent(element1);

            element1.SetValue(Element2.Value2Property, 1);

            element1.AssertInstanceChanged(Element2.Value2Property);
            element1.AssertChanged();
            element1.AssertCoerced();
            element1.AssertValue(Element2.Value2Property, 1);

            element2.AssertInstanceChanged();
            element2.AssertChanged();
            element2.AssertCoerced();
            element2.AssertValue(Element2.Value2Property, 0);
        }

        [TestMethod]
        public void NonContainedAttachedCallbackTest()
        {
            Element1 element1 = new Element1();
            Element2 element2 = new Element2();
            element2.SetParent(element1);

            element1.SetValue(Element2.Value2AttachedProperty, 1);

            element1.AssertInstanceChanged(Element2.Value2AttachedProperty);
            element1.AssertChanged(Element2.Value2AttachedProperty);
            element1.AssertCoerced(Element2.Value2AttachedProperty);
            element1.AssertValue(Element2.Value2AttachedProperty, 1);

            element2.AssertInstanceChanged();
            element2.AssertChanged();
            element2.AssertCoerced();
            element2.AssertValue(Element2.Value2AttachedProperty, 0);
        }

        [TestMethod]
        public void NonContainedInheritsCallbackTest()
        {
            Element1 element1 = new Element1();
            Element2 element2 = new Element2();
            element2.SetParent(element1);

            element1.SetValue(Element2.Value2InheritsProperty, 1);

            element1.AssertInstanceChanged(Element2.Value2InheritsProperty);
            element1.AssertChanged();
            element1.AssertCoerced();
            element1.AssertValue(Element2.Value2InheritsProperty, 1);

            // WPF has a bug here, while element2 value is changed (through inheritance), none of these callbacks are called
            element2.AssertInstanceChanged(Element2.Value2InheritsProperty);
            element2.AssertChanged(Element2.Value2InheritsProperty);
            element2.AssertCoerced(Element2.Value2InheritsProperty);
            element2.AssertValue(Element2.Value2InheritsProperty, 1);
        }

        [TestMethod]
        public void NonContainedInheritsAttachedCallbackTest()
        {
            Element1 element1 = new Element1();
            Element2 element2 = new Element2();
            element2.SetParent(element1);

            element1.SetValue(Element2.Value2InheritsAttachedProperty, 1);

            element1.AssertInstanceChanged(Element2.Value2InheritsAttachedProperty);
            element1.AssertChanged(Element2.Value2InheritsAttachedProperty);
            element1.AssertCoerced(Element2.Value2InheritsAttachedProperty);
            element1.AssertValue(Element2.Value2InheritsAttachedProperty, 1);

            element2.AssertInstanceChanged(Element2.Value2InheritsAttachedProperty);
            element2.AssertChanged(Element2.Value2InheritsAttachedProperty);
            element2.AssertCoerced(Element2.Value2InheritsAttachedProperty);
            element2.AssertValue(Element2.Value2InheritsAttachedProperty, 1);
        }
    }
}
