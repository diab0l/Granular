using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Granular.Extensions;

namespace Granular.Compatibility
{
    public static class Dictionary
    {
        public static void DynamicAdd(object dictionary, object key, object value)
        {
            MethodInfo addMethod = dictionary.GetType().GetInterfaceType(typeof(IDictionary<,>)).GetMethod("Add");

            addMethod.Invoke(dictionary, new [] { key, value });
        }
    }
}
