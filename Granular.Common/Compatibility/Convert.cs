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
            return System.Convert.ChangeType(value, conversionType);
        }
    }
}
