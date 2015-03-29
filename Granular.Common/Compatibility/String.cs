using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Compatibility
{
    public static class String
    {
        public static bool IsNullOrWhitespace(string value)
        {
            return System.String.IsNullOrWhiteSpace(value);
        }
    }
}
