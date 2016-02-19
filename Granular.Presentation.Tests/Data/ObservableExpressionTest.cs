using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Granular.Collections;
using Granular.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Data
{
    public class ObservableNode : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int value;
        public int Value
        {
            get { return value; }
            set
            {
                if (this.value == value)
                {
                    return;
                }

                this.value = value;
                PropertyChanged.Raise(this, new PropertyChangedEventArgs("Value"));
            }
        }

        private string value2;
        public string Value2
        {
            get { return value2; }
            set
            {
                if (this.value2 == value)
                {
                    return;
                }

                this.value2 = value;
                PropertyChanged.Raise(this, new PropertyChangedEventArgs("Value2"));
            }
        }

        private ObservableNode child;
        public ObservableNode Child
        {
            get { return child; }
            set
            {
                if (this.child == value)
                {
                    return;
                }

                this.child = value;
                PropertyChanged.Raise(this, new PropertyChangedEventArgs("Child"));
            }
        }

        private ObservableCollection<ObservableNode> children;
        public ObservableCollection<ObservableNode> Children
        {
            get { return children; }
            set
            {
                if (this.children == value)
                {
                    return;
                }

                this.children = value;
                PropertyChanged.Raise(this, new PropertyChangedEventArgs("Children"));
            }
        }

        public int this[int index] { get { return index * ItemValue; } }

        private int itemValue;
        public int ItemValue
        {
            get { return itemValue; }
            set
            {
                if (this.itemValue == value)
                {
                    return;
                }

                this.itemValue = value;
                PropertyChanged.Raise(this, new PropertyChangedEventArgs("ItemValue"));
                PropertyChanged.Raise(this, new PropertyChangedEventArgs("Item"));
            }
        }
    }

    public class DependencyObjectNode : DependencyObject
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(DependencyObjectNode), new PropertyMetadata());
        public int Value_ClrWrapper
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty Value2Property = DependencyProperty.Register("Value2", typeof(string), typeof(DependencyObjectNode), new PropertyMetadata());
        public string Value2_ClrWrapper
        {
            get { return (string)GetValue(Value2Property); }
            set { SetValue(Value2Property, value); }
        }

        public static readonly DependencyProperty ChildProperty = DependencyProperty.Register("Child", typeof(DependencyObjectNode), typeof(DependencyObjectNode), new PropertyMetadata());
        public DependencyObjectNode Child_ClrWrapper
        {
            get { return (DependencyObjectNode)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        public static readonly DependencyProperty ObservableChildProperty = DependencyProperty.Register("ObservableChild", typeof(ObservableNode), typeof(DependencyObjectNode), new PropertyMetadata());
        public ObservableNode ObservableChild_ClrWrapper
        {
            get { return (ObservableNode)GetValue(ObservableChildProperty); }
            set { SetValue(ObservableChildProperty, value); }
        }
    }

    [TestClass]
    public class ObservableExpressionTest
    {
        private class CallsTestElement
        {
            public int ValueGetterCallsCount { get; private set; }
            public int ValueSetterCallsCount { get; private set; }

            private int value;
            public int Value
            {
                get
                {
                    ValueGetterCallsCount++;
                    return value;
                }
                set
                {
                    ValueSetterCallsCount++;
                    this.value = value;
                }
            }
        }

        [TestMethod]
        public void ObservableExpressionClrPropertyTest()
        {
            ObservableNode child1 = new ObservableNode { Value = 1 };
            ObservableNode child2 = new ObservableNode { Value = 2 };

            ObservableNode root = new ObservableNode { Value = 0 };

            int valueChangedCount = 0;

            ObservableExpression observableExpression = new ObservableExpression(root, "Child.Value");
            observableExpression.ValueChanged += (sender, oldValue, newValue) => valueChangedCount++;

            Assert.AreEqual(ObservableValue.UnsetValue, observableExpression.Value);

            root.Child = child1;

            Assert.AreEqual(1, observableExpression.Value);
            Assert.AreEqual(1, valueChangedCount);

            child1.Value = 2;

            Assert.AreEqual(2, observableExpression.Value);
            Assert.AreEqual(2, valueChangedCount);

            root.Child = child2;

            Assert.AreEqual(2, observableExpression.Value);
            Assert.AreEqual(2, valueChangedCount);

            child1.Value = 3;

            Assert.AreEqual(2, observableExpression.Value);
            Assert.AreEqual(2, valueChangedCount);

            child2.Value = 3;

            Assert.AreEqual(3, observableExpression.Value);
            Assert.AreEqual(3, valueChangedCount);

            Assert.AreEqual(true, observableExpression.TrySetValue(4));
            Assert.AreEqual(3, child1.Value);
            Assert.AreEqual(4, child2.Value);
            Assert.AreEqual(4, valueChangedCount);

            root.Child = null;

            Assert.AreEqual(ObservableValue.UnsetValue, observableExpression.Value);
            Assert.AreEqual(5, valueChangedCount);

            observableExpression.Dispose();

            root.Child = child1;
            Assert.AreEqual(5, valueChangedCount);
        }

        [TestMethod]
        public void ObservableExpressionDependencyPropertyTest()
        {
            DependencyObjectNode child1 = new DependencyObjectNode { Value_ClrWrapper = 1 };
            DependencyObjectNode child2 = new DependencyObjectNode { Value_ClrWrapper = 2 };

            DependencyObjectNode root = new DependencyObjectNode { Value_ClrWrapper = 0 };

            int valueChangedCount = 0;

            ObservableExpression observableExpression = new ObservableExpression(root, "Child.Value");
            observableExpression.ValueChanged += (sender, oldValue, newValue) => valueChangedCount++;

            Assert.AreEqual(ObservableValue.UnsetValue, observableExpression.Value);

            root.Child_ClrWrapper = child1;

            Assert.AreEqual(1, observableExpression.Value);
            Assert.AreEqual(1, valueChangedCount);

            child1.Value_ClrWrapper = 2;

            Assert.AreEqual(2, observableExpression.Value);
            Assert.AreEqual(2, valueChangedCount);

            root.Child_ClrWrapper = child2;

            Assert.AreEqual(2, observableExpression.Value);
            Assert.AreEqual(2, valueChangedCount);

            child1.Value_ClrWrapper = 3;

            Assert.AreEqual(2, observableExpression.Value);
            Assert.AreEqual(2, valueChangedCount);

            child2.Value_ClrWrapper = 3;

            Assert.AreEqual(3, observableExpression.Value);
            Assert.AreEqual(3, valueChangedCount);

            Assert.AreEqual(true, observableExpression.TrySetValue(4));
            Assert.AreEqual(3, child1.Value_ClrWrapper);
            Assert.AreEqual(4, child2.Value_ClrWrapper);
            Assert.AreEqual(4, valueChangedCount);

            root.Child_ClrWrapper = null;

            Assert.AreEqual(ObservableValue.UnsetValue, observableExpression.Value);
            Assert.AreEqual(5, valueChangedCount);

            observableExpression.Dispose();

            root.Child_ClrWrapper = child1;
            Assert.AreEqual(5, valueChangedCount);
        }

        [TestMethod]
        public void ObservableExpressionHybridPropertyTest()
        {
            DependencyObjectNode child = new DependencyObjectNode();
            ObservableNode observableChild = new ObservableNode { Value = 1 };

            DependencyObjectNode root = new DependencyObjectNode { Value_ClrWrapper = 0 };

            int valueChangedCount = 0;

            ObservableExpression observer = new ObservableExpression(root, "Child.ObservableChild.Value");
            observer.ValueChanged += (sender, oldValue, newValue) => valueChangedCount++;

            Assert.AreEqual(ObservableValue.UnsetValue, observer.Value);

            root.Child_ClrWrapper = child;
            child.ObservableChild_ClrWrapper = observableChild;

            Assert.AreEqual(1, observer.Value);
            Assert.AreEqual(1, valueChangedCount);

            observableChild.Value = 2;

            Assert.AreEqual(2, observer.Value);
            Assert.AreEqual(2, valueChangedCount);

            root.Child_ClrWrapper = null;

            Assert.AreEqual(ObservableValue.UnsetValue, observer.Value);
            Assert.AreEqual(3, valueChangedCount);

            observer.Dispose();

            root.Child_ClrWrapper = child;

            Assert.AreEqual(3, valueChangedCount);
        }

        [TestMethod]
        public void ObservableExpressionCallsTest()
        {
            CallsTestElement element = new CallsTestElement();
            element.Value = 1;

            Assert.AreEqual(0, element.ValueGetterCallsCount);
            Assert.AreEqual(1, element.ValueSetterCallsCount);

            ObservableExpression observer = new ObservableExpression(element, "Value");

            Assert.AreEqual(1, element.ValueGetterCallsCount);
            Assert.AreEqual(1, element.ValueSetterCallsCount);
            Assert.AreEqual(1, observer.Value);

            Assert.IsTrue(observer.TrySetValue(2));

            Assert.AreEqual(2, element.ValueGetterCallsCount);
            Assert.AreEqual(2, element.ValueSetterCallsCount);
            Assert.AreEqual(2, observer.Value);

            Assert.IsTrue(observer.TrySetValue(2));

            Assert.AreEqual(2, element.ValueGetterCallsCount);
            Assert.AreEqual(2, element.ValueSetterCallsCount);
            Assert.AreEqual(2, observer.Value);

            Assert.IsTrue(observer.TrySetValue(3));

            Assert.AreEqual(3, element.ValueGetterCallsCount);
            Assert.AreEqual(3, element.ValueSetterCallsCount);
            Assert.AreEqual(3, observer.Value);
        }

        [TestMethod]
        public void ObservableExpressionIndexerTest()
        {
            ObservableNode root = new ObservableNode { Children = new ObservableCollection<ObservableNode> { new ObservableNode { Value = 1 } } };

            int valueChanged = 0;

            ObservableExpression observableExpression = new ObservableExpression(root, "Children[0].Value");
            observableExpression.ValueChanged += (sender, oldValue, newValue) => valueChanged++;

            Assert.AreEqual(1, observableExpression.Value);
            Assert.AreEqual(0, valueChanged);

            root.Children[0].Value = 2;
            Assert.AreEqual(2, observableExpression.Value);
            Assert.AreEqual(1, valueChanged);

            root.Children[0] = new ObservableNode { Value = 3 };
            Assert.AreEqual(3, observableExpression.Value);
            Assert.AreEqual(2, valueChanged);

            root.Children = new ObservableCollection<ObservableNode> { new ObservableNode { Value = 4 } };
            Assert.AreEqual(4, observableExpression.Value);
            Assert.AreEqual(3, valueChanged);

            root.Children[0] = new ObservableNode { Value = 4 };
            Assert.AreEqual(4, observableExpression.Value);
            Assert.AreEqual(3, valueChanged);

            root.Children = new ObservableCollection<ObservableNode> { new ObservableNode { Value = 4 } };
            Assert.AreEqual(4, observableExpression.Value);
            Assert.AreEqual(3, valueChanged);

            root.Children.Add(new ObservableNode { Value = 5 });
            Assert.AreEqual(4, observableExpression.Value);
            Assert.AreEqual(3, valueChanged);

            root.Children.RemoveAt(0);
            Assert.AreEqual(5, observableExpression.Value);
            Assert.AreEqual(4, valueChanged);

            observableExpression.Dispose();
            root.Children[0].Value = 6;
            Assert.AreEqual(4, valueChanged);
        }

        [TestMethod]
        public void ObservableExpressionCustomIndexerTest()
        {
            ObservableNode root = new ObservableNode { ItemValue = 2 };
            int valueChanged = 0;

            ObservableExpression observableExpression1 = new ObservableExpression(root, "Item[3]");
            ObservableExpression observableExpression2 = new ObservableExpression(root, "[4]");

            observableExpression1.ValueChanged += (sender, oldValue, newValue) => valueChanged++;
            observableExpression2.ValueChanged += (sender, oldValue, newValue) => valueChanged++;

            Assert.AreEqual(6, observableExpression1.Value);
            Assert.AreEqual(8, observableExpression2.Value);

            root.ItemValue = 3;
            Assert.AreEqual(9, observableExpression1.Value);
            Assert.AreEqual(12, observableExpression2.Value);
            Assert.AreEqual(2, valueChanged);
        }
    }
}
