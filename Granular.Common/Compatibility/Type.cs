using System;
using System.Collections.Generic;
using System.Text;

namespace Granular.Compatibility
{
    public static class Type
    {
        public static System.Type GetType(string name)
        {
            return System.Type.GetType(name);
        }

        public static IEnumerable<System.Type> GetTypeInterfaceGenericArguments(System.Type type, System.Type interfaceType)
        {
            return interfaceType.GetGenericArguments();
        }
    }
}
