using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Granular.Extensions;

namespace Granular.Compatibility
{
    public class UriComponents
    {
        // scheme:[//[user:password@]host[:port]][/]path[?query][#fragment]
        private static Regex AbsoluteUriRegex = new Regex(@"^(?<scheme>[^:]*):(//((?<userInfo>[^@]*)@)?(?<host>[^:/]*)(?<port>:[^/]*)?)?(?<path>/?[^\?#]*)(?<query>\?[^#]*)?((?<fragment>#.*))?$");

        // [/]path[?query][#fragment]
        private static Regex RelativeUriRegex = new Regex(@"^(?<path>/?[^\?#]*)(?<query>\?[^#]*)?((?<fragment>#.*))?$");

        // [drive]:/
        private static Regex RootedPathRegex = new Regex(@"^(?<root>[a-zA-Z]:/)[^/]");

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
            bool isFile = System.String.Equals(scheme, "file", StringComparison.InvariantCultureIgnoreCase);
            bool isUnc = isFile && !host.IsNullOrEmpty();
            bool isLocalhost = System.String.Equals(host, "localhost", StringComparison.InvariantCultureIgnoreCase);
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

            Match relativeUriMatch = RelativeUriRegex.Match(relativeUriString);

            string path = relativeUriMatch.Groups["path"].Value;
            string query = relativeUriMatch.Groups["query"].Value;
            string fragment = relativeUriMatch.Groups["fragment"].Value;

            string combinedPath = path.StartsWith("/") ? path : (AbsolutePath.Substring(0, AbsolutePath.LastIndexOf('/') + 1) + path);

            return new UriComponents(Scheme, UserInfo, Host, Port, combinedPath, query, fragment);
        }

        private static string CombineFilePath(string path, string relativePath)
        {
            if (RootedPathRegex.IsMatch(relativePath))
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

            if (RootedPathRegex.IsMatch(uriString))
            {
                uriString = $"file:///{uriString}";
            }

            Match absoluteUriMatch = AbsoluteUriRegex.Match(uriString);
            if (!absoluteUriMatch.Success)
            {
                uriComponents = null;
                return false;
            }

            string scheme = absoluteUriMatch.Groups["scheme"].Value;
            string userInfo = absoluteUriMatch.Groups["userInfo"].Value;
            string path = absoluteUriMatch.Groups["path"].Value;
            string query = absoluteUriMatch.Groups["query"].Value;
            string fragment = absoluteUriMatch.Groups["fragment"].Value;

            string host;
            int port;

            string hostGroupValue = absoluteUriMatch.Groups["host"].Value;
            string portGroupValue = absoluteUriMatch.Groups["port"].Value;

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
            if (System.String.Equals(scheme, "http", StringComparison.InvariantCultureIgnoreCase))
            {
                return HttpDefaultPort;
            }

            if (System.String.Equals(scheme, "https", StringComparison.InvariantCultureIgnoreCase))
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
