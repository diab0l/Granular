using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Granular.Compatibility
{
    public class AppDomain
    {
        public static IEnumerable<Assembly> GetAssemblies()
        {
            return System.AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}
