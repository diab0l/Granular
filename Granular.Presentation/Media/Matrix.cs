using System;
using System.Collections.Generic;
using System.Linq;
using Granular.Extensions;
using System.Windows.Markup;
using System.Xaml;

namespace System.Windows.Media
{
    [TypeConverter(typeof(MatrixTypeConverter))]
    public sealed class Matrix
    {
        public static readonly Matrix Identity = new Matrix(1, 0, 0, 1, 0, 0);

        public double M11 { get; private set; }
        public double M12 { get; private set; }
        public double M21 { get; private set; }
        public double M22 { get; private set; }
        public double OffsetX { get; private set; }
        public double OffsetY { get; private set; }

        public bool IsIdentity { get { return ReferenceEquals(this, Identity) || M11 == 1 && M12 == 0 && M21 == 0 && M22 == 1 && OffsetX == 0 && OffsetY == 0; } }
        public bool IsTranslation { get { return M11 == 1 && M12 == 0 && M21 == 0 && M22 == 1; } }
        public bool IsScaling { get { return M12 == 0 && M21 == 0 && OffsetX == 0 && OffsetY == 0; } }

        private Matrix inverse;
        public Matrix Inverse
        {
            get
            {
                if (inverse == null)
                {
                    inverse = GetInverseMatrix();
                }

                return inverse;
            }
        }

        // (m11 m12 0)
        // (m21 m22 0)
        // (ofx ofy 1)
        public Matrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M21 = m21;
            this.M22 = m22;
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
        }

        public override string ToString()
        {
            if (IsIdentity)
            {
                return "IdentityMatrix";
            }

            if (IsTranslation)
            {
                return String.Format("TranslationMatrix({0}, {1})", OffsetX, OffsetY);
            }

            if (IsScaling)
            {
                return String.Format("ScalingMatrix({0}, {1})", M11, M22);
            }

            return String.Format("Matrix({0}, {1}, {2}, {3}, {4}, {5})", M11, M12, M21, M22, OffsetX, OffsetY);
        }

        public override bool Equals(object obj)
        {
            Matrix other = obj as Matrix;

            return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
                Granular.Compatibility.EqualityComparer.Double.Equals(this.M11, other.M11) &&
                Granular.Compatibility.EqualityComparer.Double.Equals(this.M12, other.M12) &&
                Granular.Compatibility.EqualityComparer.Double.Equals(this.M21, other.M21) &&
                Granular.Compatibility.EqualityComparer.Double.Equals(this.M22, other.M22) &&
                Granular.Compatibility.EqualityComparer.Double.Equals(this.OffsetX, other.OffsetX) &&
                Granular.Compatibility.EqualityComparer.Double.Equals(this.OffsetY, other.OffsetY);
        }

        public override int GetHashCode()
        {
            return M11.GetHashCode() ^ M12.GetHashCode() ^
                M21.GetHashCode() ^ M22.GetHashCode() ^
                OffsetX.GetHashCode() ^ OffsetY.GetHashCode();
        }

        public bool IsClose(Matrix matrix)
        {
            return this.M11.IsClose(matrix.M11) && this.M12.IsClose(matrix.M12) &&
                this.M21.IsClose(matrix.M21) && this.M22.IsClose(matrix.M22) &&
                this.OffsetX.IsClose(matrix.OffsetX) && this.OffsetY.IsClose(matrix.OffsetY);
        }

        private Matrix GetInverseMatrix()
        {
            double determinant = M11 * M22 - M12 * M21;
            return new Matrix(M22 / determinant, -M12 / determinant, -M21 / determinant, M11 / determinant,
                              (M21 * OffsetY - M22 * OffsetX) / determinant, -(M11 * OffsetY - M12 * OffsetX) / determinant);
        }

        public static bool operator ==(Matrix matrix1, Matrix matrix2)
        {
            return Object.Equals(matrix1, matrix2);
        }

        public static bool operator !=(Matrix matrix1, Matrix matrix2)
        {
            return !(matrix1 == matrix2);
        }

