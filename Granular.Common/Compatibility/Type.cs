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
    }
}
