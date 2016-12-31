using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Compatibility
{
    public class Regex
    {
        private System.Text.RegularExpressions.Regex regex;
        public Regex(string pattern)
        {
            regex = new System.Text.RegularExpressions.Regex(pattern);
        }

        public string[] Match(string input)
        {
            System.Text.RegularExpressions.Match match = regex.Match(input);
            return match.Success ? match.Groups.Cast<System.Text.RegularExpressions.Group>().Select(group => group.Value).ToArray() : null;
        }
    }
}
