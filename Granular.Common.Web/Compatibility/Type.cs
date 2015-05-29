using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

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

        public static IEnumerable<System.Type> GetTypeInterfaceGenericArguments(System.Type type, System.Type interfaceType)
        {
            System.Type[] arguments = interfaceType.GetGenericArguments();

            if (arguments != null)
            {
                return arguments;
            }

            if (interfaceType == typeof(ICollection<>))
            {
                return type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).
                    Where(methodInfo => methodInfo.Name == "Add" && methodInfo.ParameterTypes.Length == 1).First().ParameterTypes;
            }

            if (interfaceType == typeof(IDictionary<,>))
            {
                return type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).
                    Where(methodInfo => methodInfo.Name == "Add" && methodInfo.ParameterTypes.Length == 2).First().ParameterTypes;
            }

            throw new Granular.Exception("Can't get generic arguments for type \"{0}\" interface \"{1}\"", type.Name, interfaceType.Name);
        }
    }
}
