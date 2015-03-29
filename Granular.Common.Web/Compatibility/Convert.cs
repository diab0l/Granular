using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Compatibility
{
    public static class Convert
    {
        private const string HexDigits = "0123456789ABCDEF";

        public static uint ToUInt32(string value, int fromBase)
        {
            if (fromBase <= 0 || fromBase > HexDigits.Length)
            {
                throw new Granular.Exception("Can't convert to UInt32 from \"{0}\" base", fromBase);
            }

            uint hexValue = 0;

            for (int i = 0; i < value.Length; i++)
            {
                int hexChar = HexDigits.IndexOf(value[i]);

                if (hexChar == -1)
                {
                    throw new Granular.Exception("Can't convert \"{0}\" from base \"{1}\" to UInt32", value, fromBase);
                }

                hexValue = hexValue * (uint)fromBase + (uint)hexChar;
            }

            return hexValue;
        }

        public static object ChangeType(object value, System.Type conversionType)
        {
            if (value is int && conversionType == typeof(double))
            {
                return (double)((int)value);
            }

            if (value is string)
            {
                if (conversionType == typeof(int))
                {
                    return Int32.Parse((string)value);
                }

                if (conversionType == typeof(double))
                {
                    return System.Double.Parse((string)value);
                }
            }

            throw new Granular.Exception("Can't convert value \"{0}\" from \"{1}\" to \"{2}\"", value, value.GetType().Name, conversionType.Name);
        }
    }
}
