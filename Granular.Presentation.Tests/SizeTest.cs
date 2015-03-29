using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class SizeTest
    {
        [TestMethod]
        public void SizeOperationsTest()
        {
            Size size1 = new Size(1, 1);

            Assert.AreEqual(1, size1.Width);
            Assert.AreEqual(1, size1.Height);

            Size size2 = size1 * 2;

            Assert.AreEqual(2, size2.Width);
            Assert.AreEqual(2, size2.Height);

            Size size3 = size1 / 2;

            Assert.AreEqual(0.5, size3.Width);
            Assert.AreEqual(0.5, size3.Height);

            Size size4 = size2 + size3;

            Assert.AreEqual(2.5, size4.Width);
            Assert.AreEqual(2.5, size4.Height);
        }

        [TestMethod]
        public void SizeBoundsTest()
        {
            Size size1 = new Size(2, 2).Bounds(Size.FromWidth(3), Size.FromHeight(1));

            Assert.AreEqual(3, size1.Width);
            Assert.AreEqual(1, size1.Height);

            Size size2 = Size.Empty.Bounds(Size.FromWidth(2), Size.FromHeight(1));

            Assert.AreEqual(2, size2.Width);
            Assert.AreEqual(1, size2.Height);

            Size size3 = new Size(2, 2).Min(Size.FromWidth(1));

            Assert.AreEqual(1, size3.Width);
            Assert.AreEqual(2, size3.Height);

            Size size4 = Size.FromHeight(1).Max(Size.FromWidth(1));

            Assert.AreEqual(1, size4.Width);
            Assert.AreEqual(1, size4.Height);

            Size size5 = new Size(2, 2).Min(new Size(1, 3));

            Assert.AreEqual(1, size5.Width);
            Assert.AreEqual(2, size5.Height);

            Size size6 = Size.FromWidth(1).Combine(Size.FromHeight(1));

            Assert.AreEqual(1, size6.Width);
            Assert.AreEqual(1, size6.Height);
        }
    }
}
