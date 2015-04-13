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
        private sealed class EmbeddedResourceKey
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

                return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
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

        private static readonly CacheDictionary<EmbeddedResourceKey, byte[]> resourceDataCache = new CacheDictionary<EmbeddedResourceKey, byte[]>(ResolveResourceData);
        private static readonly CacheDictionary<EmbeddedResourceKey, object> resourceElementCache = new CacheDictionary<EmbeddedResourceKey, object>(ResolveResourceElement);

        public static byte[] LoadResourceData(string resourceUri)
        {
            string assemblyName;
            string resourcePath;
            ParseResourceUri(resourceUri, out assemblyName, out resourcePath);

            return resourceDataCache.GetValue(new EmbeddedResourceKey(assemblyName, resourcePath));
        }

        public static byte[] LoadResourceData(string assemblyName, string resourcePath)
        {
            return resourceDataCache.GetValue(new EmbeddedResourceKey(assemblyName, resourcePath));
        }

        private static byte[] ResolveResourceData(EmbeddedResourceKey key)
        {
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name == key.AssemblyName).FirstOrDefault();

            if (assembly == null)
            {
                assembly = Assembly.Load(key.AssemblyName);
            }

            string resourceName = String.Format("{0}.{1}", key.AssemblyName, key.ResourcePath.TrimStart('/').Replace('/', '.'));

            byte[] resourceData = assembly != null ? assembly.GetManifestResourceData(resourceName) : null;

            if (resourceData == null)
            {
                throw new Granular.Exception("Resource \"{0}\" was not found", resourceName);
            }

            return resourceData;
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
            string resourceString = Granular.Compatibility.String.FromByteArray(resourceDataCache.GetValue(key));
            return XamlLoader.Load(XamlParser.Parse(resourceString));
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
