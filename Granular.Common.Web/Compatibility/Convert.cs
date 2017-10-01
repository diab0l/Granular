using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Compatibility
{
    public static class Convert
    {
        public static object ChangeType(object value, System.Type conversionType)
        {
            if (conversionType == typeof(int))
            {
                return System.Convert.ToInt32(value);
            }

            if (conversionType == typeof(double))
            {
                return System.Convert.ToDouble(value);
            }

            throw new Granular.Exception("Can't convert value \"{0}\" from \"{1}\" to \"{2}\"", value, value.GetType().Name, conversionType.Name);
        }
    }
}
