using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System.Windows.Data;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class TriggerTest
    {
        public static readonly DependencyProperty Value1Property = DependencyProperty.RegisterAttached("Value1", typeof(int), typeof(TriggerTest), new FrameworkPropertyMetadata(0));
        public static readonly DependencyProperty Value2Property = DependencyProperty.RegisterAttached("Value2", typeof(int), typeof(TriggerTest), new FrameworkPropertyMetadata(0));
        public static readonly DependencyProperty Value3Property = DependencyProperty.RegisterAttached("Value3", typeof(int), typeof(TriggerTest), new FrameworkPropertyMetadata(0));
        public static readonly RoutedEvent Event1Event = EventManager.RegisterRoutedEvent("Event1", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(TriggerTest));

        [TestMethod]
        public void TriggerBasicTest()
        {
            Trigger trigger = new Trigger { Property = new DependencyPropertyPathElement(Value1Property), Value = 1 };
            trigger.Setters.Add(new Setter { Property = new DependencyPropertyPathElement(Value2Property), Value = 1 });

            FrameworkElement element = new FrameworkElement();
            element.SetValue(Value1Property, 1);

            Assert.AreEqual(1, element.GetValue(Value1Property));
            Assert.AreEqual(0, element.GetValue(Value2Property));

            element.Triggers.Add(trigger);
            Assert.AreEqual(1, element.GetValue(Value1Property));
            Assert.AreEqual(1, element.GetValue(Value2Property));

            element.SetValue(Value1Property, 2);
            Assert.AreEqual(0, element.GetValue(Value2Property));

            element.SetValue(Value1Property, 1);
            Assert.AreEqual(1, element.GetValue(Value2Property));

            element.Triggers.Remove(trigger);
            Assert.AreEqual(0, element.GetValue(Value2Property));
        }

        [TestMethod]
        public void DataTriggerBasicTest()
        {
            DataTrigger trigger = new DataTrigger { Binding = new Binding(), Value = 1 };
            trigger.Setters.Add(new Setter { Property = new DependencyPropertyPathElement(Value1Property), Value = 1 });

            FrameworkElement element = new FrameworkElement();
            element.DataContext = 1;
            Assert.AreEqual(0, element.GetValue(Value1Property));

            element.Triggers.Add(trigger);
            Assert.AreEqual(1, element.GetValue(Value1Property));

            element.DataContext = 2;
            Assert.AreEqual(0, element.GetValue(Value1Property));

            element.DataContext = 1;
            Assert.AreEqual(1, element.GetValue(Value1Property));

            element.Triggers.Remove(trigger);
            Assert.AreEqual(0, element.GetValue(Value1Property));
        }

        [TestMethod]
        public void EventTriggerBasicTest()
        {
            EventTrigger trigger = new EventTrigger { RoutedEvent = Event1Event };
            trigger.Actions.Add(new Setter { Property = new DependencyPropertyPathElement(Value1Property), Value = 1 });

            FrameworkElement element = new FrameworkElement();

            Assert.AreEqual(0, element.GetValue(Value1Property));

            element.Triggers.Add(trigger);
            Assert.AreEqual(0, element.GetValue(Value1Property));

            element.RaiseEvent(new RoutedEventArgs(Event1Event, element));
            Assert.AreEqual(1, element.GetValue(Value1Property));

            element.Triggers.Remove(trigger);
            Assert.AreEqual(0, element.GetValue(Value2Property));
        }

        [TestMethod]
        public void MultiTriggerBasicTest()
        {
            Condition condition1 = new Condition { Property = new DependencyPropertyPathElement(Value1Property), Value = 1 };
            Condition condition2 = new Condition { Property = new DependencyPropertyPathElement(Value2Property), Value = 2 };

            MultiTrigger multiTrigger = new MultiTrigger();
            multiTrigger.Conditions.Add(condition1);
            multiTrigger.Conditions.Add(condition2);
            multiTrigger.Setters.Add(new Setter { Property = new DependencyPropertyPathElement(Value3Property), Value = 3 });


            FrameworkElement element = new FrameworkElement();
            element.SetValue(Value1Property, 1);
            element.SetValue(Value2Property, 2);

            Assert.AreEqual(1, element.GetValue(Value1Property));
            Assert.AreEqual(2, element.GetValue(Value2Property));
            Assert.AreEqual(0, element.GetValue(Value3Property));

            element.Triggers.Add(multiTrigger);
            Assert.AreEqual(3, element.GetValue(Value3Property));

            element.SetValue(Value1Property, 2);
            Assert.AreEqual(0, element.GetValue(Value3Property));

            element.SetValue(Value1Property, 1);
            Assert.AreEqual(3, element.GetValue(Value3Property));

            element.Triggers.Remove(multiTrigger);
            Assert.AreEqual(0, element.GetValue(Value3Property));
        }
    }
}
