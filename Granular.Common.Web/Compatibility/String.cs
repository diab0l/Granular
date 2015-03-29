using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Granular.Compatibility
{
    public static class String
    {
        private static Regex StringWhitespaceFormat = new Regex("^[ \t\r\n]*$");

        public static bool IsNullOrWhitespace(string value)
        {
            return StringWhitespaceFormat.Exec(value) != null;
        }
    }
}
