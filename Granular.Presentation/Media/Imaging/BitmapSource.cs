using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace System.Windows.Media.Imaging
{
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class BitmapSource : ImageSource
    {
        public event EventHandler DownloadProgress;
        public event EventHandler DownloadCompleted;
        public event EventHandler DownloadFailed;

        public bool IsDownloading { get; private set; }

        private IRenderImageSource renderImageSource;
        public override IRenderImageSource RenderImageSource { get { return renderImageSource; } }

        private RenderImageState renderImageState;

        protected BitmapSource()
        {
            //
        }

        private BitmapSource(IRenderImageSource renderImageSource)
        {
            this.renderImageSource = renderImageSource;
        }

        protected void SetRenderImageState(RenderImageState renderImageState)
        {
            if (this.renderImageState == renderImageState)
            {
                return;
            }

            RenderImageState oldRenderImageState = this.renderImageState;
            this.renderImageState = renderImageState;

            if (oldRenderImageState != RenderImageState.Idle && oldRenderImageState != RenderImageState.DownloadProgress || renderImageState == RenderImageState.Idle)
            {
                throw new Granular.Exception("Can't change BitmapSource.RenderImageState from \"{0}\" to \"{1}\"", oldRenderImageState, renderImageState);
            }

            switch (renderImageState)
            {
                case RenderImageState.DownloadProgress:
                    IsDownloading = true;
                    DownloadProgress.Raise(this);
                    break;

                case RenderImageState.DownloadCompleted:
                    IsDownloading = false;
                    DownloadCompleted.Raise(this);
                    break;

                case RenderImageState.DownloadFailed:
                    IsDownloading = false;
                    DownloadFailed.Raise(this);
                    break;

                default: throw new Granular.Exception("Unexpected DownloadState \"{0}\"", renderImageState);
            }
        }

        public static BitmapSource Create(byte[] data, Rect sourceRect = null)
        {
            return Create(RenderImageType.Unknown, data, sourceRect);
        }

        public static BitmapSource Create(RenderImageType imageType, byte[] data, Rect sourceRect = null)
        {
            return new BitmapSource(ApplicationHost.Current.RenderImageSourceFactory.CreateRenderImageSource(imageType, data, sourceRect));
        }
    }
}
