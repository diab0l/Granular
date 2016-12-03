using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Granular.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class UriTest
    {
        [TestMethod]
        public void HttpUriComponentsTest()
        {
            Granular.Compatibility.Uri.RegisterPackUriParser();
            AssertUriComponents(CreateUri("http://host/path"), absoluteUri: "http://host/path", scheme: "http", host: "host", port: 80, isDefaultPort: true, absolutePath: "/path", localPath: "/path", segments: new string[] { "/", "path" }, pathAndQuery: "/path");
            AssertUriComponents(CreateUri("http://user@host/path"), absoluteUri: "http://user@host/path", scheme: "http", userInfo: "user", host: "host", port: 80, isDefaultPort: true, absolutePath: "/path", localPath: "/path", segments: new string[] { "/", "path" }, pathAndQuery: "/path");
            AssertUriComponents(CreateUri("http://user:password@host/path"), absoluteUri: "http://user:password@host/path", scheme: "http", userInfo: "user:password", host: "host", port: 80, isDefaultPort: true, absolutePath: "/path", localPath: "/path", segments: new string[] { "/", "path" }, pathAndQuery: "/path");
            AssertUriComponents(CreateUri("http://user:password@host:80/path"), absoluteUri: "http://user:password@host/path", scheme: "http", userInfo: "user:password", host: "host", port: 80, isDefaultPort: true, absolutePath: "/path", localPath: "/path", segments: new string[] { "/", "path" }, pathAndQuery: "/path");
            AssertUriComponents(CreateUri("http://user:password@host:81/path"), absoluteUri: "http://user:password@host:81/path", scheme: "http", userInfo: "user:password", host: "host", port: 81, absolutePath: "/path", localPath: "/path", segments: new string[] { "/", "path" }, pathAndQuery: "/path");
        }

        [TestMethod]
        public void UncFileUriComponentsTest()
        {
            Granular.Compatibility.Uri.RegisterPackUriParser();
            AssertUriComponents(CreateUri("file://host/path"), absoluteUri: "file://host/path", scheme: "file", host: "host", isDefaultPort: true, absolutePath: "/path", localPath: "\\\\host\\path", segments: new string[] { "/", "path" }, pathAndQuery: "/path", isFile: true, isUnc: true);
        }

        [TestMethod]
        public void FileUriComponentsTest()
        {
            Granular.Compatibility.Uri.RegisterPackUriParser();
            AssertUriComponents(CreateUri("file:///C:/path"), absoluteUri: "file:///C:/path", scheme: "file", isDefaultPort: true, absolutePath: "C:/path", localPath: "C:\\path", segments: new string[] { "/", "C:/", "path" }, pathAndQuery: "C:/path", isFile: true, isLoopback: true);
            AssertUriComponents(CreateUri("C:\\path"), absoluteUri: "file:///C:/path", scheme: "file", isDefaultPort: true, absolutePath: "C:/path", localPath: "C:\\path", segments: new string[] { "/", "C:/", "path" }, pathAndQuery: "C:/path", isFile: true, isLoopback: true);
        }

        [TestMethod]
        public void PackUriComponentsTest()
        {
            Granular.Compatibility.Uri.RegisterPackUriParser();
            AssertUriComponents(CreateUri("pack://application:,,,/assembly;component/path"), absoluteUri: "pack://application:,,,/assembly;component/path", scheme: "pack", host: "application:,,,", isDefaultPort: true, absolutePath: "/assembly;component/path", localPath: "/assembly;component/path", segments: new string[] { "/", "assembly;component/", "path" }, pathAndQuery: "/assembly;component/path");
        }

        [TestMethod]
        public void HttpUriCombineTest()
        {
            Granular.Compatibility.Uri.RegisterPackUriParser();
            AssertUriComponents(CreateUri("http://host/folder/file", "file2"), absoluteUri: "http://host/folder/file2", scheme: "http", host: "host", port: 80, isDefaultPort: true, absolutePath: "/folder/file2", localPath: "/folder/file2", segments: new string[] { "/", "folder/", "file2" }, pathAndQuery: "/folder/file2");
            AssertUriComponents(CreateUri("http://host/folder/file", "/folder2/file2"), absoluteUri: "http://host/folder2/file2", scheme: "http", host: "host", port: 80, isDefaultPort: true, absolutePath: "/folder2/file2", localPath: "/folder2/file2", segments: new string[] { "/", "folder2/", "file2" }, pathAndQuery: "/folder2/file2");
            AssertUriComponents(CreateUri("http://host/path?query#fragment", "/path2?query2#fragment2"), absoluteUri: "http://host/path2?query2#fragment2", scheme: "http", host: "host", port: 80, isDefaultPort: true, absolutePath: "/path2", localPath: "/path2", segments: new string[] { "/", "path2" }, query: "?query2", fragment: "#fragment2", pathAndQuery: "/path2?query2");
        }

        [TestMethod]
        public void UncFileUriCombineTest()
        {
            Granular.Compatibility.Uri.RegisterPackUriParser();
            AssertUriComponents(CreateUri("file://host/folder/file", "file2"), absoluteUri: "file://host/folder/file2", scheme: "file", host: "host", isDefaultPort: true, absolutePath: "/folder/file2", localPath: "\\\\host\\folder\\file2", segments: new string[] { "/", "folder/", "file2" }, pathAndQuery: "/folder/file2", isFile: true, isUnc: true);
            AssertUriComponents(CreateUri("file://host/folder/file", "folder2/file2"), absoluteUri: "file://host/folder/folder2/file2", scheme: "file", host: "host", isDefaultPort: true, absolutePath: "/folder/folder2/file2", localPath: "\\\\host\\folder\\folder2\\file2", segments: new string[] { "/", "folder/", "folder2/", "file2" }, pathAndQuery: "/folder/folder2/file2", isFile: true, isUnc: true);
            AssertUriComponents(CreateUri("file://host/path", "/path2"), absoluteUri: "file://host/path/path2", scheme: "file", host: "host", isDefaultPort: true, absolutePath: "/path/path2", localPath: "\\\\host\\path\\path2", segments: new string[] { "/", "path/", "path2" }, pathAndQuery: "/path/path2", isFile: true, isUnc: true);
        }

        [TestMethod]
        public void FileUriCombineTest()
        {
            Granular.Compatibility.Uri.RegisterPackUriParser();
            AssertUriComponents(CreateUri("file:///C:/folder/file", "file2"), absoluteUri: "file:///C:/folder/file2", scheme: "file", isDefaultPort: true, absolutePath: "C:/folder/file2", localPath: "C:\\folder\\file2", segments: new string[] { "/", "C:/", "folder/", "file2" }, pathAndQuery: "C:/folder/file2", isFile: true, isLoopback: true);
            AssertUriComponents(CreateUri("file:///C:/folder/file", "folder2/file2"), absoluteUri: "file:///C:/folder/folder2/file2", scheme: "file", isDefaultPort: true, absolutePath: "C:/folder/folder2/file2", localPath: "C:\\folder\\folder2\\file2", segments: new string[] { "/", "C:/", "folder/", "folder2/", "file2" }, pathAndQuery: "C:/folder/folder2/file2", isFile: true, isLoopback: true);
            AssertUriComponents(CreateUri("file:///C:/path", "/path2"), absoluteUri: "file:///C:/path2", scheme: "file", isDefaultPort: true, absolutePath: "C:/path2", localPath: "C:\\path2", segments: new string[] { "/", "C:/", "path2" }, pathAndQuery: "C:/path2", isFile: true, isLoopback: true);
            AssertUriComponents(CreateUri("file:///C:/path", "D:/path2"), absoluteUri: "file:///D:/path2", scheme: "file", isDefaultPort: true, absolutePath: "D:/path2", localPath: "D:\\path2", segments: new string[] { "/", "D:/", "path2" }, pathAndQuery: "D:/path2", isFile: true, isLoopback: true);
        }

        [TestMethod]
        public void PackUriCombineTest()
        {
            Granular.Compatibility.Uri.RegisterPackUriParser();
            AssertUriComponents(CreateUri("pack://application:,,,/assembly;component/folder/file", "file2"), absoluteUri: "pack://application:,,,/assembly;component/folder/file2", scheme: "pack", host: "application:,,,", isDefaultPort: true, absolutePath: "/assembly;component/folder/file2", localPath: "/assembly;component/folder/file2", segments: new string[] { "/", "assembly;component/", "folder/", "file2" }, pathAndQuery: "/assembly;component/folder/file2");
            AssertUriComponents(CreateUri("pack://application:,,,/assembly;component/folder/file", "folder2/file2"), absoluteUri: "pack://application:,,,/assembly;component/folder/folder2/file2", scheme: "pack", host: "application:,,,", isDefaultPort: true, absolutePath: "/assembly;component/folder/folder2/file2", localPath: "/assembly;component/folder/folder2/file2", segments: new string[] { "/", "assembly;component/", "folder/", "folder2/", "file2" }, pathAndQuery: "/assembly;component/folder/folder2/file2");
            AssertUriComponents(CreateUri("pack://application:,,,/assembly;component/path", "/path2"), absoluteUri: "pack://application:,,,/path2", scheme: "pack", host: "application:,,,", isDefaultPort: true, absolutePath: "/path2", localPath: "/path2", segments: new string[] { "/", "path2" }, pathAndQuery: "/path2");
            AssertUriComponents(CreateUri("pack://application:,,,/assembly;component/path", "/assembly2;component/path2"), absoluteUri: "pack://application:,,,/assembly2;component/path2", scheme: "pack", host: "application:,,,", isDefaultPort: true, absolutePath: "/assembly2;component/path2", localPath: "/assembly2;component/path2", segments: new string[] { "/", "assembly2;component/", "path2" }, pathAndQuery: "/assembly2;component/path2");
        }

        private static Uri CreateUri(string uriString)
        {
            Uri uri;
            Assert.IsTrue(Granular.Compatibility.Uri.TryCreateAbsoluteUri(uriString, out uri));
            return uri;
        }

        private static Uri CreateUri(string baseUriString, string relativeUriString)
        {
            Uri baseUri, uri;
            Assert.IsTrue(Granular.Compatibility.Uri.TryCreateAbsoluteUri(baseUriString, out baseUri));
            Assert.IsTrue(Granular.Compatibility.Uri.TryCreateAbsoluteUri(baseUri, relativeUriString, out uri));
            return uri;
        }

        private static void AssertUriComponents(Uri uri,
            string absoluteUri = null,
            string scheme = null,
            string userInfo = null,
            string host = null,
            int port = -1,
            bool isDefaultPort = false,
            string absolutePath = null,
            string localPath = null,
            string[] segments = null,
            string query = null,
            string fragment = null,
            string pathAndQuery = null,
            bool isFile = false,
            bool isUnc = false,
            bool isLoopback = false)
        {
            Assert.AreEqual(scheme.DefaultIfNullOrEmpty(), uri.GetScheme());
            Assert.AreEqual(absoluteUri.DefaultIfNullOrEmpty(), uri.GetAbsoluteUri());
            Assert.AreEqual(absolutePath.DefaultIfNullOrEmpty(), uri.GetAbsolutePath());
            Assert.AreEqual(localPath.DefaultIfNullOrEmpty(), uri.GetLocalPath());
            CollectionAssert.AreEqual(segments ?? new string[0], uri.GetSegments());
            Assert.AreEqual(userInfo.DefaultIfNullOrEmpty(), uri.GetUserInfo());
            Assert.AreEqual(host.DefaultIfNullOrEmpty(), uri.GetHost());
            Assert.AreEqual(port, uri.GetPort());
            Assert.AreEqual(query.DefaultIfNullOrEmpty(), uri.GetQuery());
            Assert.AreEqual(fragment.DefaultIfNullOrEmpty(), uri.GetFragment());
            Assert.AreEqual(isDefaultPort, uri.GetIsDefaultPort());
            Assert.AreEqual(isFile, uri.GetIsFile());
            Assert.AreEqual(isUnc, uri.GetIsUnc());
            Assert.AreEqual(isLoopback, uri.GetIsLoopback());
            Assert.AreEqual(pathAndQuery.DefaultIfNullOrEmpty(), uri.GetPathAndQuery());
        }
    }
}
