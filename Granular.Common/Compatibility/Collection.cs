using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Granular.Extensions;

namespace Granular.Compatibility
{
    public static class Collection
    {
        public static void DynamicAdd(object collection, object item)
        {
            MethodInfo addMethod = collection.GetType().GetInterfaceType(typeof(ICollection<>)).GetMethod("Add");

            addMethod.Invoke(collection, new[] { item });
        }
    }
}
