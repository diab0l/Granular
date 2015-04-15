using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Granular.Extensions;

namespace System.Windows
{
    public sealed class Rect
    {
        public static readonly Rect Empty = new Rect(Point.Empty, Size.Empty);
        public static readonly Rect Zero = new Rect(Size.Zero);

        public Point Location { get; private set; }
        public Size Size { get; private set; }

        public double Left { get; private set; }
        public double Top { get; private set; }
        public double Right { get; private set; }
        public double Bottom { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }

        public bool IsEmpty { get; private set; }

        public Rect(double width, double height) :
            this(Point.Zero, new Size(width, height))
        {
            //
        }

        public Rect(double left, double top, double width, double height) :
            this(new Point(left, top), new Size(width, height))
        {
            //
        }

        public Rect(Size size) :
            this(Point.Zero, size)
        {
            //
        }

        public Rect(Point location, Size size)
        {
            if (location.IsNullOrEmpty() || size.IsNullOrEmpty())
            {
                this.IsEmpty = true;

                this.Location = Point.Empty;
                this.Size = Size.Empty;

                this.Left = Double.NaN;
                this.Top = Double.NaN;
                this.Right = Double.NaN;
                this.Bottom = Double.NaN;
                this.Width = Double.NaN;
                this.Height = Double.NaN;
            }
            else
            {
                this.IsEmpty = false;

                this.Location = location;
                this.Size = size;

                this.Left = Location.X;
                this.Top = Location.Y;
                this.Right = Size.Width + Location.X;
                this.Bottom = Size.Height + Location.Y;
                this.Width = Size.Width;
                this.Height = Size.Height;
            }
        }

        public override string ToString()
        {
            return String.Format("Rect({0}, {1}, {2}, {3})", Left, Top, Width, Height);
        }

        public override bool Equals(object obj)
        {
            Rect other = obj as Rect;

            return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
                Object.Equals(this.Location, other.Location) &&
                Object.Equals(this.Size, other.Size);
        }

        public override int GetHashCode()
        {
            return Location.GetHashCode() ^ Size.GetHashCode();
        }

        public static bool operator ==(Rect rect1, Rect rect2)
        {
            return Object.Equals(rect1, rect2);
        }

        public static bool operator !=(Rect rect1, Rect rect2)
        {
            return !(rect1 == rect2);
        }

        public static bool IsNullOrEmpty(Rect rect)
        {
            return ReferenceEquals(rect, null) || rect.IsEmpty;
        }

        public static Rect Parse(string value)
        {
            double[] values = value.Split(',').Select(v => Double.Parse(v)).ToArray();

            if (values.Length == 2)
            {
                return new Rect(values[0], values[1]);
            }

            if (values.Length == 4)
            {
                return new Rect(values[0], values[1], values[2], values[3]);
            }

            throw new Granular.Exception("Can't parse Rect value \"{0}\"", value);
        }
    }

    public static class RectExtensions
    {
        public static bool IsNullOrEmpty(this Rect rect)
        {
            return rect == null || rect.IsEmpty;
        }

        public static Rect DefaultIfNullOrEmpty(this Rect rect, Rect defaultValue = null)
        {
            return rect.IsNullOrEmpty() ? (defaultValue ?? Rect.Zero) : rect;
        }

        public static bool IsClose(this Rect @this, Rect rect)
        {
            return @this.Location.IsClose(rect.Location) && @this.Size.IsClose(rect.Size);
        }

        public static bool Contains(this Rect rect, Point point)
        {
            return rect.Left <= point.X && point.X < rect.Right && rect.Top <= point.Y && point.Y < rect.Bottom;
        }

        public static Rect Transform(this Rect rect, Matrix matrix)
        {
            Point topLeft = rect.Location * matrix;
            Point topRight = new Point(rect.Left + rect.Width, rect.Top) * matrix;
            Point bottomLeft = new Point(rect.Left, rect.Top + rect.Height) * matrix;
            Point bottomRight = new Point(rect.Left + rect.Width, rect.Top + rect.Height) * matrix;

            Point location = new Point(
                topLeft.X.Min(topRight.X).Min(bottomLeft.X).Min(bottomRight.X),
                topLeft.Y.Min(topRight.Y).Min(bottomLeft.Y).Min(bottomRight.Y));

            Size size = new Size(
                topLeft.X.Max(topRight.X).Max(bottomLeft.X).Max(bottomRight.X) - location.X,
                topLeft.Y.Max(topRight.Y).Max(bottomLeft.Y).Max(bottomRight.Y) - location.Y);

            return new Rect(location, size);
        }

        public static Point GetTopLeft(this Rect rect)
        {
            return rect.Location;
        }

        public static Point GetTopRight(this Rect rect)
        {
            return new Point(rect.Location.X + rect.Size.Width, rect.Location.Y);
        }

        public static Point GetBottomLeft(this Rect rect)
        {
            return new Point(rect.Location.X, rect.Location.Y + rect.Size.Height);
        }

        public static Point GetBottomRight(this Rect rect)
        {
            return new Point(rect.Location.X + rect.Size.Width, rect.Location.Y + rect.Size.Height);
        }

        public static Rect AddOffset(this Rect rect, Point offset)
        {
            return new Rect(rect.Left + offset.X, rect.Top + offset.Y, rect.Width, rect.Height);
        }

        public static Rect AddMargin(this Rect rect, Thickness margin)
        {
            return new Rect(rect.Location - margin.Location, rect.Size + margin.Size);
        }
    }
}
