using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Compatibility
{
    public static class BindingFlags
    {
        public const System.Reflection.BindingFlags InstanceNonPublic =
            System.Reflection.BindingFlags.Instance;

        public const System.Reflection.BindingFlags InstancePublicNonPublicFlattenHierarchy =
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.FlattenHierarchy;
    }
}
