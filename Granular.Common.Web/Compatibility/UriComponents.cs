using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Granular.Extensions;

namespace Granular.Compatibility
{
    public class UriComponents
    {
        // scheme:[//[user:password@]host[:port]][/]path[?query][#fragment]
        private static Granular.Compatibility.Regex AbsoluteUriRegex = new Granular.Compatibility.Regex(@"^([^:]*):(\/\/(([^@]*)@)?([^:\/]*)(:[^\/]*)?)?(\/?[^\?#]*)(\?[^#]*)?(#.*)?$");

        // [/]path[?query][#fragment]
        private static Granular.Compatibility.Regex RelativeUriRegex = new Granular.Compatibility.Regex(@"^(\/?[^\?#]*)(\?[^#]*)?(#.*)?$");

        // [drive]:/
        private static Granular.Compatibility.Regex RootedPathRegex = new Granular.Compatibility.Regex(@"^([a-zA-Z]:\/)[^\/]");

        private const int HttpDefaultPort = 80;
        private const int HttpsDefaultPort = 443;

        public string Scheme { get; }

        public string AbsoluteUri { get; }
        public string AbsolutePath { get; }
        public string LocalPath { get; }

        public string[] Segments { get; }

        public string UserInfo { get; }
        public string Host { get; }
        public int Port { get; }
        public string Query { get; }
        public string Fragment { get; }

        public bool IsDefaultPort { get; }
        public bool IsFile { get; }
        public bool IsUnc { get; }
        public bool IsLoopback { get; }
        public string PathAndQuery { get; }

        private UriComponents(string scheme, string userInfo, string host, int port, string path, string query, string fragment)
        {
            bool isFile = scheme.ToLower() == "file";
            bool isUnc = isFile && !host.IsNullOrEmpty();
            bool isLocalhost = host.ToLower() == "localhost";
            string absolutePath = isFile && !isUnc ? path.TrimStart('/') : path;

            this.Scheme = scheme;
            this.AbsolutePath = absolutePath;
            this.UserInfo = userInfo;
            this.Host = host;
            this.Port = port;
            this.Query = query;
            this.Fragment = fragment;

            this.IsFile = isFile;
            this.IsUnc = isUnc;
            this.IsLoopback = isFile && !isUnc || isLocalhost;
            this.LocalPath = isUnc ? System.String.Format("\\\\{0}{1}", host, absolutePath.Replace('/', '\\')) : isFile ? absolutePath.Replace('/', '\\') : absolutePath;
            this.IsDefaultPort = Port == GetDefaultPort(scheme);
            this.PathAndQuery = $"{absolutePath}{query}";
            this.AbsoluteUri = GetAbsoluteUri(scheme, userInfo, host, port, path, query, fragment);
            this.Segments = GetPathSegments(path);
        }

        public UriComponents Combine(string relativeUriString)
        {
            relativeUriString = relativeUriString.Replace('\\', '/');

            if (IsUnc)
            {
                return new UriComponents(Scheme, UserInfo, Host, Port, CombineUncPath(AbsolutePath, relativeUriString), Query, Fragment);
            }

            if (IsFile)
            {
                return new UriComponents(Scheme, UserInfo, Host, Port, "/" + CombineFilePath(AbsolutePath, relativeUriString), Query, Fragment);
            }

            string[] relativeUriMatch = RelativeUriRegex.Match(relativeUriString);

            string path = relativeUriMatch[1].DefaultIfNullOrEmpty();
            string query = relativeUriMatch[2].DefaultIfNullOrEmpty();
            string fragment = relativeUriMatch[3].DefaultIfNullOrEmpty();

            string combinedPath = path.StartsWith("/") ? path : (AbsolutePath.Substring(0, AbsolutePath.LastIndexOf('/') + 1) + path);

            return new UriComponents(Scheme, UserInfo, Host, Port, combinedPath, query, fragment);
        }

        private static string CombineFilePath(string path, string relativePath)
        {
            if (RootedPathRegex.Match(relativePath) != null)
            {
                return relativePath;
            }

            if (relativePath.StartsWith("/"))
            {
                return path.Substring(0, path.IndexOf(':') + 1) + relativePath;
            }

            return path.Substring(0, path.LastIndexOf('/') + 1) + relativePath;
        }

        private static string CombineUncPath(string absolutePath, string relativePath)
        {
            return relativePath.StartsWith("/") ? (absolutePath + relativePath) : (absolutePath.Substring(0, absolutePath.LastIndexOf('/') + 1) + relativePath);
        }

        public static bool TryParse(string uriString, out UriComponents uriComponents)
        {
            uriString = uriString.Replace('\\', '/');

            if (RootedPathRegex.Match(uriString) != null)
            {
                uriString = $"file:///{uriString}";
            }

            string[] absoluteUriMatch = AbsoluteUriRegex.Match(uriString);
            if (absoluteUriMatch == null)
            {
                uriComponents = null;
                return false;
            }

            string scheme = absoluteUriMatch[1].DefaultIfNullOrEmpty();
            string userInfo = absoluteUriMatch[4].DefaultIfNullOrEmpty();
            string path = absoluteUriMatch[7].DefaultIfNullOrEmpty();
            string query = absoluteUriMatch[8].DefaultIfNullOrEmpty();
            string fragment = absoluteUriMatch[9].DefaultIfNullOrEmpty();

            string host;
            int port;

            string hostGroupValue = absoluteUriMatch[5].DefaultIfNullOrEmpty();
            string portGroupValue = absoluteUriMatch[6].DefaultIfNullOrEmpty();

            if (portGroupValue.IsNullOrEmpty())
            {
                host = hostGroupValue;
                port = GetDefaultPort(scheme);
            }
            else if (!Int32.TryParse(portGroupValue.Substring(1), out port))
            {
                host = hostGroupValue + portGroupValue;
                port = -1;
            }
            else
            {
                host = hostGroupValue;
            }

            uriComponents = new UriComponents(scheme, userInfo, host, port, path, query, fragment);
            return true;
        }

        private static int GetDefaultPort(string scheme)
        {
            if (scheme.ToLower() == "http")
            {
                return HttpDefaultPort;
            }

            if (scheme.ToLower() == "https")
            {
                return HttpsDefaultPort;
            }

            return -1;
        }

        private static string[] GetPathSegments(string path)
        {
            string[] segments = path.Split('/');
            return segments.Take(segments.Length - 1).Select(segment => $"{segment}/").Concat(new[] { segments.Last() }).ToArray();
        }

        private static string GetAbsoluteUri(string scheme, string userInfo, string host, int port, string path, string query, string fragment)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(scheme);
            stringBuilder.Append("://");

            if (!userInfo.IsNullOrEmpty())
            {
                stringBuilder.Append(userInfo);
                stringBuilder.Append("@");
            }

            stringBuilder.Append(host);

            if (port != GetDefaultPort(scheme))
            {
                stringBuilder.Append(":");
                stringBuilder.Append(port);
            }

            stringBuilder.Append(path);
            stringBuilder.Append(query);
            stringBuilder.Append(fragment);

            return stringBuilder.ToString();
        }
    }
}
