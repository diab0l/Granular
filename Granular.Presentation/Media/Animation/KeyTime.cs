using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media.Animation
{
    public enum KeyTimeType
    {
        Uniform,
        Percent,
        TimeSpan,
        Paced
    }

    public sealed class KeyTime
    {
        public static readonly KeyTime Paced = new KeyTime(KeyTimeType.Paced, TimeSpan.Zero, Double.NaN);
        public static readonly KeyTime Uniform = new KeyTime(KeyTimeType.Uniform, TimeSpan.Zero, Double.NaN);

        public KeyTimeType Type { get; private set; }
        public TimeSpan TimeSpan { get; private set; }
        public double Percent { get; private set; }

        public bool HasTimeSpan { get { return Type == KeyTimeType.TimeSpan; } }
        public bool HasPercent { get { return Type == KeyTimeType.Percent; } }
        public bool IsPaced { get { return Type == KeyTimeType.Paced; } }
        public bool IsUniform { get { return Type == KeyTimeType.Uniform; } }

        private KeyTime(KeyTimeType type, TimeSpan timeSpan, double percent)
        {
            this.Type = type;
            this.TimeSpan = timeSpan;
            this.Percent = percent;
        }

        public override bool Equals(object obj)
        {
            KeyTime other = obj as KeyTime;

            return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
                Object.Equals(this.Type, other.Type) &&
                Object.Equals(this.TimeSpan, other.TimeSpan) &&
                Object.Equals(this.Percent, other.Percent);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() ^ TimeSpan.GetHashCode() ^ Percent.GetHashCode();
        }

        public static KeyTime FromTimeSpan(TimeSpan timeSpan)
        {
            return new KeyTime(KeyTimeType.TimeSpan, timeSpan, Double.NaN);
        }

        public static KeyTime FromPercent(double percent)
        {
            return new KeyTime(KeyTimeType.Percent, TimeSpan.Zero, percent);
        }

        public static KeyTime Parse(string value)
        {
            value = value.Trim();

            if (value == "Paced")
            {
                return KeyTime.Paced;
            }

            if (value == "Uniform")
            {
                return KeyTime.Uniform;
            }

            if (value.EndsWith("%"))
            {
                double percent;
                if (Granular.Compatibility.Double.TryParse(value.Substring(0, value.Length - 1), out percent))
                {
                    return FromPercent(percent / 100);
                }
            }

            TimeSpan timeSpan;
            if (Granular.Compatibility.TimeSpan.TryParse(value, out timeSpan))
            {
                return KeyTime.FromTimeSpan(timeSpan);
            }

            throw new Granular.Exception("Can't parse KeyTime value \"{0}\"", value);
        }
    }
}
