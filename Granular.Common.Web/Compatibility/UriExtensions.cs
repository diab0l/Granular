using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static partial class UriExtensions
    {
        public static string GetOriginalString(this System.Uri uri)
        {
            return uri.AbsoluteUri;
        }

        public static bool GetIsAbsoluteUri(this System.Uri uri)
        {
            Granular.Compatibility.UriComponents uriComponents;
            return Granular.Compatibility.Uri.TryGetUriComponents(uri, out uriComponents);
        }

        public static string GetScheme(this System.Uri uri)
        {
            return Granular.Compatibility.Uri.GetUriComponents(uri).Scheme;
        }

        public static string GetAbsoluteUri(this System.Uri uri)
        {
            return Granular.Compatibility.Uri.GetUriComponents(uri).AbsoluteUri;
        }

        public static string GetAbsolutePath(this System.Uri uri)
        {
            return Granular.Compatibility.Uri.GetUriComponents(uri).AbsolutePath;
        }

        public static string GetLocalPath(this System.Uri uri)
        {
            return Granular.Compatibility.Uri.GetUriComponents(uri).LocalPath;
        }

        public static string[] GetSegments(this System.Uri uri)
        {
            return Granular.Compatibility.Uri.GetUriComponents(uri).Segments;
        }

        public static string GetUserInfo(this System.Uri uri)
        {
            return Granular.Compatibility.Uri.GetUriComponents(uri).UserInfo;
        }

        public static string GetHost(this System.Uri uri)
        {
            return Granular.Compatibility.Uri.GetUriComponents(uri).Host;
        }

        public static int GetPort(this System.Uri uri)
        {
            return Granular.Compatibility.Uri.GetUriComponents(uri).Port;
        }

        public static string GetQuery(this System.Uri uri)
        {
            return Granular.Compatibility.Uri.GetUriComponents(uri).Query;
        }

        public static string GetFragment(this System.Uri uri)
        {
            return Granular.Compatibility.Uri.GetUriComponents(uri).Fragment;
        }

        public static bool GetIsDefaultPort(this System.Uri uri)
        {
            return Granular.Compatibility.Uri.GetUriComponents(uri).IsDefaultPort;
        }

        public static bool GetIsFile(this System.Uri uri)
        {
            return Granular.Compatibility.Uri.GetUriComponents(uri).IsFile;
        }

        public static bool GetIsUnc(this System.Uri uri)
        {
            return Granular.Compatibility.Uri.GetUriComponents(uri).IsUnc;
        }

        public static bool GetIsLoopback(this System.Uri uri)
        {
            return Granular.Compatibility.Uri.GetUriComponents(uri).IsLoopback;
        }

        public static string GetPathAndQuery(this System.Uri uri)
        {
            return Granular.Compatibility.Uri.GetUriComponents(uri).PathAndQuery;
        }
    }
}