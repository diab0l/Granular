using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Granular.Compatibility
{
    public static class Collection
    {
        [InlineCode("{$System.Script}.add({collection}, {item})")]
        public static void DynamicAdd(object collection, object item)
        {
            //
        }
    }
}
