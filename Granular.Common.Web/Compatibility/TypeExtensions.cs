using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    public static class TypeExtensions
    {
        public static bool GetIsAbstract(this Type type)
        {
            return false;
        }

        public static bool GetIsGenericType(this Type type)
        {
            return type.GetGenericTypeDefinition() != null;
        }

        public static bool GetIsValueType(this Type type)
        {
            return !type.IsClass && !type.IsInterface;
        }

        public static ConstructorInfo GetDefaultConstructor(this Type type)
        {
            return type.GetConstructors().FirstOrDefault(constructorInfo => constructorInfo.Name == ".ctor" && constructorInfo.ParameterTypes.Length == 0);
        }

        public static Type GetTypeHandle(this Type type)
        {
            return type;
        }
    }
}
