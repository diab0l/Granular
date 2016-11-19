using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class NameScopeTest
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached("Value", typeof(object), typeof(NameScopeTest), new PropertyMetadata());

        [TestMethod]
        public void ContainingNameScopeTest()
        {
            FrameworkElement logicalParent = new FrameworkElement();
            FrameworkElement visualParent = new FrameworkElement();
            FrameworkElement child = new FrameworkElement();

            NameScope logicalNameScope = new NameScope();
            NameScope.SetNameScope(logicalParent, logicalNameScope);

            NameScope visualNameScope = new NameScope();
            NameScope.SetNameScope(visualParent, visualNameScope);

            Freezable value = new Freezable();
            Freezable subValue = new Freezable();

            // visual / context tree: logicalParent -> visualParent -> child -> value -> subValue
            logicalParent.AddVisualChild(visualParent);
            visualParent.AddVisualChild(child);
            child.SetValue(ValueProperty, value);
            value.SetValue(ValueProperty, subValue);

            // logical tree: logicalParent -> child
            logicalParent.AddLogicalChild(child);

            Assert.AreEqual(logicalNameScope, NameScope.GetContainingNameScope(subValue));
        }
    }
}
