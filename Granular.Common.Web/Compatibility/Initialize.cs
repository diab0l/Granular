using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Granular.Common.Compatibility
{
    internal class Initialize
    {
        [Bridge.Init(Bridge.InitPosition.Before)]
        static void DefineTypeFullNameCache()
        {
            /*@
                Bridge.Reflection.resolveTypeFullName = Bridge.Reflection.getTypeFullName;

                Bridge.Reflection.getTypeFullName = function (obj) {
                    if (obj.$$fullname === undefined) {
                        obj.$$fullname = Bridge.Reflection.resolveTypeFullName(obj);
                    }

                    return obj.$$fullname;
                };
            */
        }

        [Bridge.Init(Bridge.InitPosition.Before)]
        static void DefineTypeAliasCache()
        {
            /*@
                Bridge.resolveTypeAlias = Bridge.getTypeAlias;

                Bridge.getTypeAlias = function (obj) {
                    if (obj.$$alias === undefined) {
                        obj.$$alias = Bridge.resolveTypeAlias(obj);
                    }

                    return obj.$$alias;
                };
             */
        }
    }
}
