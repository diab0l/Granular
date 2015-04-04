using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace System
{
    public class AssemblyName
    {
        public string Name { get; private set; }

        public AssemblyName(string name)
        {
            this.Name = name;
        }
    }

    public static class AssemblyExtensions
    {
        public static AssemblyName GetName(this Assembly assembly)
        {
            return new AssemblyName(assembly.FullName);
        }
    }
}
