using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace System.Windows
{
    [TypeConverter(typeof(DurationTypeConverter))]
    public sealed class Duration
    {
        private enum DurationType { Automatic, TimeSpan, Forever }

        public static readonly Duration Automatic = new Duration(DurationType.Automatic, TimeSpan.Zero);
        public static readonly Duration Forever = new Duration(DurationType.Forever, TimeSpan.Zero);

        public TimeSpan TimeSpan { get; private set; }

        public bool IsAutomatic { get { return durationType == DurationType.Automatic; } }
        public bool IsForever { get { return durationType == DurationType.Forever; } }
        public bool HasTimeSpan { get { return durationType == DurationType.TimeSpan; } }

        private DurationType durationType;

        public Duration(TimeSpan timeSpan) :
            this(DurationType.TimeSpan, timeSpan)
        {
            //
        }

        private Duration(DurationType durationType, TimeSpan timeSpan)
        {
            this.TimeSpan = timeSpan;
            this.durationType = durationType;
        }

        public override bool Equals(object obj)
        {
            Duration other = obj as Duration;

            return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
                this.durationType == other.durationType && this.TimeSpan == other.TimeSpan;
        }

        public override int GetHashCode()
        {
            return durationType.GetHashCode() ^ TimeSpan.GetHashCode();
        }

        public override string ToString()
        {
            if (IsAutomatic)
            {
                return "Automatic";
            }

            if (IsForever)
            {
                return "Forever";
            }

            return TimeSpan.ToString();
        }

        public static Duration Parse(string value)
        {
            value = value.Trim();

            if (value == "Automatic")
            {
                return Duration.Automatic;
            }

            if (value == "Forever")
            {
                return Duration.Forever;
            }

            TimeSpan timeSpan;
            if (Granular.Compatibility.TimeSpan.TryParse(value, out timeSpan))
            {
                return new Duration(timeSpan);
            }

            throw new Granular.Exception("Can't parse Duration value \"{0}\"", value);
        }
    }

    public class DurationTypeConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, object value)
        {
            return Duration.Parse(value.ToString().Trim());
        }
    }
}
