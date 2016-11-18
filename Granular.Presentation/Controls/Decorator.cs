using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace System.Windows.Controls
{
    [ContentProperty("Child")]
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class Decorator : FrameworkElement
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

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Child == null)
            {
                return Size.Zero;
            }

            Child.Measure(availableSize);
            return Child.DesiredSize;
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
