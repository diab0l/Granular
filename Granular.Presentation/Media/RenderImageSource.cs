using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Media
{
    public enum RenderImageType
    {
        Unknown,
        Gif,
        Jpeg,
        Png,
        Svg,
    }

    public enum RenderImageState
    {
        Idle,
        DownloadProgress,
        DownloadCompleted,
        DownloadFailed
    }

    public interface IRenderImageSource
    {
        event EventHandler StateChanged;
        RenderImageState State { get; }
        Size Size { get; }
    }
}
