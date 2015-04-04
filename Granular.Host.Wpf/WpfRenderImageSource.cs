extern alias wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Granular.Extensions;
using System.IO;
using System.Windows.Media;

namespace Granular.Host.Wpf
{
    internal class WpfRenderImageSourceFactory : IRenderImageSourceFactory
    {
        public static readonly IRenderImageSourceFactory Default = new WpfRenderImageSourceFactory();

        private WpfRenderImageSourceFactory()
        {
            //
        }

        public IRenderImageSource CreateRenderImageSource(RenderImageType imageType, byte[] data, Rect sourceRect)
        {
            wpf::System.Windows.Media.Imaging.BitmapImage bitmapImage = new wpf::System.Windows.Media.Imaging.BitmapImage();

            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(data);

            if (!sourceRect.IsNullOrEmpty())
            {
                bitmapImage.SourceRect = new wpf::System.Windows.Int32Rect((int)sourceRect.Left, (int)sourceRect.Top, (int)sourceRect.Width, (int)sourceRect.Height);
            }

            bitmapImage.EndInit();

            return new WpfRenderImageSource(bitmapImage);
        }

        public IRenderImageSource CreateRenderImageSource(string uri, Rect sourceRect)
        {
            if (!IsUrl(uri))
            {
                return CreateRenderImageSource(RenderImageType.Unknown, EmbeddedResourceLoader.LoadResourceData(uri), sourceRect);
            }

            wpf::System.Windows.Media.Imaging.BitmapImage bitmapImage = new wpf::System.Windows.Media.Imaging.BitmapImage();

            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(uri);

            if (!sourceRect.IsNullOrEmpty())
            {
                bitmapImage.SourceRect = new wpf::System.Windows.Int32Rect((int)sourceRect.Left, (int)sourceRect.Top, (int)sourceRect.Width, (int)sourceRect.Height);
            }

            bitmapImage.EndInit();

            return new WpfRenderImageSource(bitmapImage);
        }

        private static bool IsUrl(string uri)
        {
            return uri.Contains("://");
        }
    }

    internal class WpfRenderImageSource : IRenderImageSource
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

        public WpfRenderImageSource(wpf::System.Windows.Media.Imaging.BitmapImage bitmapImage)
        {
            this.BitmapImage = bitmapImage;

            BitmapImage.DownloadProgress += (sender, e) => State = RenderImageState.DownloadProgress;
            BitmapImage.DownloadFailed += (sender, e) => State = RenderImageState.DownloadFailed;
            BitmapImage.DownloadCompleted += (sender, e) =>
            {
                Size = new Size(BitmapImage.PixelWidth, BitmapImage.PixelHeight);
                State = RenderImageState.DownloadCompleted;
            };

            Size = new Size(BitmapImage.PixelWidth, BitmapImage.PixelHeight);
        }
    }
}
