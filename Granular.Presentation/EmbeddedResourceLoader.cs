using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Granular.Extensions;
using Granular.Collections;
using System.Xaml;
using System.Windows.Markup;

namespace System.Windows
{
    public static class EmbeddedResourceLoader
    {
        private class EmbeddedResourceKey
        {
            public string AssemblyName { get; private set; }
            public string ResourcePath { get; private set; }

            public EmbeddedResourceKey(string assemblyName, string resourcePath)
            {
                this.AssemblyName = assemblyName;
                this.ResourcePath = resourcePath;
            }

            public override bool Equals(object obj)
            {
                EmbeddedResourceKey other = obj as EmbeddedResourceKey;

                return other != null && this.GetType() == other.GetType() &&
                    Object.Equals(this.AssemblyName, other.AssemblyName) &&
                    Object.Equals(this.ResourcePath, other.ResourcePath);
            }

            public override int GetHashCode()
            {
                return AssemblyName.GetHashCode() ^ ResourcePath.GetHashCode();
            }
        }

        private static readonly Regex ResourceUriRegex = new Regex("/([^;]*);component/(.*)");
        private const int ResourceUriAssemblyNameGroupIndex = 1;
        private const int ResourceUriPathGroupIndex = 2;

        private static readonly CacheDictionary<EmbeddedResourceKey, string> resourceStringCache = new CacheDictionary<EmbeddedResourceKey, string>(ResolveResourceString);
        private static readonly CacheDictionary<EmbeddedResourceKey, object> resourceElementCache = new CacheDictionary<EmbeddedResourceKey, object>(ResolveResourceElement);

        public static string LoadResourceString(string resourceUri)
        {
            string assemblyName;
            string resourcePath;
            ParseResourceUri(resourceUri, out assemblyName, out resourcePath);

            return resourceStringCache.GetValue(new EmbeddedResourceKey(assemblyName, resourcePath));
        }

        public static string LoadResourceString(string assemblyName, string resourcePath)
        {
            return resourceStringCache.GetValue(new EmbeddedResourceKey(assemblyName, resourcePath));
        }

        private static string ResolveResourceString(EmbeddedResourceKey key)
        {
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name == key.AssemblyName).FirstOrDefault();

            if (assembly == null)
            {
                assembly = Assembly.Load(key.AssemblyName);
            }

            string resourceName = String.Format("{0}.{1}", key.AssemblyName, key.ResourcePath.TrimStart('/').Replace('/', '.'));

            string resourceString = assembly != null ? assembly.GetEmbeddedResourceString(resourceName) : null;

            if (resourceString.IsNullOrEmpty())
            {
                throw new Granular.Exception("Resource \"{0}\" was not found", resourceName);
            }

            return resourceString;
        }

        public static object LoadResourceElement(string resourceUri)
        {
            string assemblyName;
            string resourcePath;
            ParseResourceUri(resourceUri, out assemblyName, out resourcePath);

            return resourceElementCache.GetValue(new EmbeddedResourceKey(assemblyName, resourcePath));
        }

        public static object LoadResourceElement(string assemblyName, string resourcePath)
        {
            return resourceElementCache.GetValue(new EmbeddedResourceKey(assemblyName, resourcePath));
        }

        private static object ResolveResourceElement(EmbeddedResourceKey key)
        {
            return XamlLoader.Load(XamlParser.Parse(resourceStringCache.GetValue(key)));
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        private static void ParseResourceUri(string resourceUri, out string assemblyName, out string resourcePath)
        {
            Match match = ResourceUriRegex.Match(resourceUri);
            assemblyName = match.Groups[ResourceUriAssemblyNameGroupIndex].Value;
            resourcePath = match.Groups[ResourceUriPathGroupIndex].Value;

            if (!match.Success)
            {
                throw new Granular.Exception("Invalid resource uri \"{0}\"", resourceUri);
            }
        }
    }
}
