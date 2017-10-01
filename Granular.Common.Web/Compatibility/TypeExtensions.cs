using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    public static class TypeExtensions
    {
        public static bool GetIsValueType(this Type type)
        {
            return !type.IsClass && !type.IsInterface;
        }

        public static Type GetTypeHandle(this Type type)
        {
            return type;
        }
    }
}
