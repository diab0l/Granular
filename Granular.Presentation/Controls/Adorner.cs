using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Controls
{
    public class Adorner : FrameworkElement
    {
        private UIElement child;
        public UIElement Child
        {
            get { return child; }
            set
            {
                if (child == value)
                {
                    return;
                }

                if (child != null)
                {
                    RemoveLogicalChild(child);
                    RemoveVisualChild(child);
                }

                child = value;

                if (child != null)
                {
                    AddLogicalChild(child);
                    AddVisualChild(child);
                }

                InvalidateMeasure();
            }
        }

        public UIElement AdornedElement { get; private set; }

        static Adorner()
        {
            ClipToBoundsProperty.OverrideMetadata(typeof(Adorner), new FrameworkPropertyMetadata(false));
        }

        public Adorner(UIElement adornedElement)
        {
            this.AdornedElement = adornedElement;
            VisualClipToBounds = false;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Child == null)
            {
                return Size.Zero;
            }

            Child.Measure(AdornedElement.RenderSize);
            return AdornedElement.RenderSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Child != null)
            {
                Child.Arrange(new Rect(finalSize));
            }

            return finalSize;
        }
    }
}
