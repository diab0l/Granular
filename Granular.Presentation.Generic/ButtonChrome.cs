using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Granular.Presentation.Generic
{
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.NormalState)]
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.MouseOverState)]
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.PressedState)]
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.DisabledState)]
    public class ButtonChrome : Control
    {
        public static readonly DependencyProperty RenderEnabledProperty = DependencyProperty.RegisterAttached("RenderEnabled", typeof(bool), typeof(ButtonChrome), new ControlPropertyMetadata(affectsVisualState: true));
        public bool RenderEnabled
        {
            get { return (bool)GetValue(RenderEnabledProperty); }
            set { SetValue(RenderEnabledProperty, value); }
        }

        public static readonly DependencyProperty RenderMouseOverProperty = DependencyProperty.RegisterAttached("RenderMouseOver", typeof(bool), typeof(ButtonChrome), new ControlPropertyMetadata(affectsVisualState: true));
        public bool RenderMouseOver
        {
            get { return (bool)GetValue(RenderMouseOverProperty); }
            set { SetValue(RenderMouseOverProperty, value); }
        }

        public static readonly DependencyProperty RenderPressedProperty = DependencyProperty.RegisterAttached("RenderPressed", typeof(bool), typeof(ButtonChrome), new ControlPropertyMetadata(affectsVisualState: true));
        public bool RenderPressed
        {
            get { return (bool)GetValue(RenderPressedProperty); }
            set { SetValue(RenderPressedProperty, value); }
        }

        public static readonly DependencyProperty RenderFocusedProperty = DependencyProperty.RegisterAttached("RenderFocused", typeof(bool), typeof(ButtonChrome), new FrameworkPropertyMetadata());
        public bool RenderFocused
        {
            get { return (bool)GetValue(RenderFocusedProperty); }
            set { SetValue(RenderFocusedProperty, value); }
        }

        public static readonly DependencyProperty RenderCornersProperty = DependencyProperty.RegisterAttached("RenderCorners", typeof(bool), typeof(ButtonChrome), new FrameworkPropertyMetadata(true));
        public bool RenderCorners
        {
            get { return (bool)GetValue(RenderCornersProperty); }
            set { SetValue(RenderCornersProperty, value); }
        }

        static ButtonChrome()
        {
            FocusableProperty.OverrideMetadata(typeof(ButtonChrome), new FrameworkPropertyMetadata(false));
        }

        protected override void UpdateVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, GetStateName(), useTransitions);
        }

        private string GetStateName()
        {
            if (!RenderEnabled)
            {
                return VisualStates.DisabledState;
            }

            if (RenderPressed)
            {
                return VisualStates.PressedState;
            }

            if (RenderMouseOver)
            {
                return VisualStates.MouseOverState;
            }

            return VisualStates.NormalState;
        }
    }
}