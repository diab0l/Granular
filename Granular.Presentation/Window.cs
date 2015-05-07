using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace System.Windows
{
    public class Window : ContentControl, IPopupLayerHost, IAdornerLayerHost, IRadioButtonGroupScope
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Window), new FrameworkPropertyMetadata(String.Empty, (sender, e) => ((Window)sender).OnTitleChanged(e)));
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public AdornerLayer AdornerLayer { get; private set; }
        public PopupLayer PopupLayer { get; private set; }

        private IPresentationSource presentationSource;
        private KeyboardNavigation keyboardNavigation;
        private ISelectionGroupScope<RadioButton> radioButtonGroupScope;

        static Window()
        {
            Control.IsTabStopProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(false));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
            FocusManager.IsFocusScopeProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(true));
        }

        public Window()
        {
            SetResourceInheritanceParent(Application.Current);

            AdornerLayer = new AdornerLayer();
            AddVisualChild(AdornerLayer);

            PopupLayer = new PopupLayer();
            AddVisualChild(PopupLayer);

            radioButtonGroupScope = new SelectionGroupScope<RadioButton>();
        }

        protected override void OnTemplateChildChanged()
        {
            SetVisualChildIndex(AdornerLayer, VisualChildren.Count() - 2);
            SetVisualChildIndex(PopupLayer, VisualChildren.Count() - 1);
        }

        public void Show()
        {
            if (presentationSource != null)
            {
                return;
            }

            if (Application.Current.MainWindow == null)
            {
                Application.Current.MainWindow = this;
            }

            presentationSource = ApplicationHost.Current.PresentationSourceFactory.CreatePresentationSource(this);
            presentationSource.Title = this.Title;

            keyboardNavigation = new KeyboardNavigation(presentationSource);
        }

        public ISelectionGroup<RadioButton> GetRadioButtonGroup(string groupName)
        {
            return radioButtonGroupScope.GetSelectionGroup(groupName);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            AdornerLayer.Measure(availableSize);
            PopupLayer.Measure(availableSize);
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            AdornerLayer.Arrange(new Rect(finalSize));
            PopupLayer.Arrange(new Rect(finalSize));
            return base.ArrangeOverride(finalSize);
        }

        private void OnTitleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (presentationSource != null)
            {
                presentationSource.Title = this.Title;
            }
        }
    }
}
