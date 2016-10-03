using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Compatibility
{
    public static class Collection
    {
        [Bridge.Template("{collection}.add({item})")]
        public static extern void DynamicAdd(object collection, object item);
    }
}
