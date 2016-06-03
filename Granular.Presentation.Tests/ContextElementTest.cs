using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class ContextElementTest
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached("Value", typeof(object), typeof(ContextElementTest), new FrameworkPropertyMetadata());

        [TestMethod]
        public void ContextElementParentTest()
        {
            FrameworkElement root = new FrameworkElement();
            FrameworkElement child = new FrameworkElement();

            Freezable value = new Freezable();
            Freezable subValue = new Freezable();

            value.SetValue(ValueProperty, subValue);
            child.SetValue(ValueProperty, value);
            root.AddVisualChild(child);

            Assert.AreEqual(value, ((IContextElement)subValue).ContextParent);
            Assert.AreEqual(child, ((IContextElement)value).ContextParent);
            Assert.AreEqual(root, ((IContextElement)child).ContextParent);
        }
    }
}
