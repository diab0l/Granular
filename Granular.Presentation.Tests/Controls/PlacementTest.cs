using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Controls
{
    [TestClass]
    public class PlacementTest
    {
        private static readonly Rect PlacementTargetRect = new Rect(200, 100, 600, 400);
        private static readonly Rect PlacementRectangle = new Rect(200, 100, 600, 400);
        private static readonly Rect ContainerBounds = new Rect(1000, 800);
        private static readonly Size PopupSize = new Size(200, 100);

        [TestMethod]
        public void PlacementAbsoluteTest()
        {
            Assert.AreEqual(ContainerBounds.GetTopLeft(), Placement.GetPosition(PlacementMode.Absolute, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(0, 0), PopupSize, ContainerBounds));
            Assert.AreEqual(PlacementRectangle.GetTopLeft(), Placement.GetPosition(PlacementMode.Absolute, PlacementTargetRect, PlacementRectangle, Rect.Empty, new Point(0, 0), PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(0, 0), Placement.GetPosition(PlacementMode.Absolute, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(-100, -50), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(0, 100), Placement.GetPosition(PlacementMode.Absolute, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(-100, 100), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(0, 700), Placement.GetPosition(PlacementMode.Absolute, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(-100, 750), PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(100, 0), Placement.GetPosition(PlacementMode.Absolute, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(100, -50), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(100, 100), Placement.GetPosition(PlacementMode.Absolute, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(100, 100), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(100, 700), Placement.GetPosition(PlacementMode.Absolute, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(100, 750), PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(800, 0), Placement.GetPosition(PlacementMode.Absolute, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(900, -50), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(800, 100), Placement.GetPosition(PlacementMode.Absolute, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(900, 100), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(800, 700), Placement.GetPosition(PlacementMode.Absolute, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(900, 750), PopupSize, ContainerBounds));
        }

        [TestMethod]
        public void PlacementAbsolutePointTest()
        {
            Assert.AreEqual(ContainerBounds.GetTopLeft(), Placement.GetPosition(PlacementMode.AbsolutePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(0, 0), PopupSize, ContainerBounds));
            Assert.AreEqual(PlacementRectangle.GetTopLeft(), Placement.GetPosition(PlacementMode.AbsolutePoint, PlacementTargetRect, PlacementRectangle, Rect.Empty, new Point(0, 0), PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(0, 0), Placement.GetPosition(PlacementMode.AbsolutePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(-100, -50), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(0, 100), Placement.GetPosition(PlacementMode.AbsolutePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(-100, 100), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(0, 650), Placement.GetPosition(PlacementMode.AbsolutePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(-100, 750), PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(100, 0), Placement.GetPosition(PlacementMode.AbsolutePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(100, -50), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(100, 100), Placement.GetPosition(PlacementMode.AbsolutePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(100, 100), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(100, 650), Placement.GetPosition(PlacementMode.AbsolutePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(100, 750), PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(700, 0), Placement.GetPosition(PlacementMode.AbsolutePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(900, -50), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(700, 100), Placement.GetPosition(PlacementMode.AbsolutePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(900, 100), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(700, 650), Placement.GetPosition(PlacementMode.AbsolutePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(900, 750), PopupSize, ContainerBounds));
        }

        [TestMethod]
        public void PlacementRelativeTest()
        {
            Assert.AreEqual(PlacementTargetRect.GetTopLeft(), Placement.GetPosition(PlacementMode.Relative, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(0, 0), PopupSize, ContainerBounds));
            Assert.AreEqual(PlacementTargetRect.GetTopLeft() + PlacementRectangle.GetTopLeft(), Placement.GetPosition(PlacementMode.Relative, PlacementTargetRect, PlacementRectangle, Rect.Empty, new Point(0, 0), PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(0, 0), Placement.GetPosition(PlacementMode.Relative, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(-300, -150), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(0, 200), Placement.GetPosition(PlacementMode.Relative, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(-300, 100), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(0, 700), Placement.GetPosition(PlacementMode.Relative, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(-300, 650), PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(300, 0), Placement.GetPosition(PlacementMode.Relative, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(100, -150), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(300, 200), Placement.GetPosition(PlacementMode.Relative, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(100, 100), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(300, 700), Placement.GetPosition(PlacementMode.Relative, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(100, 650), PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(800, 0), Placement.GetPosition(PlacementMode.Relative, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(700, -150), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(800, 200), Placement.GetPosition(PlacementMode.Relative, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(700, 100), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(800, 700), Placement.GetPosition(PlacementMode.Relative, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(700, 650), PopupSize, ContainerBounds));
        }

        [TestMethod]
        public void PlacementRelativePointTest()
        {
            Assert.AreEqual(PlacementTargetRect.GetTopLeft(), Placement.GetPosition(PlacementMode.RelativePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(0, 0), PopupSize, ContainerBounds));
            Assert.AreEqual(PlacementTargetRect.GetTopLeft() + PlacementRectangle.GetTopLeft(), Placement.GetPosition(PlacementMode.RelativePoint, PlacementTargetRect, PlacementRectangle, Rect.Empty, new Point(0, 0), PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(0, 0), Placement.GetPosition(PlacementMode.RelativePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(-300, -150), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(0, 200), Placement.GetPosition(PlacementMode.RelativePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(-300, 100), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(0, 650), Placement.GetPosition(PlacementMode.RelativePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(-300, 650), PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(300, 0), Placement.GetPosition(PlacementMode.RelativePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(100, -150), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(300, 200), Placement.GetPosition(PlacementMode.RelativePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(100, 100), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(300, 650), Placement.GetPosition(PlacementMode.RelativePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(100, 650), PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(700, 0), Placement.GetPosition(PlacementMode.RelativePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(700, -150), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(700, 200), Placement.GetPosition(PlacementMode.RelativePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(700, 100), PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(700, 650), Placement.GetPosition(PlacementMode.RelativePoint, PlacementTargetRect, Rect.Empty, Rect.Empty, new Point(700, 650), PopupSize, ContainerBounds));
        }

        [TestMethod]
        public void PlacementTopTest()
        {
            Assert.AreEqual(new Point(0, 50), Placement.GetPosition(PlacementMode.Top, new Rect(-100, -50, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(300, 50), Placement.GetPosition(PlacementMode.Top, new Rect(300, -50, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(800, 50), Placement.GetPosition(PlacementMode.Top, new Rect(900, -50, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(0, 200), Placement.GetPosition(PlacementMode.Top, new Rect(-100, 300, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(300, 200), Placement.GetPosition(PlacementMode.Top, new Rect(300, 300, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(800, 200), Placement.GetPosition(PlacementMode.Top, new Rect(900, 300, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(0, 650), Placement.GetPosition(PlacementMode.Top, new Rect(-100, 750, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(300, 650), Placement.GetPosition(PlacementMode.Top, new Rect(300, 750, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(800, 650), Placement.GetPosition(PlacementMode.Top, new Rect(900, 750, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
        }

        [TestMethod]
        public void PlacementBottomTest()
        {
            Assert.AreEqual(new Point(0, 50), Placement.GetPosition(PlacementMode.Bottom, new Rect(-100, -50, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(300, 50), Placement.GetPosition(PlacementMode.Bottom, new Rect(300, -50, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(800, 50), Placement.GetPosition(PlacementMode.Bottom, new Rect(900, -50, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(0, 400), Placement.GetPosition(PlacementMode.Bottom, new Rect(-100, 300, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(300, 400), Placement.GetPosition(PlacementMode.Bottom, new Rect(300, 300, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(800, 400), Placement.GetPosition(PlacementMode.Bottom, new Rect(900, 300, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(0, 650), Placement.GetPosition(PlacementMode.Bottom, new Rect(-100, 750, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(300, 650), Placement.GetPosition(PlacementMode.Bottom, new Rect(300, 750, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(800, 650), Placement.GetPosition(PlacementMode.Bottom, new Rect(900, 750, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
        }

        [TestMethod]
        public void PlacementLeftTest()
        {
            Assert.AreEqual(new Point(100, 0), Placement.GetPosition(PlacementMode.Left, new Rect(-100, -50, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(100, 0), Placement.GetPosition(PlacementMode.Left, new Rect(300, -50, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(700, 0), Placement.GetPosition(PlacementMode.Left, new Rect(900, -50, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(100, 300), Placement.GetPosition(PlacementMode.Left, new Rect(-100, 300, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(100, 300), Placement.GetPosition(PlacementMode.Left, new Rect(300, 300, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(700, 300), Placement.GetPosition(PlacementMode.Left, new Rect(900, 300, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(100, 700), Placement.GetPosition(PlacementMode.Left, new Rect(-100, 750, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(100, 700), Placement.GetPosition(PlacementMode.Left, new Rect(300, 750, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(700, 700), Placement.GetPosition(PlacementMode.Left, new Rect(900, 750, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
        }

        [TestMethod]
        public void PlacementRightTest()
        {
            Assert.AreEqual(new Point(100, 0), Placement.GetPosition(PlacementMode.Right, new Rect(-100, -50, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(500, 0), Placement.GetPosition(PlacementMode.Right, new Rect(300, -50, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(700, 0), Placement.GetPosition(PlacementMode.Right, new Rect(900, -50, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(100, 300), Placement.GetPosition(PlacementMode.Right, new Rect(-100, 300, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(500, 300), Placement.GetPosition(PlacementMode.Right, new Rect(300, 300, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(700, 300), Placement.GetPosition(PlacementMode.Right, new Rect(900, 300, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(100, 700), Placement.GetPosition(PlacementMode.Right, new Rect(-100, 750, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(500, 700), Placement.GetPosition(PlacementMode.Right, new Rect(300, 750, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(700, 700), Placement.GetPosition(PlacementMode.Right, new Rect(900, 750, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
        }

        [TestMethod]
        public void PlacementCenterTest()
        {
            Assert.AreEqual(new Point(0, 0), Placement.GetPosition(PlacementMode.Center, new Rect(-100, -50, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(100, 0), Placement.GetPosition(PlacementMode.Center, new Rect(100, -50, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(800, 0), Placement.GetPosition(PlacementMode.Center, new Rect(900, -50, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(0, 100), Placement.GetPosition(PlacementMode.Center, new Rect(-100, 100, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(100, 100), Placement.GetPosition(PlacementMode.Center, new Rect(100, 100, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(800, 100), Placement.GetPosition(PlacementMode.Center, new Rect(900, 100, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(0, 700), Placement.GetPosition(PlacementMode.Center, new Rect(-100, 750, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(100, 700), Placement.GetPosition(PlacementMode.Center, new Rect(100, 750, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(800, 700), Placement.GetPosition(PlacementMode.Center, new Rect(900, 750, 200, 100), Rect.Empty, Rect.Empty, Point.Zero, PopupSize, ContainerBounds));
        }

        [TestMethod]
        public void PlacementMouseTest()
        {
            Assert.AreEqual(new Point(0, 50), Placement.GetPosition(PlacementMode.Mouse, Rect.Empty, Rect.Empty, new Rect(-100, -50, 200, 100), Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(100, 50), Placement.GetPosition(PlacementMode.Mouse, Rect.Empty, Rect.Empty, new Rect(100, -50, 200, 100), Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(800, 50), Placement.GetPosition(PlacementMode.Mouse, Rect.Empty, Rect.Empty, new Rect(900, -50, 200, 100), Point.Zero, PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(0, 200), Placement.GetPosition(PlacementMode.Mouse, Rect.Empty, Rect.Empty, new Rect(-100, 100, 200, 100), Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(100, 200), Placement.GetPosition(PlacementMode.Mouse, Rect.Empty, Rect.Empty, new Rect(100, 100, 200, 100), Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(800, 200), Placement.GetPosition(PlacementMode.Mouse, Rect.Empty, Rect.Empty, new Rect(900, 100, 200, 100), Point.Zero, PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(0, 650), Placement.GetPosition(PlacementMode.Mouse, Rect.Empty, Rect.Empty, new Rect(-100, 750, 200, 100), Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(100, 650), Placement.GetPosition(PlacementMode.Mouse, Rect.Empty, Rect.Empty, new Rect(100, 750, 200, 100), Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(800, 650), Placement.GetPosition(PlacementMode.Mouse, Rect.Empty, Rect.Empty, new Rect(900, 750, 200, 100), Point.Zero, PopupSize, ContainerBounds));
        }

        [TestMethod]
        public void PlacementMousePointTest()
        {
            Assert.AreEqual(new Point(0, 0), Placement.GetPosition(PlacementMode.MousePoint, Rect.Empty, Rect.Empty, new Rect(-100, -50, 200, 100), Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(100, 0), Placement.GetPosition(PlacementMode.MousePoint, Rect.Empty, Rect.Empty, new Rect(100, -50, 200, 100), Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(700, 0), Placement.GetPosition(PlacementMode.MousePoint, Rect.Empty, Rect.Empty, new Rect(900, -50, 200, 100), Point.Zero, PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(0, 100), Placement.GetPosition(PlacementMode.MousePoint, Rect.Empty, Rect.Empty, new Rect(-100, 100, 200, 100), Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(100, 100), Placement.GetPosition(PlacementMode.MousePoint, Rect.Empty, Rect.Empty, new Rect(100, 100, 200, 100), Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(700, 100), Placement.GetPosition(PlacementMode.MousePoint, Rect.Empty, Rect.Empty, new Rect(900, 100, 200, 100), Point.Zero, PopupSize, ContainerBounds));

            Assert.AreEqual(new Point(0, 650), Placement.GetPosition(PlacementMode.MousePoint, Rect.Empty, Rect.Empty, new Rect(-100, 750, 200, 100), Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(100, 650), Placement.GetPosition(PlacementMode.MousePoint, Rect.Empty, Rect.Empty, new Rect(100, 750, 200, 100), Point.Zero, PopupSize, ContainerBounds));
            Assert.AreEqual(new Point(700, 650), Placement.GetPosition(PlacementMode.MousePoint, Rect.Empty, Rect.Empty, new Rect(900, 750, 200, 100), Point.Zero, PopupSize, ContainerBounds));
        }
    }
}
