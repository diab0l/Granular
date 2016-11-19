using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class FreezableTest
    {
        public static readonly DependencyProperty AttachedValueProperty = DependencyProperty.RegisterAttached("AttachedValue", typeof(int), typeof(FreezableTest), new FrameworkPropertyMetadata());

        private class FreezableTestElement : Freezable
        {
            public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(FreezableTestElement), new FrameworkPropertyMetadata());
            public int Value
            {
                get { return (int)GetValue(ValueProperty); }
                set { SetValue(ValueProperty, value); }
            }

            public static readonly DependencyProperty ChildProperty = DependencyProperty.Register("Child", typeof(FreezableTestElement), typeof(FreezableTestElement), new FrameworkPropertyMetadata());
            public FreezableTestElement Child
            {
                get { return (FreezableTestElement)GetValue(ChildProperty); }
                set { SetValue(ChildProperty, value); }
            }
        }

        [TestMethod]
        public void FreezableResourceContainerTest()
        {
            int resourcesChangedCount = 0;
            object resourceValue;

            Freezable freezable = new Freezable();
            freezable.ResourcesChanged += (sender, e) => resourcesChangedCount++;

            FrameworkElement element = new FrameworkElement();
            element.Resources = new ResourceDictionary();
            element.Resources.Add("key1", "value1");

            Assert.IsFalse(freezable.TryGetResource("key1", out resourceValue));

            freezable.TrySetContextParent(element);
            Assert.AreEqual(1, resourcesChangedCount);
            Assert.IsTrue(freezable.TryGetResource("key1", out resourceValue));
            Assert.AreEqual("value1", resourceValue);

            element.Resources.Add("key2", "value2");
            Assert.AreEqual(2, resourcesChangedCount);
            Assert.IsTrue(freezable.TryGetResource("key2", out resourceValue));
            Assert.AreEqual("value2", resourceValue);
        }

        [TestMethod]
        public void FreezableCollectionBasicTest()
        {
            int resources1ChangedCount = 0;
            object resourceValue;

            Freezable freezable1 = new Freezable();
            freezable1.ResourcesChanged += (sender, e) => resources1ChangedCount++;

            FreezableCollection<Freezable> freezableCollection = new FreezableCollection<Freezable> { freezable1 };

            FrameworkElement element = new FrameworkElement();
            element.Resources = new ResourceDictionary();
            element.Resources.Add("key1", "value1");

            Assert.IsFalse(freezable1.TryGetResource("key1", out resourceValue));

            freezableCollection.TrySetContextParent(element);
            Assert.AreEqual(2, resources1ChangedCount);
            Assert.IsTrue(freezable1.TryGetResource("key1", out resourceValue));
            Assert.AreEqual("value1", resourceValue);

            Freezable freezable2 = new Freezable();
            freezableCollection.Add(freezable2);
            Assert.IsTrue(freezable2.TryGetResource("key1", out resourceValue));
            Assert.AreEqual("value1", resourceValue);

            freezableCollection.Clear();
            Assert.AreEqual(3, resources1ChangedCount);
            Assert.IsFalse(freezable1.TryGetResource("key1", out resourceValue));
            Assert.IsFalse(freezable2.TryGetResource("key1", out resourceValue));
        }

        [TestMethod]
        public void FreezableResourceReferenceExpressionTest()
        {
            string styleResource = @"
<Style xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
  <Setter Property='Control.Template'>
    <Setter.Value>
      <ControlTemplate TargetType='Button'>
        <Border>
          <Border.Background>
            <SolidColorBrush Color='{DynamicResource BackgroundColor}'/>
          </Border.Background>
        </Border>
      </ControlTemplate>
    </Setter.Value>
  </Setter>
</Style>";

            Style style = XamlLoader.Load(XamlParser.Parse(styleResource)) as Style;

            Control control = new Control();
            control.Resources = new ResourceDictionary();
            control.Resources.Add("BackgroundColor", Colors.Red);

            control.Style = style;
            Assert.AreEqual(1, control.VisualChildren.Count());

            Border border = control.VisualChildren.First() as Border;
            Assert.IsNotNull(border);

            Assert.IsTrue(border.Background is SolidColorBrush);
            Assert.AreEqual(Colors.Red, ((SolidColorBrush)border.Background).Color);

            control.Resources.Add("BackgroundColor", Colors.Blue);
            Assert.AreEqual(Colors.Blue, ((SolidColorBrush)border.Background).Color);
        }

        [TestMethod]
        public void FreezableFreezeTest()
        {
            FreezableTestElement freezable = new FreezableTestElement();
            Assert.IsFalse(freezable.IsFrozen);

            freezable.Value = 1;
            Assert.AreEqual(1, freezable.Value);

            FreezableTestElement child = new FreezableTestElement();
            freezable.Child = child;
            child.Value = 2;
            Assert.AreEqual(2, freezable.Child.Value);

            freezable.Freeze();
            Assert.IsTrue(freezable.IsFrozen);

            try
            {
                freezable.Value = 4;
                Assert.Fail();
            }
            catch
            {
                //
            }

            try
            {
                freezable.SetValue(AttachedValueProperty, 3);
                Assert.Fail();
            }
            catch
            {
                //
            }

            try
            {
                child.Value = 5;
                Assert.Fail();
            }
            catch
            {
                //
            }
        }
    }
}
