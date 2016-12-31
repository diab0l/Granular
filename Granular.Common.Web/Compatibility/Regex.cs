using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Granular.Compatibility
{
    [Bridge.External]
    [Bridge.Name("RegExp")]
    public class Regex
    {
        public Regex(string pattern)
        {
            //
        }

        [Bridge.Name("exec")]
        public extern string[] Match(string input);
    }
}
