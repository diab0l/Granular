extern alias wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Granular.Extensions;
using System.IO;
using System.Windows.Media;

namespace Granular.Host.Wpf.Render
{
    internal class WpfImageSourceRenderResource : IImageSourceRenderResource
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

        public wpf::System.Windows.Media.ImageSource ImageSource { get; private set; }
        public wpf::System.Windows.Media.Imaging.BitmapImage BitmapImage { get; private set; }
        private bool initialized;

        public void Initialize(Uri uri, Rect sourceRect)
        {
            if (initialized)
            {
                throw new Granular.Exception("ImageSourceRenderResource is already initialized");
            }

            wpf::System.Windows.Media.Imaging.BitmapImage bitmapImage = new wpf::System.Windows.Media.Imaging.BitmapImage();
            bitmapImage.DownloadProgress += (sender, e) => State = RenderImageState.DownloadProgress;
            bitmapImage.DownloadFailed += (sender, e) => State = RenderImageState.DownloadFailed;
            bitmapImage.DownloadCompleted += (sender, e) =>
            {
                Size = new Size(bitmapImage.PixelWidth, bitmapImage.PixelHeight);
                State = RenderImageState.DownloadCompleted;
            };

            bitmapImage.BeginInit();

            if (uri.Scheme == "pack")
            {
                bitmapImage.StreamSource = new MemoryStream(EmbeddedResourceLoader.LoadResourceData(uri));
            }
            else
            {
                bitmapImage.UriSource = uri;
            }

            if (!sourceRect.IsNullOrEmpty())
            {
                bitmapImage.SourceRect = new wpf::System.Windows.Int32Rect((int)sourceRect.Left, (int)sourceRect.Top, (int)sourceRect.Width, (int)sourceRect.Height);
            }

            bitmapImage.EndInit();

            this.BitmapImage = bitmapImage;
            this.Size = new Size(BitmapImage.PixelWidth, BitmapImage.PixelHeight);

            initialized = true;
        }
    }
}
