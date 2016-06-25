using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Controls.Primitives
{
    [TestClass]
    public class UniformGridTest
    {
        [TestMethod]
        public void UniformGridRowsTest()
        {
            FrameworkElement child1 = new FrameworkElement();
            FrameworkElement child2 = new FrameworkElement();
            FrameworkElement child3 = new FrameworkElement();

            UniformGrid uniformGrid = new UniformGrid { Rows = 4 };

            uniformGrid.Children.Add(child1);
            uniformGrid.Children.Add(child2);
            uniformGrid.Children.Add(child3);

            uniformGrid.Measure(Size.Infinity);
            uniformGrid.Arrange(new Rect(120, 120));

            Assert.AreEqual(new Rect(0, 0, 120, 30), child1.VisualBounds);
            Assert.AreEqual(new Rect(0, 30, 120, 30), child2.VisualBounds);
            Assert.AreEqual(new Rect(0, 60, 120, 30), child3.VisualBounds);
        }

        [TestMethod]
        public void UniformGridColumnsTest()
        {
            FrameworkElement child1 = new FrameworkElement();
            FrameworkElement child2 = new FrameworkElement();
            FrameworkElement child3 = new FrameworkElement();

            UniformGrid uniformGrid = new UniformGrid { Columns = 4 };

            uniformGrid.Children.Add(child1);
            uniformGrid.Children.Add(child2);
            uniformGrid.Children.Add(child3);

            uniformGrid.Measure(Size.Infinity);
            uniformGrid.Arrange(new Rect(120, 120));

            Assert.AreEqual(new Rect(0, 0, 30, 120), child1.VisualBounds);
            Assert.AreEqual(new Rect(30, 0, 30, 120), child2.VisualBounds);
            Assert.AreEqual(new Rect(60, 0, 30, 120), child3.VisualBounds);
        }

        [TestMethod]
        public void UniformGridDefaultLayoutTest()
        {
            FrameworkElement child1 = new FrameworkElement();
            FrameworkElement child2 = new FrameworkElement();
            FrameworkElement child3 = new FrameworkElement();

            UniformGrid uniformGrid = new UniformGrid();

            uniformGrid.Children.Add(child1);
            uniformGrid.Children.Add(child2);
            uniformGrid.Children.Add(child3);

            uniformGrid.Measure(Size.Infinity);
            uniformGrid.Arrange(new Rect(120, 120));

            Assert.AreEqual(new Rect(0, 0, 60, 60), child1.VisualBounds);
            Assert.AreEqual(new Rect(60, 0, 60, 60), child2.VisualBounds);
            Assert.AreEqual(new Rect(0, 60, 60, 60), child3.VisualBounds);
        }

        [TestMethod]
        public void UniformGridFirstColumnTest()
        {
            FrameworkElement child1 = new FrameworkElement();

            UniformGrid uniformGrid = new UniformGrid { FirstColumn = 3 };

            uniformGrid.Children.Add(child1);

            uniformGrid.Measure(Size.Infinity);
            uniformGrid.Arrange(new Rect(120, 120));

            Assert.AreEqual(new Rect(60, 60, 60, 60), child1.VisualBounds);
        }
    }
}
