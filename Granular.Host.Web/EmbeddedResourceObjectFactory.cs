using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows;
using Granular.Compatibility.Linq;
using Granular.Collections;
using Granular.Extensions;

namespace Granular.Host
{
    public class EmbeddedResourceObjectFactory
    {
        private HtmlValueConverter converter;
        private CacheDictionary<Uri, string> objectUrlCache;

        public EmbeddedResourceObjectFactory(HtmlValueConverter converter)
        {
            this.converter = converter;
            objectUrlCache = CacheDictionary<Uri, string>.CreateUsingStringKeys(ResolveObjectUrl, uri => uri.GetAbsoluteUri());
        }

        public string GetObjectUrl(Uri uri)
        {
            return objectUrlCache.GetValue(uri);
        }

        private string ResolveObjectUrl(Uri uri)
        {
            byte[] imageData = EmbeddedResourceLoader.LoadResourceData(uri);
            string mimeType = converter.ToMimeTypeString(GetExtension(uri.GetLocalPath()));

            return CreateObjectUrl(CreateBlob(imageData, mimeType));
        }

        [Bridge.Template("new Blob([new Uint8Array({data})], {type: {mimeType}})")]
        private static extern object CreateBlob(byte[] data, string mimeType);

        [Bridge.Template("URL.createObjectURL({blob})")]
        public static extern string CreateObjectUrl(object blob);

        private static string GetExtension(string path)
        {
            return path.Substring(path.LastIndexOf('.') + 1).ToLower();
        }
    }
}
