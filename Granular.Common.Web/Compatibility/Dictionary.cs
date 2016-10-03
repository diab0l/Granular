using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Compatibility
{
    public static class Dictionary
    {
        [Bridge.Template("{dictionary}.add({key}, {value})")]
        public static extern void DynamicAdd(object dictionary, object key, object value);
    }
}
