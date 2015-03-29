using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Granular.Extensions;

namespace Granular.Presentation.Tests.Input
{
    public class TestPresentationSource : IPresentationSource
    {
        public event EventHandler HitTestInvalidated;

        public UIElement RootElement { get; set; }

        public MouseDevice MouseDevice { get; set; }
        public KeyboardDevice KeyboardDevice { get; set; }

        public string Title { get; set; }

        public TestPresentationSource()
        {
            //
        }

        public IInputElement HitTest(Point position)
        {
            return RootElement != null ? RootElement.HitTest(position) as IInputElement : null;
        }

        public void InvalidateHitTest()
        {
            HitTestInvalidated.Raise(this);
        }

        public int GetTimestamp()
        {
            return 0;
        }
    }
}
