using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;

namespace System.Windows.Controls
{
    public class CheckBox : ToggleButton
    {
        static CheckBox()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBox), new FrameworkPropertyMetadata(new StyleKey(typeof(CheckBox))));
        }
    }
}
