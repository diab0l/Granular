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

        public bool IsIdentity { get { return M11 == 1 && M12 == 0 && M21 == 0 && M22 == 1 && OffsetX == 0 && OffsetY == 0; } }
        public bool IsTranslation { get { return M11 == 1 && M12 == 0 && M21 == 0 && M22 == 1; } }
        public bool IsScaling { get { return M12 == 0 && M21 == 0 && OffsetX == 0 && OffsetY == 0; } }

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

        public static bool operator ==(Matrix matrix1, Matrix matrix2)
        {
            return Object.ReferenceEquals(matrix1, null) ? Object.ReferenceEquals(matrix2, null) : matrix1.Equals(matrix2);
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
}
