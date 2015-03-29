using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    public static class AssemblyExtensions
    {
        public static string GetEmbeddedResourceString(this Assembly assembly, string resourceName)
        {
            Stream resourceStream = assembly.GetManifestResourceStream(resourceName);

            if (resourceStream == null)
            {
                return System.String.Empty;
            }

            StreamReader streamReader = new StreamReader(resourceStream);
            return streamReader.ReadToEnd();
        }
    }
}
