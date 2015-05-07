using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace System.Windows.Input
{
    public enum KeyboardNavigationMode
    {
        Continue,
        Once,
        Cycle,
        None,
        Contained,
        Local
    }

    public enum FocusNavigationDirection
    {
        Next,
        Previous,
        First,
        Last,
        Left,
        Right,
        Up,
        Down
    }

    public class KeyboardNavigation : IDisposable
    {
        public static readonly DependencyProperty TabNavigationProperty = DependencyProperty.RegisterAttached("TabNavigation", typeof(KeyboardNavigationMode), typeof(KeyboardNavigation), new FrameworkPropertyMetadata(KeyboardNavigationMode.Continue));

        public static KeyboardNavigationMode GetTabNavigation(DependencyObject obj)
        {
            return (KeyboardNavigationMode)obj.GetValue(TabNavigationProperty);
        }

        public static void SetTabNavigation(DependencyObject obj, KeyboardNavigationMode value)
        {
            obj.SetValue(TabNavigationProperty, value);
        }

        public static readonly DependencyProperty ControlTabNavigationProperty = DependencyProperty.RegisterAttached("ControlTabNavigation", typeof(KeyboardNavigationMode), typeof(KeyboardNavigation), new FrameworkPropertyMetadata(KeyboardNavigationMode.Continue));

        public static KeyboardNavigationMode GetControlTabNavigation(DependencyObject obj)
        {
            return (KeyboardNavigationMode)obj.GetValue(ControlTabNavigationProperty);
        }

        public static void SetControlTabNavigation(DependencyObject obj, KeyboardNavigationMode value)
        {
            obj.SetValue(ControlTabNavigationProperty, value);
        }

        public static readonly DependencyProperty DirectionalNavigationProperty = DependencyProperty.RegisterAttached("DirectionalNavigation", typeof(KeyboardNavigationMode), typeof(KeyboardNavigation), new FrameworkPropertyMetadata(KeyboardNavigationMode.Continue));

        public static KeyboardNavigationMode GetDirectionalNavigation(DependencyObject obj)
        {
            return (KeyboardNavigationMode)obj.GetValue(DirectionalNavigationProperty);
        }

        public static void SetDirectionalNavigation(DependencyObject obj, KeyboardNavigationMode value)
        {
            obj.SetValue(DirectionalNavigationProperty, value);
        }

        public static readonly DependencyProperty IsTabStopProperty = DependencyProperty.RegisterAttached("IsTabStop", typeof(bool), typeof(KeyboardNavigation), new FrameworkPropertyMetadata(true));

        public static bool GetIsTabStop(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsTabStopProperty);
        }

        public static void SetIsTabStop(DependencyObject obj, bool value)
        {
            obj.SetValue(IsTabStopProperty, value);
        }

        public static readonly DependencyProperty TabIndexProperty = DependencyProperty.RegisterAttached("TabIndex", typeof(int), typeof(KeyboardNavigation), new FrameworkPropertyMetadata(Int32.MaxValue));

        public static int GetTabIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(TabIndexProperty);
        }

        public static void SetTabIndex(DependencyObject obj, int value)
        {
            obj.SetValue(TabIndexProperty, value);
        }

        private IPresentationSource presentationSource;

        public KeyboardNavigation(IPresentationSource presentationSource)
        {
            this.presentationSource = presentationSource;

            presentationSource.KeyboardDevice.PostProcessKey += OnPostProcessKey;
        }

        public void Dispose()
        {
            presentationSource.KeyboardDevice.TargetChanged -= OnTargetChanged;
            presentationSource.KeyboardDevice.PostProcessKey -= OnPostProcessKey;
        }

        private void OnPostProcessKey(object sender, KeyEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            FocusNavigationDirection navigationDirection;
            DependencyProperty navigationModeProperty;

            if (TryGetNavigationMode(e, out navigationModeProperty, out navigationDirection))
            {
                UIElement currentTarget = (UIElement)presentationSource.KeyboardDevice.Target ?? presentationSource.RootElement;

                UIElement newTarget = (UIElement)KeyboardNavigationTarget.FindTarget(currentTarget, navigationDirection, navigationModeProperty);

                if (currentTarget != newTarget && newTarget != null)
                {
                    newTarget.Focus();
                }

                e.Handled = true;
            }
        }

        private static bool TryGetNavigationMode(KeyEventArgs e, out DependencyProperty navigationModeProperty, out FocusNavigationDirection navigationDirection)
        {
            navigationModeProperty = null;
            navigationDirection = FocusNavigationDirection.Next;

            if (!e.IsDown)
            {
                return false;
            }

            if (e.Key == Key.Tab)
            {
                navigationModeProperty = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == 0 ? TabNavigationProperty : ControlTabNavigationProperty;
                navigationDirection = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == 0 ? FocusNavigationDirection.Next : FocusNavigationDirection.Previous;
                return true;
            }

            if (e.Key == Key.Left)
            {
                navigationModeProperty = DirectionalNavigationProperty;
                navigationDirection = FocusNavigationDirection.Left;
                return true;
            }

            if (e.Key == Key.Right)
            {
                navigationModeProperty = DirectionalNavigationProperty;
                navigationDirection = FocusNavigationDirection.Right;
                return true;
            }

            if (e.Key == Key.Up)
            {
                navigationModeProperty = DirectionalNavigationProperty;
                navigationDirection = FocusNavigationDirection.Up;
                return true;
            }

            if (e.Key == Key.Down)
            {
                navigationModeProperty = DirectionalNavigationProperty;
                navigationDirection = FocusNavigationDirection.Down;
                return true;
            }

            return false;
        }
    }
}
