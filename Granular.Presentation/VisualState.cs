using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace System.Windows
{
    [RuntimeNameProperty("Name")]
    [ContentProperty("Storyboard")]
    public class VisualState : Freezable
    {
        public string Name { get; set; }

        public static readonly DependencyProperty StoryboardProperty = DependencyProperty.Register("Storyboard", typeof(Storyboard), typeof(VisualState), new FrameworkPropertyMetadata());
        public Storyboard Storyboard
        {
            get { return (Storyboard)GetValue(StoryboardProperty); }
            set { SetValue(StoryboardProperty, value); }
        }
    }
}
