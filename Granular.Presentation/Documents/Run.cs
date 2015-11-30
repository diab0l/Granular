using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace System.Windows.Documents
{
    [ContentProperty("Text")]
    public class Run : Inline
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Run), new FrameworkPropertyMetadata());
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Run()
        {
            //
        }

        public Run(string text)
        {
            this.Text = text;
        }

        public override object GetRenderElement(Media.IRenderElementFactory factory)
        {
            return null;
        }

        public override void RemoveRenderElement(Media.IRenderElementFactory factory)
        {
            //
        }
    }
}
