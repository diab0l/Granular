using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static partial class UriExtensions
    {
        public static string GetOriginalString(this Uri uri)
        {
            return uri.OriginalString;
        }

        public static bool GetIsAbsoluteUri(this Uri uri)
        {
            return uri.IsAbsoluteUri;
        }

        public static string GetScheme(this Uri uri)
        {
            return uri.Scheme;
        }

        public static string GetAbsoluteUri(this Uri uri)
        {
            return uri.AbsoluteUri;
        }

        public static string GetAbsolutePath(this Uri uri)
        {
            return uri.AbsolutePath;
        }

        public static string GetLocalPath(this Uri uri)
        {
            return uri.LocalPath;
        }

        public static string[] GetSegments(this Uri uri)
        {
            return uri.Segments;
        }

        public static string GetUserInfo(this Uri uri)
        {
            return uri.UserInfo;
        }

        public static string GetHost(this Uri uri)
        {
            return uri.Host;
        }

        public static int GetPort(this Uri uri)
        {
            return uri.Port;
        }

        public static string GetQuery(this Uri uri)
        {
            return uri.Query;
        }

        public static string GetFragment(this Uri uri)
        {
            return uri.Fragment;
        }

        public static bool GetIsDefaultPort(this Uri uri)
        {
            return uri.IsDefaultPort;
        }

        public static bool GetIsFile(this Uri uri)
        {
            return uri.IsFile;
        }

        public static bool GetIsUnc(this Uri uri)
        {
            return uri.IsUnc;
        }

        public static bool GetIsLoopback(this Uri uri)
        {
            return uri.IsLoopback;
        }

        public static string GetPathAndQuery(this Uri uri)
        {
            return uri.PathAndQuery;
        }
    }
}
