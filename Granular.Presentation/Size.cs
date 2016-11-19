using System;
using System.Collections.Generic;
using System.Linq;
using Granular.Extensions;
using System.Windows.Markup;

namespace System.Windows
{
    [TypeConverter(typeof(SizeTypeConverter))]
    public sealed class Size
    {
        public static readonly Size Empty = new Size(Double.NaN, Double.NaN);
        public static readonly Size Zero = new Size(0, 0);
        public static readonly Size Infinity = new Size(Double.PositiveInfinity, Double.PositiveInfinity);

        public double Width { get; private set; }
        public double Height { get; private set; }

        public bool IsWidthEmpty { get; private set; }
        public bool IsHeightEmpty { get; private set; }
        public bool IsEmpty { get; private set; }
        public bool IsPartiallyEmpty { get; private set; }

        public Size(double width, double height)
        {
            this.Width = width;
            this.Height = height;

            this.IsWidthEmpty = Double.IsNaN(Width);
            this.IsHeightEmpty = Double.IsNaN(Height);
            this.IsEmpty = IsWidthEmpty && IsHeightEmpty;
            this.IsPartiallyEmpty = IsWidthEmpty || IsHeightEmpty;
        }

        public override string ToString()
        {
            return String.Format("Size({0}, {1})", Width, Height);
        }

        public override bool Equals(object obj)
        {
            Size other = obj as Size;

            return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
                Granular.Compatibility.EqualityComparer.Double.Equals(this.Width, other.Width) &&
                Granular.Compatibility.EqualityComparer.Double.Equals(this.Height, other.Height);
        }

        public override int GetHashCode()
        {
            return Width.GetHashCode() ^ Height.GetHashCode();
        }

        public static Size FromWidth(double width)
        {
            return new Size(width, Double.NaN);
        }

        public static Size FromHeight(double height)
        {
            return new Size(Double.NaN, height);
        }

        public static bool operator ==(Size size1, Size size2)
        {
            return Object.Equals(size1, size2);
        }

        public static bool operator !=(Size size1, Size size2)
        {
            return !(size1 == size2);
        }

        public static Size operator -(Size size)
        {
            if (size == Size.Zero)
            {
                return size;
            }

            return new Size(-size.Width, -size.Height);
        }

        public static Size operator +(Size size1, Size size2)
        {
            if (size1 == Size.Zero)
            {
                return size2;
            }

            if (size2 == Size.Zero)
            {
                return size1;
            }

            return new Size(size1.Width + size2.Width, size1.Height + size2.Height);
        }

        public static Size operator -(Size size1, Size size2)
        {
            if (size1 == Size.Zero)
            {
                return -size2;
            }

            if (size2 == Size.Zero)
            {
                return size1;
            }

            return new Size(size1.Width - size2.Width, size1.Height - size2.Height);
        }

        public static Size operator *(Size size, double factor)
        {
            if (factor == 1 || ReferenceEquals(size, Size.Zero))
            {
                return size;
            }

            return new Size(size.Width * factor, size.Height * factor);
        }

        public static Size operator *(double factor, Size size)
        {
            return size * factor;
        }

        public static Size operator /(Size size, double factor)
        {
            if (factor == 1 || ReferenceEquals(size, Size.Zero))
            {
                return size;
            }

            return new Size(size.Width / factor, size.Height / factor);
        }

        public static Size Parse(string value)
        {
            double[] values = value.Split(',').Select(v => Double.Parse(v)).ToArray();

            if (values.Length == 2)
            {
                return new Size(values[0], values[1]);
            }

            throw new Granular.Exception("Can't parse Size value \"{0}\"", value);
        }

        public static bool IsNullOrEmpty(Size size)
        {
            return ReferenceEquals(size, null) || size.IsEmpty;
        }
    }


    public static class SizeExtensions
    {
        public static bool IsNullOrEmpty(this Size size)
        {
            return Size.IsNullOrEmpty(size);
        }

        public static Size DefaultIfNullOrEmpty(this Size size, Size defaultValue = null)
        {
            return Size.IsNullOrEmpty(size) ? (defaultValue ?? Size.Zero) : size;
        }

        public static Size Combine(this Size size, Size fallback)
        {
            if (!size.IsPartiallyEmpty)
            {
                return size;
            }

            if (size.IsEmpty)
            {
                return fallback;
            }

            return new Size(
                size.IsWidthEmpty ? fallback.Width : size.Width,
                size.IsHeightEmpty ? fallback.Height : size.Height);
        }

        public static Size Min(this Size @this, Size size)
        {
            if (@this.IsEmpty)
            {
                return size;
            }

            if (size.IsEmpty)
            {
                return @this;
            }

            if (!@this.IsPartiallyEmpty && !@size.IsPartiallyEmpty)
            {
                if (@this.Width < size.Width && @this.Height < size.Height)
                {
                    return @this;
                }

                if (@this.Width >= size.Width && @this.Height >= size.Height)
                {
                    return size;
                }
            }

            return new Size(
                @this.IsWidthEmpty ? size.Width : (size.IsWidthEmpty ? @this.Width : Math.Min(@this.Width, size.Width)),
                @this.IsHeightEmpty ? size.Height : (size.IsHeightEmpty ? @this.Height : Math.Min(@this.Height, size.Height)));
        }

        public static Size Max(this Size @this, Size size)
        {
            if (@this.IsEmpty)
            {
                return size;
            }

            if (size.IsEmpty)
            {
                return @this;
            }

            if (!@this.IsPartiallyEmpty && !@size.IsPartiallyEmpty)
            {
                if (@this.Width > size.Width && @this.Height > size.Height)
                {
                    return @this;
                }

                if (@this.Width <= size.Width && @this.Height <= size.Height)
                {
                    return size;
                }
            }

            return new Size(
                @this.IsWidthEmpty ? size.Width : (size.IsWidthEmpty ? @this.Width : Math.Max(@this.Width, size.Width)),
                @this.IsHeightEmpty ? size.Height : (size.IsHeightEmpty ? @this.Height : Math.Max(@this.Height, size.Height)));
        }

        public static Size Bounds(this Size size, Size minimum, Size maximum)
        {
            if (minimum.Width > maximum.Width || minimum.Height > maximum.Height)
            {
                throw new Granular.Exception("Invalid bounds (minimum: {0}, maximum: {1})", minimum, maximum);
            }

            return size.Max(minimum).Min(maximum);
        }

        public static bool IsClose(this Size @this, Size size)
        {
            return @this.Width.IsClose(size.Width) && @this.Height.IsClose(size.Height);
        }

        public static Point ToPoint(this Size size)
        {
            return new Point(size.Width, size.Height);
        }

        public static bool Contains(this Size size, Point point)
        {
            return 0 <= point.X && point.X < size.Width && 0 <= point.Y && point.Y < size.Height;
        }

        public static Size MaxArea(this Size @this, Size size)
        {
            return @this.Width * @this.Height > size.Width * size.Height ? @this : size;
        }
    }

    public class SizeTypeConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, object value)
        {
            return Size.Parse(value.ToString().Trim());
        }
    }
}
