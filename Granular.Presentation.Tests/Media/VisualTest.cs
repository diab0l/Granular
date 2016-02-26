using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Media
{
    [TestClass]
    public class VisualTest
    {
        private class TestVisual : Visual
        {
            private Transform transform;
            public Transform Transform
            {
                get { return transform; }
                set
                {
                    transform = value;
                    InvalidateVisualTransform();
                }
            }

            public Rect Bounds
            {
                get { return VisualBounds; }
                set { VisualBounds = value; }
            }

            protected override Transform GetVisualTransformOverride()
            {
                return transform;
            }
        }

        [TestMethod]
        public void PointTranslationBasicTest()
        {
            TestVisual parent = new TestVisual();
            TestVisual child1 = new TestVisual();
            TestVisual child2 = new TestVisual();

            parent.AddVisualChild(child1);
            child1.AddVisualChild(child2);

            parent.Bounds = new Rect(100, 50, 450, 400);
            child1.Bounds = new Rect(50, 100, 350, 200);
            child2.Bounds = new Rect(100, 50, 150, 100);

            Assert.AreEqual(new Point(100, 50), parent.PointToRoot(new Point(0, 0)));
            Assert.AreEqual(new Point(150, 150), child1.PointToRoot(new Point(0, 0)));
            Assert.AreEqual(new Point(250, 200), child2.PointToRoot(new Point(0, 0)));

            Assert.AreEqual(new Point(0, 0), parent.PointFromRoot(new Point(100, 50)));
            Assert.AreEqual(new Point(0, 0), child1.PointFromRoot(new Point(150, 150)));
            Assert.AreEqual(new Point(0, 0), child2.PointFromRoot(new Point(250, 200)));

            Assert.AreEqual(new Point(300, 250), parent.PointToRoot(new Point(200, 200)));
            Assert.AreEqual(new Point(300, 250), child1.PointToRoot(new Point(150, 100)));
            Assert.AreEqual(new Point(300, 250), child2.PointToRoot(new Point(50, 50)));

            Assert.AreEqual(new Point(200, 200), parent.PointFromRoot(new Point(300, 250)));
            Assert.AreEqual(new Point(150, 100), child1.PointFromRoot(new Point(300, 250)));
            Assert.AreEqual(new Point(50, 50), child2.PointFromRoot(new Point(300, 250)));
        }

        [TestMethod]
        public void PointTranslationTransformTest()
        {
            TestVisual parent = new TestVisual();
            TestVisual child1 = new TestVisual();
            TestVisual child2 = new TestVisual();

            parent.AddVisualChild(child1);
            child1.AddVisualChild(child2);

            parent.Bounds = new Rect(100, 50, 500, 400);
            child1.Bounds = new Rect(50, 100, 400, 200);
            child2.Bounds = new Rect(100, 50, 200, 100);

            child1.Transform = new RotateTransform { Angle = 90, CenterX = 200, CenterY = 100 };
            child2.Transform = new ScaleTransform { ScaleX = 2, ScaleY = 0.5, CenterX = 100, CenterY = 50 };

            Assert.IsTrue(parent.PointToRoot(new Point(0, 0)).IsClose(new Point(100, 50)));
            Assert.IsTrue(child1.PointToRoot(new Point(0, 0)).IsClose(new Point(450, 50)));
            Assert.IsTrue(child2.PointToRoot(new Point(0, 0)).IsClose(new Point(375, 50)));

            Assert.IsTrue(parent.PointFromRoot(new Point(100, 50)).IsClose(new Point(0, 0)));
            Assert.IsTrue(child1.PointFromRoot(new Point(450, 50)).IsClose(new Point(0, 0)));
            Assert.IsTrue(child2.PointFromRoot(new Point(375, 50)).IsClose(new Point(0, 0)));
        }
    }
}
