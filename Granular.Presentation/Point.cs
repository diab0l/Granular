using System;
using System.Collections.Generic;
using System.Linq;
using Granular.Extensions;

namespace System.Windows
{
    public sealed class Point
    {
        public static readonly Point Empty = new Point();
        public static readonly Point Zero = new Point(0, 0);

        public double X { get; private set; }
        public double Y { get; private set; }

        public bool IsEmpty { get; private set; }

        private Point()
        {
            X = Double.NaN;
            Y = Double.NaN;
            IsEmpty = true;
        }

        public Point(double x, double y)
        {
            if (x.IsNaN() || y.IsNaN())
            {
                throw new Granular.Exception("Can't create point with NaN values");
            }

            this.X = x;
            this.Y = y;
            IsEmpty = false;
        }

        public override string ToString()
        {
            return String.Format("Point({0}, {1})", X, Y);
        }

        public override bool Equals(object obj)
        {
            Point other = obj as Point;

            return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
                Granular.Compatibility.EqualityComparer<double>.Default.Equals(this.X, other.X) &&
                Granular.Compatibility.EqualityComparer<double>.Default.Equals(this.Y, other.Y);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static bool operator ==(Point point1, Point point2)
        {
            return Object.ReferenceEquals(point1, null) ? Object.ReferenceEquals(point2, null) : point1.Equals(point2);
        }

        public static bool operator !=(Point point1, Point point2)
        {
            return !(point1 == point2);
        }

        public static Point operator -(Point point)
        {
            return new Point(-point.X, -point.Y);
        }

        public static Point operator +(Point point1, Point point2)
        {
            return new Point(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static Point operator -(Point point1, Point point2)
        {
            return new Point(point1.X - point2.X, point1.Y - point2.Y);
        }

        public static Point operator *(Point point, double scalar)
        {
            return new Point(point.X * scalar, point.Y * scalar);
        }

        public static Point operator *(double scalar, Point point)
        {
            return point * scalar;
        }

        public static Point operator /(Point point, double scalar)
        {
            return new Point(point.X / scalar, point.Y / scalar);
        }

        public static Point Parse(string value)
        {
            double[] values = value.Split(',').Select(v => Double.Parse(v)).ToArray();

            if (values.Length == 2)
            {
                return new Point(values[0], values[1]);
            }

            throw new Granular.Exception("Can't parse Point value \"{0}\"", value);
        }

        public static bool IsNullOrEmpty(Point point)
        {
            return ReferenceEquals(point, null) || point.IsEmpty;
        }
    }

    public static class PointExtensions
    {
        public static bool IsNullOrEmpty(this Point point)
        {
            return Point.IsNullOrEmpty(point);
        }

        public static Point DefaultIfNullOrEmpty(this Point point, Point defaultValue = null)
        {
            return Point.IsNullOrEmpty(point) ? (defaultValue ?? Point.Zero) : point;
        }

        public static bool IsClose(this Point @this, Point point)
        {
            return @this.X.IsClose(point.X) && @this.Y.IsClose(point.Y);
        }

        public static Point Min(this Point @this, Point point)
        {
            if (@this.IsEmpty)
            {
                return point;
            }

            if (point.IsEmpty)
            {
                return @this;
            }

            if (@this.X < point.X && @this.Y < point.Y)
            {
                return @this;
            }

            if (@this.X >= point.X && @this.Y >= point.Y)
            {
                return point;
            }

            return new Point(Math.Min(@this.X, point.X), Math.Min(@this.Y, point.Y));
        }

        public static Point Max(this Point @this, Point point)
        {
            if (@this.IsEmpty)
            {
                return point;
            }

            if (point.IsEmpty)
            {
                return @this;
            }

            if (@this.X > point.X && @this.Y > point.Y)
            {
                return @this;
            }

            if (@this.X <= point.X && @this.Y <= point.Y)
            {
                return point;
            }

            return new Point(Math.Max(@this.X, point.X), Math.Max(@this.Y, point.Y));
        }

        public static Point Bounds(this Point point, Point minimum, Point maximum)
        {
            if (minimum.X > maximum.X || minimum.Y > maximum.Y)
            {
                throw new Granular.Exception("Invalid bounds (minimum: {0}, maximum: {1})", minimum, maximum);
            }

            return point.Max(minimum).Min(maximum);
        }
    }
}
