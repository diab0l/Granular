using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace System.Windows.Media.Imaging
{
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public abstract class BitmapSource : ImageSource
    {
        public event EventHandler DownloadProgress;
        public event EventHandler DownloadCompleted;
        public event EventHandler DownloadFailed;

        public bool IsDownloading { get; private set; }

        private RenderImageState renderImageState;
        private IImageSourceRenderResource renderResource;

        protected override void OnRenderResourceCreated(object renderResource)
        {
            base.OnRenderResourceCreated(renderResource);

            this.renderResource = (IImageSourceRenderResource)renderResource;
            this.renderResource.StateChanged += (sender, e) => SetRenderImageState();

            SetRenderImageState();
        }

        private void SetRenderImageState()
        {
            if (this.renderImageState == renderResource.State)
            {
                return;
            }

            RenderImageState oldRenderImageState = this.renderImageState;
            this.renderImageState = renderResource.State;

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
    }
}
