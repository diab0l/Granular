using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows;
using Granular.Extensions;
using Granular.Collections;

namespace Granular.Host
{
    public class RenderImageSourceFactory : IRenderImageSourceFactory
    {
        public static readonly IRenderImageSourceFactory Default = new RenderImageSourceFactory(HtmlValueConverter.Default);

        private System.Html.Element container;
        private System.Html.Element Container
        {
            get
            {
                if (container == null)
                {
                    container = System.Html.Document.CreateElement("div");
                    container.Style.Visibility = "hidden";
                    container.Style.Overflow = "hidden";
                    container.Style.Width = "0px";
                    container.Style.Height = "0px";

                    System.Html.Document.Body.AppendChild(container);
                }

                return container;
            }
        }

        private IHtmlValueConverter converter;
        private CacheDictionary<string, string> objectUrlCache;

        private RenderImageSourceFactory(IHtmlValueConverter converter)
        {
            this.converter = converter;
            objectUrlCache = new CacheDictionary<string, string>(ResolveObjectUrl);
        }

        public IRenderImageSource CreateRenderImageSource(string uri, Rect sourceRect)
        {
            if (IsUrl(uri))
            {
                return new RenderImageSource(Container, uri, false, sourceRect);
            }

            return new RenderImageSource(Container, objectUrlCache.GetValue(uri), true, sourceRect);
        }

        public IRenderImageSource CreateRenderImageSource(RenderImageType imageType, byte[] imageData, Rect sourceRect)
        {
            string mimeType = converter.ToMimeTypeString(imageType);
            string url = CreateObjectUrl(CreateBlob(imageData, mimeType));

            return new RenderImageSource(Container, url, true, sourceRect);
        }

        private string ResolveObjectUrl(string uri)
        {
            byte[] imageData = EmbeddedResourceLoader.LoadResourceData(uri);
            string mimeType = converter.ToMimeTypeString(GetRenderImageType(uri));

            return CreateObjectUrl(CreateBlob(imageData, mimeType));
        }

        [System.Runtime.CompilerServices.InlineCode("new Blob([new Uint8Array({data})], {{type: {mimeType}}})")]
        private static object CreateBlob(byte[] data, string mimeType)
        {
            return null;
        }

        [System.Runtime.CompilerServices.InlineCode("URL.createObjectURL({blob})")]
        public static string CreateObjectUrl(object blob)
        {
            return null;
        }

        private static bool IsUrl(string uri)
        {
            return uri.Contains("://");
        }

        private static RenderImageType GetRenderImageType(string uri)
        {
            string extension = uri.Substring(uri.LastIndexOf('.') + 1).ToLower();

            switch (extension)
            {
                case "gif": return RenderImageType.Gif;
                case "jpg": return RenderImageType.Jpeg;
                case "png": return RenderImageType.Png;
                case "svg": return RenderImageType.Svg;
            }

            return RenderImageType.Unknown;
        }
    }

    public class RenderImageSource : IRenderImageSource
    {
        public event EventHandler StateChanged;

        private RenderImageState state;
        public RenderImageState State
        {
            get { return state; }
            set
            {
                if (state == value)
                {
                    return;
                }

                state = value;
                StateChanged.Raise(this);
            }
        }

        public Size Size { get; private set; }

        public string Url { get; private set; }

        public Rect SourceRect { get; private set; }
        public Size ImageSize { get; private set; }

        private System.Html.Element container;
        private System.Html.Element image;

        public RenderImageSource(System.Html.Element container, string url, bool isLocalUrl, Rect sourceRect)
        {
            this.container = container;

            this.Url = url;
            this.SourceRect = sourceRect;
            this.State = isLocalUrl ? RenderImageState.Idle : RenderImageState.DownloadProgress;

            image = System.Html.Document.CreateElement("img");
            container.AppendChild(image);

            image.AddEventListener("load", OnImageLoad);
            image.AddEventListener("error", OnImageError);
            image.AddEventListener("abort", OnImageAbort);

            image.SetAttribute("src", url);

            ImageSize = Size.Empty;
            Size = Size.Empty;
        }

        private void OnImageLoad()
        {
            ImageSize = new Size(image.ClientWidth, image.ClientHeight);
            Size = !SourceRect.IsNullOrEmpty() ? SourceRect.Size : ImageSize;
            State = RenderImageState.DownloadCompleted;
            container.RemoveChild(image);
        }

        private void OnImageError()
        {
            State = RenderImageState.DownloadFailed;
            container.RemoveChild(image);
        }

        private void OnImageAbort()
        {
            State = RenderImageState.DownloadFailed;
            container.RemoveChild(image);
        }
    }
}
