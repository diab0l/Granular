using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace System.Windows
{
    public abstract class ContentElement : DependencyObject
    {
        public ContentElement()
        {
            //
        }

        public abstract object GetRenderElement(IRenderElementFactory factory);

        public abstract void RemoveRenderElement(IRenderElementFactory factory);
    }
}
