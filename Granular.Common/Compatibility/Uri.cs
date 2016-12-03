using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Compatibility
{
    public static class Uri
    {
        public static readonly string PackUriScheme = "pack";

        public static void RegisterPackUriParser()
        {
            if (UriParser.IsKnownScheme(PackUriScheme))
            {
                return;
            }

            UriParser.Register(new GenericUriParser(GenericUriParserOptions.GenericAuthority), PackUriScheme, -1);
        }

        public static System.Uri CreateRelativeOrAbsoluteUri(string relativeUriString)
        {
            return new System.Uri(relativeUriString, UriKind.RelativeOrAbsolute);
        }

        public static System.Uri CreateAbsoluteUri(string absoluteUriString)
        {
            System.Uri uri;

            if (!TryCreateAbsoluteUri(absoluteUriString, out uri))
            {
                throw new Granular.Exception("Can't create CreateAbsoluteUri from {0}", absoluteUriString);
            }

            return uri;
        }

        public static System.Uri CreateAbsoluteUri(System.Uri baseUri, string relativeUriString)
        {
            System.Uri uri;

            if (!TryCreateAbsoluteUri(baseUri, relativeUriString, out uri))
            {
                throw new Granular.Exception("Can't create CreateAbsoluteUri from {0} and {1}", baseUri.GetAbsolutePath(), relativeUriString);
            }

            return uri;
        }

        public static bool TryCreateAbsoluteUri(string absoluteUriString, out System.Uri uri)
        {
            return System.Uri.TryCreate(absoluteUriString, UriKind.Absolute, out uri);
        }

        public static bool TryCreateAbsoluteUri(System.Uri baseUri, string relativeUriString, out System.Uri uri)
        {
            return System.Uri.TryCreate(baseUri, relativeUriString, out uri);
        }
    }
}
