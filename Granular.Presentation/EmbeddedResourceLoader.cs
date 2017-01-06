using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Granular.Extensions;
using Granular.Collections;
using System.Windows.Markup;

namespace System.Windows
{
    public static class EmbeddedResourceLoader
    {
        private class UriEqualityComparer : IEqualityComparer<Uri>
        {
            public static readonly UriEqualityComparer Default = new UriEqualityComparer();

            private UriEqualityComparer()
            {
                //
            }

            public bool Equals(Uri x, Uri y)
            {
                return Object.Equals(x.GetAbsoluteUri(), y.GetAbsoluteUri());
            }

            public int GetHashCode(Uri obj)
            {
                return obj.GetAbsoluteUri().GetHashCode();
            }
        }

        private static readonly Granular.Compatibility.Regex ResourceUriRegex = new Granular.Compatibility.Regex("/([^;]*);component/(.*)");
        private const int ResourceUriAssemblyNameGroupIndex = 1;
        private const int ResourceUriPathGroupIndex = 2;

        private static readonly CacheDictionary<Uri, byte[]> resourceDataCache = CacheDictionary<Uri, byte[]>.CreateUsingStringKeys(ResolveResourceData, uri => uri.GetAbsoluteUri());
        private static readonly CacheDictionary<Uri, object> resourceElementCache = CacheDictionary<Uri, object>.CreateUsingStringKeys(ResolveResourceElement, uri => uri.GetAbsoluteUri());

        public static byte[] LoadResourceData(Uri resourceUri)
        {
            VerifyResourceUri(resourceUri);

            return resourceDataCache.GetValue(resourceUri);
        }

        private static byte[] ResolveResourceData(Uri resourceUri)
        {
            string assemblyName;
            string resourcePath;
            if (!TryParseAbsolutePath(resourceUri.GetAbsolutePath(), out assemblyName, out resourcePath))
            {
                throw new Granular.Exception("Resource \"{0}\" absolute path is invalid", resourceUri.GetAbsoluteUri());
            }

            Assembly assembly = Granular.Compatibility.AppDomain.GetAssemblies().Where(a => a.GetName().Name == assemblyName).FirstOrDefault();

            if (assembly == null)
            {
                assembly = Assembly.Load(assemblyName);
            }

            string resourceName = String.Format("{0}.{1}", assemblyName, resourcePath.TrimStart('/').Replace('/', '.'));

            byte[] resourceData = assembly != null ? assembly.GetManifestResourceData(resourceName) : null;

            if (resourceData == null)
            {
                throw new Granular.Exception("Resource \"{0}\" was not found", resourceUri.GetAbsoluteUri());
            }

            return resourceData;
        }

        public static object LoadResourceElement(Uri resourceUri)
        {
            VerifyResourceUri(resourceUri);

            return resourceElementCache.GetValue(resourceUri);
        }

        private static object ResolveResourceElement(Uri resourceUri)
        {
            string resourceString = Granular.Compatibility.String.FromByteArray(resourceDataCache.GetValue(resourceUri));
            return XamlLoader.Load(XamlParser.Parse(resourceString, resourceUri));
        }

        private static bool TryParseAbsolutePath(string absolutePath, out string assemblyName, out string resourcePath)
        {
            string[] matches = ResourceUriRegex.Match(absolutePath);

            if (matches == null)
            {
                assemblyName = null;
                resourcePath = null;
                return false;
            }

            assemblyName = matches[ResourceUriAssemblyNameGroupIndex];
            resourcePath = matches[ResourceUriPathGroupIndex];
            return true;
        }

        private static void VerifyResourceUri(Uri resourceUri)
        {
            if (!resourceUri.GetIsAbsoluteUri())
            {
                throw new Granular.Exception("Resource uri \"{0}\" must be an absolute uri", resourceUri.GetOriginalString());
            }

            if (resourceUri.GetScheme() != "pack")
            {
                throw new Granular.Exception("Resource uri \"{0}\" must be a pack uri", resourceUri.GetAbsoluteUri());
            }
        }
    }
}
