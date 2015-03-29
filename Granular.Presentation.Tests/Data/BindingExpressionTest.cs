using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Data
{
    [TestClass]
    public class BindingExpressionTest
    {
        private class ConversionCounter : IValueConverter
        {
            public int ConvertedCount { get; private set; }
            public int ConvertedBackCount { get; private set; }

            public object Convert(object value, Type targetType, object parameter)
            {
                ConvertedCount++;
                return value;
            }

            public object ConvertBack(object value, Type targetType, object parameter)
            {
                ConvertedBackCount++;
                return value;
            }
        }

        [TestMethod]
        public void TwoWayBindingTest()
        {
            ObservableNode sourceRoot = new ObservableNode();
            ObservableNode source = new ObservableNode { Value = 1 };

            DependencyObjectNode target = new DependencyObjectNode();

            int sourcePropertyChangedCount = 0;
            int targetPropertyChangedCount = 0;

            source.PropertyChanged += (sender, e) => sourcePropertyChangedCount++;
            target.PropertyChanged += (sender, e) => targetPropertyChangedCount++;

            ConversionCounter conversionCounter = new ConversionCounter();
            Binding binding = new Binding { Source = sourceRoot, Path = PropertyPath.Parse("Child.Value"), Mode = BindingMode.TwoWay, Converter = conversionCounter };

            Assert.AreEqual(0, target.Value_ClrWrapper);

            sourceRoot.Child = source;

            target.SetValue(DependencyObjectNode.ValueProperty, binding);

            Assert.AreEqual(1, target.Value_ClrWrapper);
            Assert.AreEqual(1, targetPropertyChangedCount);
            Assert.AreEqual(1, conversionCounter.ConvertedCount);
            Assert.AreEqual(0, conversionCounter.ConvertedBackCount);

            source.Value = 2;

            Assert.AreEqual(2, target.Value_ClrWrapper);
            Assert.AreEqual(1, sourcePropertyChangedCount);
            Assert.AreEqual(2, targetPropertyChangedCount);
            Assert.AreEqual(2, conversionCounter.ConvertedCount);
            Assert.AreEqual(0, conversionCounter.ConvertedBackCount);

            target.Value_ClrWrapper = 3;

            Assert.AreEqual(3, source.Value);
            Assert.AreEqual(2, sourcePropertyChangedCount);
            Assert.AreEqual(3, targetPropertyChangedCount);
            Assert.AreEqual(2, conversionCounter.ConvertedCount);
            Assert.AreEqual(1, conversionCounter.ConvertedBackCount);

            sourceRoot.Child = null;

            Assert.AreEqual(0, target.Value_ClrWrapper);
            Assert.AreEqual(4, targetPropertyChangedCount);
            Assert.AreEqual(2, conversionCounter.ConvertedCount);
            Assert.AreEqual(1, conversionCounter.ConvertedBackCount);
        }

        [TestMethod]
        public void OneWayBindingTest()
        {
            ObservableNode sourceRoot = new ObservableNode();
            ObservableNode source = new ObservableNode { Value = 1 };

            DependencyObjectNode target = new DependencyObjectNode();

            int sourcePropertyChangedCount = 0;
            int targetPropertyChangedCount = 0;

            source.PropertyChanged += (sender, e) => sourcePropertyChangedCount++;
            target.PropertyChanged += (sender, e) => targetPropertyChangedCount++;

            ConversionCounter conversionCounter = new ConversionCounter();
            Binding binding = new Binding { Source = sourceRoot, Path = PropertyPath.Parse("Child.Value"), Mode = BindingMode.OneWay, Converter = conversionCounter };

            target.SetValue(DependencyObjectNode.ValueProperty, binding);

            Assert.AreEqual(0, target.Value_ClrWrapper);
            Assert.AreEqual(0, conversionCounter.ConvertedCount);
            Assert.AreEqual(0, conversionCounter.ConvertedBackCount);

            sourceRoot.Child = source;

            Assert.AreEqual(1, target.Value_ClrWrapper);
            Assert.AreEqual(1, targetPropertyChangedCount);
            Assert.AreEqual(1, conversionCounter.ConvertedCount);
            Assert.AreEqual(0, conversionCounter.ConvertedBackCount);

            source.Value = 2;

            Assert.AreEqual(2, target.Value_ClrWrapper);
            Assert.AreEqual(1, sourcePropertyChangedCount);
            Assert.AreEqual(2, targetPropertyChangedCount);
            Assert.AreEqual(2, conversionCounter.ConvertedCount);
            Assert.AreEqual(0, conversionCounter.ConvertedBackCount);

            target.Value_ClrWrapper = 3;

            Assert.AreEqual(2, source.Value);
            Assert.AreEqual(1, sourcePropertyChangedCount);
            Assert.AreEqual(3, targetPropertyChangedCount);
            Assert.AreEqual(2, conversionCounter.ConvertedCount);
            Assert.AreEqual(0, conversionCounter.ConvertedBackCount);

            sourceRoot.Child = null;

            Assert.AreEqual(0, target.Value_ClrWrapper);
            Assert.AreEqual(4, targetPropertyChangedCount);
            Assert.AreEqual(2, conversionCounter.ConvertedCount);
            Assert.AreEqual(0, conversionCounter.ConvertedBackCount);
        }

        [TestMethod]
        public void OneWayToSourceBindingTest()
        {
            ObservableNode source = new ObservableNode { Value = 1 };
            ObservableNode sourceRoot = new ObservableNode { Child = source };

            DependencyObjectNode target = new DependencyObjectNode();

            int sourcePropertyChangedCount = 0;
            int targetPropertyChangedCount = 0;

            source.PropertyChanged += (sender, e) => sourcePropertyChangedCount++;
            target.PropertyChanged += (sender, e) => targetPropertyChangedCount++;

            ConversionCounter conversionCounter = new ConversionCounter();
            Binding binding = new Binding { Source = sourceRoot, Path = PropertyPath.Parse("Child.Value"), Mode = BindingMode.OneWayToSource, Converter = conversionCounter };

            Assert.AreEqual(1, source.Value);

            target.SetValue(DependencyObjectNode.ValueProperty, binding);

            Assert.AreEqual(0, source.Value);
            Assert.AreEqual(0, target.Value_ClrWrapper);
            Assert.AreEqual(0, targetPropertyChangedCount);
            Assert.AreEqual(0, conversionCounter.ConvertedCount);
            Assert.AreEqual(1, conversionCounter.ConvertedBackCount);

            source.Value = 2;

            Assert.AreEqual(0, target.Value_ClrWrapper);
            Assert.AreEqual(2, sourcePropertyChangedCount);
            Assert.AreEqual(0, targetPropertyChangedCount);
            Assert.AreEqual(0, conversionCounter.ConvertedCount);
            Assert.AreEqual(1, conversionCounter.ConvertedBackCount);

            target.Value_ClrWrapper = 3;

            Assert.AreEqual(3, source.Value);
            Assert.AreEqual(3, sourcePropertyChangedCount);
            Assert.AreEqual(1, targetPropertyChangedCount);
            Assert.AreEqual(0, conversionCounter.ConvertedCount);
            Assert.AreEqual(2, conversionCounter.ConvertedBackCount);

            sourceRoot.Child = null;

            Assert.AreEqual(3, source.Value);
            Assert.AreEqual(3, sourcePropertyChangedCount);
            Assert.AreEqual(3, target.Value_ClrWrapper);
            Assert.AreEqual(1, targetPropertyChangedCount);
            Assert.AreEqual(0, conversionCounter.ConvertedCount);
            Assert.AreEqual(2, conversionCounter.ConvertedBackCount);

            target.Value_ClrWrapper = 4;
            Assert.AreEqual(3, source.Value);
            Assert.AreEqual(3, sourcePropertyChangedCount);
            Assert.AreEqual(4, target.Value_ClrWrapper);
            Assert.AreEqual(2, targetPropertyChangedCount);
            Assert.AreEqual(0, conversionCounter.ConvertedCount);
            Assert.AreEqual(3, conversionCounter.ConvertedBackCount);
        }

        [TestMethod]
        public void BindingFallbackValueTest()
        {
            ObservableNode sourceRoot = new ObservableNode();
            ObservableNode source = new ObservableNode();

            DependencyObjectNode target = new DependencyObjectNode();

            Binding binding = new Binding { Source = sourceRoot, Path = PropertyPath.Parse("Child.Value2"), Mode = BindingMode.TwoWay, FallbackValue = "fallback", TargetNullValue = "null" };

            target.SetValue(DependencyObjectNode.Value2Property, binding);

            Assert.AreEqual("fallback", target.Value2_ClrWrapper);

            sourceRoot.Child = source;

            Assert.AreEqual("null", target.Value2_ClrWrapper);

            source.Value2 = "value";

            Assert.AreEqual("value", target.Value2_ClrWrapper);
        }
    }
}
