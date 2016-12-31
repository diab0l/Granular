using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace Granular.Compatibility
{
    public class TimeSpan
    {
        public static readonly System.TimeSpan MinValue = System.TimeSpan.FromDays(-10000);
        public static readonly System.TimeSpan MaxValue = System.TimeSpan.FromDays(10000);

        //[ws][-]{[d.]hh:mm[:ss[.ff]]|d}[ws]
        private static readonly Granular.Compatibility.Regex TimeSpanFormatRegex = new Granular.Compatibility.Regex(@"^(-?)(((([0-9]+)\.)?([0-9]+):([0-9]+)(:([0-9]*)(\.([0-9]+))?)?)|([0-9]+))$");
        private const int TimeSpanFormatSignGroupIndex = 1;
        private const int TimeSpanFormatDaysGroupIndex = 5;
        private const int TimeSpanFormatHoursGroupIndex = 6;
        private const int TimeSpanFormatMinutesGroupIndex = 7;
        private const int TimeSpanFormatSecondsGroupIndex = 9;
        private const int TimeSpanFormatMillisecondsGroupIndex = 11;
        private const int TimeSpanFormatDaysAlternativeGroupIndex = 12;

        public static bool TryParse(string s, out System.TimeSpan result)
        {
            int days = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            int milliseconds = 0;

            string[] match = TimeSpanFormatRegex.Match(s);

            if (match == null)
            {
                result = System.TimeSpan.Zero;
                return false;
            }

            if (!Int32.TryParse(match[TimeSpanFormatDaysAlternativeGroupIndex], out days))
            {
                days = Int32.Parse(match[TimeSpanFormatDaysGroupIndex].DefaultIfNullOrEmpty("0"));
                hours = Int32.Parse(match[TimeSpanFormatHoursGroupIndex].DefaultIfNullOrEmpty("0"));
                minutes = Int32.Parse(match[TimeSpanFormatMinutesGroupIndex].DefaultIfNullOrEmpty("0"));
                seconds = Int32.Parse(match[TimeSpanFormatSecondsGroupIndex].DefaultIfNullOrEmpty("0"));
                milliseconds = Int32.Parse(match[TimeSpanFormatMillisecondsGroupIndex].DefaultIfNullOrEmpty("000").PadRight(3, '0'));
            }

            if (hours >= 24 || minutes >= 60 || seconds >= 60 || milliseconds >= 1000)
            {
                result = System.TimeSpan.Zero;
                return false;
            }

            if (match[TimeSpanFormatSignGroupIndex] == "-")
            {
                days = -days;
                hours = -hours;
                minutes = -minutes;
                seconds = -seconds;
                milliseconds = -milliseconds;
            }

            result = new System.TimeSpan(days, hours, minutes, seconds, milliseconds);
            return true;
        }

        public static System.TimeSpan Subtract(DateTime value1, DateTime value2)
        {
            return value1.Subtract(value2);
        }
    }
}
