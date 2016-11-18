using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Granular.Compatibility
{
    public class AppDomain
    {
        [Bridge.Template("Object.keys(System.Reflection.Assembly.assemblies).map(function(n) { return System.Reflection.Assembly.assemblies[n]; })")]
        public extern static IEnumerable<Assembly> GetAssemblies();
    }
}
