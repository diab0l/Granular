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

        public static string GetEmbeddedResourceString(this Assembly assembly, string resourceName)
        {
            byte[] resourceStream = assembly.GetManifestResourceData(resourceName);

            if (resourceStream == null)
            {
                return String.Empty;
            }

            return String.DecodeUriComponent(String.Escape(new String(Granular.Compatibility.Array.ImplicitCast<char>(resourceStream))));
        }
    }
}
