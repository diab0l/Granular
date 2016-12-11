using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static partial class UriExtensions
    {
        public static Uri ResolveAbsoluteUri(this Uri uri, Uri baseUri)
        {
            if (uri.GetIsAbsoluteUri())
            {
                return uri;
            }

            if (baseUri != null && baseUri.GetIsAbsoluteUri())
            {
                return Granular.Compatibility.Uri.CreateAbsoluteUri(baseUri, uri.GetOriginalString());
            }

            if (baseUri == null)
            {
                throw new Granular.Exception("Can't resolve absolute uri for \"{0}\", base uri is null", uri.GetOriginalString());
            }

            throw new Granular.Exception("Can't resolve absolute uri for \"{0}\", base uri \"{1}\" is not an absolute Uri", uri.GetOriginalString(), baseUri.GetOriginalString());
        }
    }
}
