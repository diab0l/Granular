using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xaml;

namespace System.Windows.Controls
{
    public enum GridUnitType
    {
        Auto,
        Pixel,
        Star
    }

    [TypeConverter(typeof(GridLengthTypeConverter))]
    public class GridLength
    {
        public static readonly GridLength Auto = new GridLength(GridUnitType.Auto, 0);
        public static readonly GridLength Star = new GridLength(GridUnitType.Star, 1);

        public GridUnitType GridUnitType { get; private set; }
        public double Value { get; private set; }

        public bool IsAuto { get { return GridUnitType == GridUnitType.Auto; } }
        public bool IsAbsolute { get { return GridUnitType == GridUnitType.Pixel; } }
        public bool IsStar { get { return GridUnitType == GridUnitType.Star; } }

        private GridLength(GridUnitType gridUnitType, double value)
        {
            this.GridUnitType = gridUnitType;
            this.Value = value;
        }

        public override bool Equals(object obj)
        {
            GridLength other = obj as GridLength;

            return other != null && this.GetType() == other.GetType() &&
                this.GridUnitType == other.GridUnitType &&
                Granular.Compatibility.EqualityComparer<double>.Default.Equals(this.Value, other.Value);
        }

        public override int GetHashCode()
        {
            return GridUnitType.GetHashCode() ^ Value.GetHashCode();
        }

        public override string ToString()
        {
            return IsAuto ? "Auto" : String.Format("{0}{1}", Value, IsAbsolute ? "px" : "*");
        }

        public static GridLength FromPixles(double pixels)
        {
            return new GridLength(GridUnitType.Pixel, pixels);
        }

        public static GridLength FromStars(double stars)
        {
            return new GridLength(GridUnitType.Star, stars);
        }
    }

    internal class GridLengthTypeConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, object value)
        {
            string text = value.ToString().Trim();

            if (text.ToLower() == "auto")
            {
                return GridLength.Auto;
            }

            if (text == "*")
            {
                return GridLength.Star;
            }

            if (text.EndsWith("*"))
            {
                double stars;
                if (Granular.Compatibility.Double.TryParse(text.Substring(0, text.Length - 1), out stars))
                {
                    return GridLength.FromStars(stars);
                }
            }

            double pixels;
            if (Granular.Compatibility.Double.TryParse(text, out pixels))
            {
                return GridLength.FromPixles(pixels);
            }

            throw new Granular.Exception("Can't parse GridLength value \"{0}\"", text);
        }
    }
}
