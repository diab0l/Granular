using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media;
using Granular.Extensions;

namespace System.Windows.Controls.Primitives
{
    [ContentProperty("Child")]
    public class Popup : FrameworkElement
    {
        private class PopupContainer : FrameworkElement, IAdornerLayerHost
        {
            public AdornerLayer AdornerLayer { get; private set; }

            // the actual popup content
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
                        RemoveVisualChild(child);
                    }

                    child = value;

                    if (child != null)
                    {
                        AddVisualChild(child);
                        SetVisualChildIndex(child, 0);
                    }

                    InvalidateMeasure();
                    InvalidateArrange();
                }
            }

            private Point position;
            public Point Position
            {
                get { return position; }
                set
                {
                    position = value;
                    PopupLayer.SetPosition(this, position);
                }
            }

            private bool isOpen;
            public bool IsOpen
            {
                get { return isOpen; }
                set
                {
                    isOpen = value;
                    Visibility = isOpen ? Visibility.Visible : Visibility.Collapsed;
                    PopupLayer.SetIsOpen(this, isOpen);
                }
            }

            public PopupContainer()
            {
                Visibility = Visibility.Collapsed;

                AdornerLayer = new AdornerLayer();
                AddVisualChild(AdornerLayer);
            }

            protected override Size MeasureOverride(Size availableSize)
            {
                AdornerLayer.Measure(availableSize);

                if (Child == null)
                {
                    return Size.Zero;
                }

                Child.Measure(availableSize);
                return Child.DesiredSize;
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                AdornerLayer.Arrange(new Rect(finalSize));

                if (Child != null)
                {
                    Child.Arrange(new Rect(finalSize));
                }

                return finalSize;
            }
        }

        public event EventHandler Opened;
        public event EventHandler Closed;

        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(Popup), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((Popup)sender).SetPosition()));
        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register("VerticalOffset", typeof(double), typeof(Popup), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((Popup)sender).SetPosition()));
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register("Placement", typeof(PlacementMode), typeof(Popup), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((Popup)sender).SetPosition()));
        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        public static readonly DependencyProperty PlacementRectangleProperty = DependencyProperty.Register("PlacementRectangle", typeof(Rect), typeof(Popup), new FrameworkPropertyMetadata(Rect.Empty, propertyChangedCallback: (sender, e) => ((Popup)sender).SetPosition()));
        public Rect PlacementRectangle
        {
            get { return (Rect)GetValue(PlacementRectangleProperty); }
            set { SetValue(PlacementRectangleProperty, value); }
        }

        public static readonly DependencyProperty PlacementTargetProperty = DependencyProperty.Register("PlacementTarget", typeof(Visual), typeof(Popup), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((Popup)sender).SetPosition()));
        public Visual PlacementTarget
        {
            get { return (Visual)GetValue(PlacementTargetProperty); }
            set { SetValue(PlacementTargetProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(Popup), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((Popup)sender).OnIsOpenChanged(e)));
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty StaysOpenProperty = DependencyProperty.Register("StaysOpen", typeof(bool), typeof(Popup), new FrameworkPropertyMetadata(true, propertyChangedCallback: (sender, e) => ((Popup)sender).OnStaysOpenChanged(e)));
        public bool StaysOpen
        {
            get { return (bool)GetValue(StaysOpenProperty); }
            set { SetValue(StaysOpenProperty, value); }
        }

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
                }

                child = value;
                popupContainer.Child = child;

                if (child != null)
                {
                    AddLogicalChild(child);
                }
            }
        }

        private PopupLayer popupLayer;
        private PopupLayer PopupLayer
        {
            get { return popupLayer; }
            set
            {
                if (popupLayer == value)
                {
                    return;
                }

                if (popupLayer != null)
                {
                    popupLayer.RemoveChild(popupContainer);
                    popupLayer.ClosePopupRequest -= OnClosePopupRequest;
                }

                popupLayer = value;

                if (popupLayer != null)
                {
                    popupLayer.AddChild(popupContainer);
                    popupLayer.ClosePopupRequest += OnClosePopupRequest;
                }

                SetPosition();
            }
        }

        private PopupContainer popupContainer;

        public Popup()
        {
            popupContainer = new PopupContainer();
        }

        protected virtual void OnOpened()
        {
            //
        }

        protected virtual void OnClosed()
        {
            //
        }

        protected override void OnVisualAncestorChanged()
        {
            base.OnVisualAncestorChanged();
            PopupLayer = PopupLayer.GetPopupLayer(this);
        }

        private void OnStaysOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            PopupLayer.SetStaysOpen(popupContainer, (bool)e.NewValue);
        }

        private void OnIsOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            bool isOpen = (bool)e.NewValue;

            popupContainer.IsOpen = isOpen;

            if (isOpen)
            {
                popupContainer.UpdateLayout();
                SetPosition();

                OnOpened();
                Opened.Raise(this);
            }
            else
            {
                OnClosed();
                Closed.Raise(this);
            }
        }

        private void OnClosePopupRequest(object sender, EventArgs e)
        {
            if (!StaysOpen)
            {
                IsOpen = false;
            }
        }

        private void SetPosition()
        {
            if (PopupLayer != null && IsOpen)
            {
                Rect placementTargetRect = PlacementTarget != null ? new Rect(PopupLayer.PointFromRoot(PlacementTarget.PointToRoot(Point.Zero)), PlacementTarget.VisualSize) : Rect.Zero;
                Point position = System.Windows.Controls.Primitives.Placement.GetPosition(Placement, placementTargetRect, PlacementRectangle, GetMouseBounds(), new Point(HorizontalOffset, VerticalOffset), popupContainer.VisualSize, new Rect(PopupLayer.VisualSize));

                popupContainer.Position = position;
                PopupLayer.UpdateLayout();
            }
        }

        private Rect GetMouseBounds()
        {
            if (Placement != PlacementMode.Mouse && Placement != PlacementMode.MousePoint)
            {
                return Rect.Zero;
            }

            return new Rect(PopupLayer.PointFromRoot(ApplicationHost.Current.GetMouseDeviceFromElement(PopupLayer).Position), new Size(12, 19));
        }
    }
}
