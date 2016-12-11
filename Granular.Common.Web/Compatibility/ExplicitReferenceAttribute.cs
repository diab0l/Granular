using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Granular.Compatibility
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class AssemblyReferenceAttribute : Attribute
    {
        public AssemblyReferenceAttribute(object reference)
        {
            //
        }
    }
}
