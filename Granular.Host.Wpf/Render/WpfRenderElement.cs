extern alias wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Host.Wpf.Render
{
    public interface IWpfRenderElement
    {
        wpf::System.Windows.FrameworkElement WpfElement { get; }
    }

    public interface IWpfDeferredRenderElement : IWpfRenderElement
    {
        event EventHandler WpfElementCreated;
    }
}
