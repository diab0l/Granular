using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows
{
    public sealed class CornerRadius
    {
        public static readonly CornerRadius Zero = new CornerRadius(0);

        public double TopLeft { get; private set; }
        public double TopRight { get; private set; }
        public double BottomRight { get; private set; }
        public double BottomLeft { get; private set; }

        public bool IsUniform { get; private set; }

        public CornerRadius(double uniformRadius) :
            this(uniformRadius, uniformRadius, uniformRadius, uniformRadius)
        {
            //
        }

        public CornerRadius(double topLeft, double topRight, double bottomRight, double bottomLeft)
        {
            this.TopLeft = topLeft;
            this.TopRight = topRight;
            this.BottomRight = bottomRight;
            this.BottomLeft = bottomLeft;

            this.IsUniform = TopLeft == TopRight && TopLeft == BottomRight && TopLeft == BottomLeft;
        }

        public override string ToString()
        {
            return IsUniform ?
                String.Format("CornerRadius({0})", TopLeft) :
                String.Format("CornerRadius({0}, {1}, {2}, {3})", TopLeft, TopRight, BottomRight, BottomLeft);
        }

        public override bool Equals(object obj)
        {
            CornerRadius other = obj as CornerRadius;

            return other != null && this.GetType() == other.GetType() &&
                Granular.Compatibility.EqualityComparer<double>.Default.Equals(this.TopLeft, other.TopLeft) &&
                Granular.Compatibility.EqualityComparer<double>.Default.Equals(this.TopRight, other.TopRight) &&
                Granular.Compatibility.EqualityComparer<double>.Default.Equals(this.BottomRight, other.BottomRight) &&
                Granular.Compatibility.EqualityComparer<double>.Default.Equals(this.BottomLeft, other.BottomLeft);
        }

        public override int GetHashCode()
        {
            return TopLeft.GetHashCode() ^
                TopRight.GetHashCode() ^
                BottomRight.GetHashCode() ^
                BottomLeft.GetHashCode();
        }

        public static bool operator ==(CornerRadius cornerRadius1, CornerRadius cornerRadius2)
        {
            return Object.ReferenceEquals(cornerRadius1, null) ? Object.ReferenceEquals(cornerRadius2, null) : cornerRadius1.Equals(cornerRadius2);
        }

        public static bool operator !=(CornerRadius cornerRadius1, CornerRadius cornerRadius2)
        {
            return !(cornerRadius1 == cornerRadius2);
        }

        public static CornerRadius Parse(string value)
        {
            double[] values = value.Split(',').Select(v => Double.Parse(v)).ToArray();

            if (values.Length == 1)
            {
                return new CornerRadius(values[0]);
            }

            if (values.Length == 4)
            {
                return new CornerRadius(values[0], values[1], values[2], values[3]);
            }

            throw new Granular.Exception("Can't parse CornerRadius value \"{0}\"", value);
        }
    }

    public static class CornerRadiusExtensions
    {
        public static CornerRadius DefaultIfNull(this CornerRadius cornerRadius, CornerRadius defaultValue = null)
        {
            return cornerRadius ?? defaultValue ?? CornerRadius.Zero;
        }
    }
}
