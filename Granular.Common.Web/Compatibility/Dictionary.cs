using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Granular.Compatibility
{
    public static class Dictionary
    {
        [InlineCode("{dictionary}.add({key}, {value})")]
        public static void DynamicAdd(object dictionary, object key, object value)
        {
            //
        }
    }
}
