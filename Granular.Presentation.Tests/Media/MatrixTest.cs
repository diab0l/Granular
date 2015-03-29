using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Media
{
    [TestClass]
    public class MatrixTest
    {
        [TestMethod]
        public void MatrixTypeTest()
        {
            Assert.IsTrue(Matrix.Identity.IsIdentity);
            Assert.IsTrue(Matrix.Identity.IsTranslation);
            Assert.IsTrue(Matrix.Identity.IsScaling);
            Assert.IsTrue(Matrix.TranslationMatrix(2, 3).IsTranslation);
            Assert.IsTrue(Matrix.ScalingMatrix(2, 3).IsScaling);

            Matrix matrix = new Matrix(1, 2, 3, 4, 5, 6);
            Assert.IsTrue(!matrix.IsIdentity);
            Assert.IsTrue(!matrix.IsTranslation);
            Assert.IsTrue(!matrix.IsScaling);
        }

        [TestMethod]
        public void MatrixMultiplyTest()
        {
            Matrix matrix1 = new Matrix(1, 2, 3, 4, 5, 6);
            Assert.AreEqual(matrix1, matrix1 * Matrix.Identity);

            Assert.AreEqual(new Point(36, 52), new Point(7, 8) * matrix1);

            Matrix matrix2 = new Matrix(7, 8, 9, 10, 11, 12);
            Assert.AreEqual(new Matrix(25, 28, 57, 64, 100, 112), matrix1 * matrix2);
        }

        [TestMethod]
        public void MatrixConstructorTest()
        {
            Matrix offset = Matrix.TranslationMatrix(1, 2);
            Assert.AreEqual(new Matrix(1, 0, 0, 1, 1, 2), offset);

            Matrix scale = Matrix.ScalingMatrix(3, 4);
            Assert.AreEqual(new Matrix(3, 0, 0, 4, 0, 0), scale);

            Matrix scaleOffset = Matrix.ScalingMatrix(3, 4, 1, 2);
            Assert.AreEqual((Matrix.TranslationMatrix(-1, -2) * Matrix.ScalingMatrix(3, 4) * Matrix.TranslationMatrix(1, 2)), scaleOffset);

            double angle = Math.PI / 6;
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);

            Matrix rotate = Matrix.RotationMatrix(angle);
            Assert.AreEqual(new Matrix(cos, sin, -sin, cos, 0, 0), rotate);

            Matrix rotateOffset = Matrix.RotationMatrix(angle, 1, 2);
            Assert.IsTrue((Matrix.TranslationMatrix(-1, -2) * Matrix.RotationMatrix(angle) * Matrix.TranslationMatrix(1, 2)).IsClose(rotateOffset));

            double angleX = Math.PI / 6;
            double angleY = 2 * Math.PI / 6;
            double tanX = Math.Tan(angleX);
            double tanY = Math.Tan(angleY);

            Matrix skew = Matrix.SkewMatrix(angleX, angleY);
            Assert.AreEqual(new Matrix(1, tanY, tanX, 1, 0, 0), skew);

            Matrix skewOffset = Matrix.SkewMatrix(angleX, angleY, 1, 2);
            Assert.IsTrue((Matrix.TranslationMatrix(-1, -2) * Matrix.SkewMatrix(angleX, angleY) * Matrix.TranslationMatrix(1, 2)).IsClose(skewOffset));
        }
    }
}
