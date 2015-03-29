using System;
using System.Collections.Generic;
using System.Text;

namespace Granular.Compatibility
{
    public static class Type
    {
        public static System.Type GetType(string name)
        {
            if (name == "System.Double")
            {
                return typeof(System.Double);
            }

            if (name == "System.Int32")
            {
                return typeof(System.Int32);
            }

            if (name == "System.String")
            {
                return typeof(System.String);
            }

            return System.Type.GetType(name);
        }
    }
}
