using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Granular.Extensions;

namespace Granular.Host.Render
{
    public class HtmlImageSourceRenderResource : IImageSourceRenderResource
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

        private EmbeddedResourceObjectFactory objectFactory;
        private ImageElementContainer container;

        private Bridge.Html5.HTMLElement image;

        public HtmlImageSourceRenderResource(EmbeddedResourceObjectFactory objectFactory, ImageElementContainer container)
        {
            this.objectFactory = objectFactory;
            this.container = container;

            SourceRect = Rect.Empty;
            ImageSize = Size.Empty;
            Size = Size.Empty;
        }

        public void Initialize(Uri uri, Rect sourceRect)
        {
            SourceRect = sourceRect;

            if (uri.GetScheme() == "pack")
            {
                Url = objectFactory.GetObjectUrl(uri);
            }
            else
            {
                Url = uri.GetAbsoluteUri();
                State = RenderImageState.DownloadProgress;
            }

            image = Bridge.Html5.Document.CreateElement("img");
            image.AddEventListener("load", OnImageLoad);
            image.AddEventListener("error", OnImageError);
            image.AddEventListener("abort", OnImageAbort);

            container.Add(image);

            image.SetAttribute("src", Url);
        }

        private void OnImageLoad()
        {
            ImageSize = new Size(image.ClientWidth, image.ClientHeight);
            Size = !SourceRect.IsNullOrEmpty() ? SourceRect.Size : ImageSize;
            State = RenderImageState.DownloadCompleted;
            container.Remove(image);
        }

        private void OnImageError()
        {
            State = RenderImageState.DownloadFailed;
            container.Remove(image);
        }

        private void OnImageAbort()
        {
            State = RenderImageState.DownloadFailed;
            container.Remove(image);
        }
    }
}
