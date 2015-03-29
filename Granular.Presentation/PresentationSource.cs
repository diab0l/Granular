using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace System.Windows
{
    public interface IPresentationSource
    {
        event EventHandler HitTestInvalidated;

        UIElement RootElement { get; }

        MouseDevice MouseDevice { get; }
        KeyboardDevice KeyboardDevice { get; }

        string Title { get; set; }

        IInputElement HitTest(Point position);

        int GetTimestamp();
    }

    public interface IPresentationSourceFactory
    {
        IPresentationSource CreatePresentationSource(UIElement rootElement);
        IPresentationSource GetPresentationSourceFromElement(UIElement element);
    }
}
