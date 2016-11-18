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
            return type.IsAbstract;
        }

        public static bool GetIsGenericType(this Type type)
        {
            return type.IsGenericType;
        }

        public static bool GetIsValueType(this Type type)
        {
            return type.IsValueType;
        }

        public static ConstructorInfo GetDefaultConstructor(this Type type)
        {
            return type.GetConstructor(new Type[0]);
        }

        public static RuntimeTypeHandle GetTypeHandle(this Type type)
        {
            return type.TypeHandle;
        }
    }
}
