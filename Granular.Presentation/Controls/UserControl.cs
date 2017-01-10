using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Controls
{
    public class UserControl : ContentControl
    {
        static UserControl()
        {
            Control.IsTabStopProperty.OverrideMetadata(typeof(UserControl), new FrameworkPropertyMetadata(false));
            Control.HorizontalContentAlignmentProperty.OverrideMetadata(typeof(UserControl), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch));
            Control.VerticalContentAlignmentProperty.OverrideMetadata(typeof(UserControl), new FrameworkPropertyMetadata(VerticalAlignment.Stretch));
            UIElement.FocusableProperty.OverrideMetadata(typeof(UserControl), new FrameworkPropertyMetadata(false));
        }
    }
}
