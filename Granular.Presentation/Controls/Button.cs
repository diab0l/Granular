using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;

namespace System.Windows.Controls
{
    public class Button : ButtonBase
    {
        static Button()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(new StyleKey(typeof(Button))));
        }
    }
}
