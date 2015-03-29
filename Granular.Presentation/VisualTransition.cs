using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace System.Windows
{
    [ContentProperty("Storyboard")]
    public class VisualTransition : Freezable
    {
        public string From { get; set; }

        public string To { get; set; }

        public static readonly DependencyProperty StoryboardProperty = DependencyProperty.Register("Storyboard", typeof(Storyboard), typeof(VisualTransition), new FrameworkPropertyMetadata());
        public Storyboard Storyboard
        {
            get { return (Storyboard)GetValue(StoryboardProperty); }
            set { SetValue(StoryboardProperty, value); }
        }
    }
}