        // (m11 m12 0)   (m11 m12 0)
        // (m21 m22 0) * (m21 m22 0)
        // (ofx ofy 1)   (ofx ofy 1)
        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.IsIdentity)
            {
                return matrix2;
            }

            if (matrix2.IsIdentity)
            {
                return matrix1;
            }

            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21;
            double m12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22;
            double m21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21;
            double m22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22;
            double offsetX = matrix1.OffsetX * matrix2.M11 + matrix1.OffsetY * matrix2.M21 + matrix2.OffsetX;
            double offsetY = matrix1.OffsetX * matrix2.M12 + matrix1.OffsetY * matrix2.M22 + matrix2.OffsetY;

            return new Matrix(m11, m12, m21, m22, offsetX, offsetY);
        }

        //             (m11 m12 0)
        // (px py 1) * (m21 m22 0)
        //             (ofx ofy 1)
        public static Point operator *(Point point, Matrix matrix)
        {
            if (matrix.IsIdentity)
            {
                return point;
            }

            double x = point.X * matrix.M11 + point.Y * matrix.M21 + matrix.OffsetX;
            double y = point.X * matrix.M12 + point.Y * matrix.M22 + matrix.OffsetY;

            return new Point(x, y);
        }

        public static Matrix RotationMatrix(double radians, double centerX = 0, double centerY = 0)
        {
            double sin = Math.Sin(radians);
            double cos = Math.Cos(radians);
            double offsetX = (centerX * (1.0 - cos)) + (centerY * sin);
            double offsetY = (centerY * (1.0 - cos)) - (centerX * sin);

            return new Matrix(cos, sin, -sin, cos, offsetX, offsetY);
        }

        public static Matrix ScalingMatrix(double scaleX, double scaleY, double centerX = 0, double centerY = 0)
        {
            double offsetX = centerX - scaleX * centerX;
            double offsetY = centerY - scaleY * centerY;

            return new Matrix(scaleX, 0, 0, scaleY, offsetX, offsetY);
        }

        public static Matrix SkewMatrix(double radiansX, double radiansY, double centerX = 0, double centerY = 0)
        {
            double offsetX = -centerY * Math.Tan(radiansX);
            double offsetY = -centerX * Math.Tan(radiansY);

            return new Matrix(1, Math.Tan(radiansY), Math.Tan(radiansX), 1, offsetX, offsetY);
        }

        public static Matrix TranslationMatrix(double offsetX, double offsetY)
        {
            return new Matrix(1, 0, 0, 1, offsetX, offsetY);
        }

        public static Matrix Parse(string value)
        {
            double[] values = value.Split(',').Select(v => Double.Parse(v)).ToArray();

            if (values.Length == 6)
            {
                return new Matrix(values[0], values[1], values[2], values[3], values[4], values[5]);
            }

            throw new Granular.Exception("Can't parse Matrix value \"{0}\"", value);
        }
    }

    public class MatrixTypeConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, object value)
        {
            return Matrix.Parse(value.ToString().Trim());
        }
    }

    public static class MatrixExtensions
    {
        public static bool IsNullOrIdentity(this Matrix matrix)
        {
            return ReferenceEquals(matrix, null) || matrix.IsIdentity;
        }

        public static Rect GetContainingRect(this Matrix matrix, Rect rect)
        {
            IEnumerable<Point> vertices = rect.GetCorners().Select(corner => corner * matrix).ToArray();

            double left = vertices.Select(vertex => vertex.X).Min();
            double right = vertices.Select(vertex => vertex.X).Max();
            double top = vertices.Select(vertex => vertex.Y).Min();
            double bottom = vertices.Select(vertex => vertex.Y).Max();

            return new Rect(left, top, right - left, bottom - top);
        }

        public static Rect GetApproximatedRect(this Matrix matrix, Size size)
        {
            return matrix.GetApproximatedRect(new Rect(size));
        }

        public static Rect GetApproximatedRect(this Matrix matrix, Rect rect)
        {
            IEnumerable<Point> vertices = rect.GetCorners().Select(corner => corner * matrix).ToArray();

            double[] verticesX = vertices.Select(vertex => vertex.X).OrderBy(x => x).ToArray();
            double[] verticesY = vertices.Select(vertex => vertex.Y).OrderBy(y => y).ToArray();

            double left = (verticesX[0] + verticesX[1]) / 2;
            double right = (verticesX[2] + verticesX[3]) / 2;
            double top = (verticesY[0] + verticesY[1]) / 2;
            double bottom = (verticesY[2] + verticesY[3]) / 2;

            return new Rect(left, top, right - left, bottom - top);
        }

        public static Size GetContainingSize(this Matrix matrix, Size size)
        {
            return matrix.GetContainingRect(new Rect(size)).Size;
        }

        public static Size GetContainedSize(this Matrix matrix, Size containerSize)
        {
            double w = containerSize.Width;
            double h = containerSize.Height;

            // Each width unit of the contained size, will add "a" units to the transformed width and "b" units to the transformed height
            Point transformedWidth = (new Point(1, 0) * matrix).Abs();
            double a = transformedWidth.X;
            double b = transformedWidth.Y;

            // Each height unit of the contained size, will add "c" units to the transformed width and "d" units to the transformed height
            Point transformedHeight = (new Point(0, 1) * matrix).Abs();
            double c = transformedHeight.X;
            double d = transformedHeight.Y;

            if (a == 0 && c == 0 || b == 0 && d == 0)
            {
                return Size.Zero;
            }

            // Find a contained size (x, y) with maximum area (x * y) where
            //      w >= a * x + c * y
            //      h >= b * x + d * y
            //
            // The solution is on one of these constrains egeds (where the area derivative is zero) or in the intersection
            //
            // The area on the first constrain edge is:
            //      area1(x) = x * (w - a * x) / c
            //
            // The maximum is at:
            //      area1'(x) = (w - 2 * a * x) / c = 0
            //      x = w / (2 * a)
            //      y = Min((w - a * x) / c, (h - b * x) / d)

            double determinant = a * d - b * c;

            // Intersection size
            Size size0 = determinant != 0 ?
                new Size(((w * d - h * c) / determinant).Max(0), ((h * a - w * b) / determinant).Max(0)) :
                new Size(0, 0);

            Func<double, double> GetConstrainedY = x => Math.Min(c > 0 ? (w - a * x) / c : Double.PositiveInfinity, d > 0 ? (h - b * x) / d : Double.PositiveInfinity);
            Func<double, double> GetConstrainedX = y => Math.Min(a > 0 ? (w - c * y) / a : Double.PositiveInfinity, b > 0 ? (h - d * y) / b : Double.PositiveInfinity);

            // Maximum size on the first constrain edge
            Size size1 = a > c ?
                new Size(w / (2 * a), GetConstrainedY(w / (2 * a)).Max(0)) :
                new Size(GetConstrainedX(w / (2 * c)).Max(0), w / (2 * c));

            // Maximum size on the second constrain edge
            Size size2 = b > d ?
                new Size(h / (2 * b), GetConstrainedY(h / (2 * b)).Max(0)) :
                new Size(GetConstrainedX(h / (2 * d)).Max(0), h / (2 * d));

            return size0.MaxArea(size1).MaxArea(size2);
        }
    }
}
