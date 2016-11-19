using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace System.Windows.Media.Animation
{
    [TypeConverter(typeof(RepeatBehaviorTypeConverter))]
    public class RepeatBehavior
    {
        public static readonly RepeatBehavior Forever = new RepeatBehavior(Double.PositiveInfinity, TimeSpan.Zero);
        public static readonly RepeatBehavior OneTime = new RepeatBehavior(1, TimeSpan.Zero);

        public double Count { get; private set; }
        public TimeSpan Duration { get; private set; }

        private RepeatBehavior(double count, TimeSpan duration)
        {
            this.Count = count;
            this.Duration = duration;
        }

        public static RepeatBehavior FromRepeatCount(double count)
        {
            return new RepeatBehavior(count, TimeSpan.Zero);
        }

        public static RepeatBehavior FromTimeSpan(TimeSpan timeSpan)
        {
            return new RepeatBehavior(Double.NaN, timeSpan);
        }

        public static RepeatBehavior Parse(string value)
        {
            value = value.Trim();

            if (value == "Forever")
            {
                return RepeatBehavior.Forever;
            }

            if (value == "OneTime")
            {
                return RepeatBehavior.OneTime;
            }

            if (value.EndsWith("x"))
            {
                double count;
                if (Granular.Compatibility.Double.TryParse(value.Substring(0, value.Length - 1), out count))
                {
                    return RepeatBehavior.FromRepeatCount(count);
                }
            }

            TimeSpan timeSpan;
            if (Granular.Compatibility.TimeSpan.TryParse(value, out timeSpan))
            {
                return RepeatBehavior.FromTimeSpan(timeSpan);
            }

            throw new Granular.Exception("Can't parse RepeatBehavior value \"{0}\"", value);
        }
    }

    public class RepeatBehaviorTypeConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, object value)
        {
            return RepeatBehavior.Parse(value.ToString().Trim());
        }
    }
}
