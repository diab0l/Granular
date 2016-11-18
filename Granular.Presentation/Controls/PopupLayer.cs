using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using Granular.Extensions;

namespace System.Windows.Controls
{
    public interface IPopupLayerHost
    {
        PopupLayer PopupLayer { get; }
    }

    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class PopupLayer : FrameworkElement
    {
        public event EventHandler ClosePopupRequest;

        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached("Position", typeof(Point), typeof(PopupLayer), new FrameworkPropertyMetadata(Point.Zero, propertyChangedCallback: OnPositionChanged));

        public static Point GetPosition(DependencyObject obj)
        {
            return (Point)obj.GetValue(PositionProperty);
        }

        public static void SetPosition(DependencyObject obj, Point value)
        {
            obj.SetValue(PositionProperty, value);
        }

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.RegisterAttached("IsOpen", typeof(bool), typeof(PopupLayer), new FrameworkPropertyMetadata(propertyChangedCallback: OnIsOpenChanged));

        public static bool GetIsOpen(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsOpenProperty);
        }

        public static void SetIsOpen(DependencyObject obj, bool value)
        {
            obj.SetValue(IsOpenProperty, value);
        }

        public static readonly DependencyProperty StaysOpenProperty = DependencyProperty.RegisterAttached("StaysOpen", typeof(bool), typeof(PopupLayer), new FrameworkPropertyMetadata(true));

        public static bool GetStaysOpen(DependencyObject obj)
        {
            return (bool)obj.GetValue(StaysOpenProperty);
        }

        public static void SetStaysOpen(DependencyObject obj, bool value)
        {
            obj.SetValue(StaysOpenProperty, value);
        }

        public PopupLayer()
        {
            //
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in VisualChildren)
            {
                child.Measure(availableSize);
            }

            return Size.Zero;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in VisualChildren)
            {
                child.Arrange(new Rect(GetPosition(child), child.DesiredSize));
            }

            return finalSize;
        }

        public void AddChild(Visual child)
        {
            AddVisualChild(child);
            BringToFront(child);

            InvalidateMeasure();
        }

        public void RemoveChild(Visual child)
        {
            RemoveVisualChild(child);

            InvalidateMeasure();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.OriginalSource == this)
            {
                ClosePopupRequest.Raise(this);
                e.Handled = e.MouseDevice.ProcessRawEvent(new RawMouseButtonEventArgs(e.ChangedButton, e.ButtonState, e.AbsolutePosition, e.Timestamp));
            }
        }

        protected override bool HitTestOverride(Point position)
        {
            // receive a click if there is a non "StaysOpen" child opened
            return VisualChildren.Any(child => GetIsOpen(child) && !GetStaysOpen(child));
        }

        public static PopupLayer GetPopupLayer(Visual target)
        {
            if (target == null)
            {
                return null;
            }

            return target is IPopupLayerHost ? ((IPopupLayerHost)target).PopupLayer : GetPopupLayer(target.VisualParent);
        }

        private void BringToFront(Visual child)
        {
            SetVisualChildIndex(child, VisualChildren.Count() - 1);
        }

        private static void OnIsOpenChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && dependencyObject is Visual && ((Visual)dependencyObject).VisualParent is PopupLayer)
            {
                ((PopupLayer)((Visual)dependencyObject).VisualParent).BringToFront((Visual)dependencyObject);
            }
        }

        private static void OnPositionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is Visual && ((Visual)dependencyObject).VisualParent is PopupLayer)
            {
                ((PopupLayer)((Visual)dependencyObject).VisualParent).InvalidateArrange();
            }
        }
    }
}
