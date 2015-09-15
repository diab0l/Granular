using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace System.Windows.Input
{
    public static class FocusManager
    {
        public static readonly RoutedEvent GotFocusEvent = EventManager.RegisterRoutedEvent("GotFocus", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FocusManager));
        public static readonly RoutedEvent LostFocusEvent = EventManager.RegisterRoutedEvent("LostFocus", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FocusManager));

        public static readonly DependencyProperty FocusedElementProperty = DependencyProperty.RegisterAttached("FocusedElement", typeof(IInputElement), typeof(FocusManager), new FrameworkPropertyMetadata(null, OnFocusedElementChanged));

        public static IInputElement GetFocusedElement(DependencyObject obj)
        {
            return (IInputElement)obj.GetValue(FocusedElementProperty);
        }

        public static void SetFocusedElement(DependencyObject obj, IInputElement value)
        {
            obj.SetValue(FocusedElementProperty, value);
        }

        public static readonly DependencyProperty IsFocusScopeProperty = DependencyProperty.RegisterAttached("IsFocusScope", typeof(bool), typeof(FocusManager), new FrameworkPropertyMetadata());

        public static bool GetIsFocusScope(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusScopeProperty);
        }

        public static void SetIsFocusScope(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusScopeProperty, value);
        }

        public static UIElement GetFocusScope(UIElement element)
        {
            while (element != null && !GetIsFocusScope(element))
            {
                element = (UIElement)(element.LogicalParent ?? element.VisualParent);
            }

            return element;
        }

        public static IDisposable Focus(UIElement element)
        {
            if (!element.Focusable)
            {
                return null;
            }

            UIElement focusScope = GetFocusScope(element);

            if (focusScope != null)
            {
                SetFocusedElement(focusScope, element);
                return new Disposable(() =>
                {
                    if (GetFocusedElement(focusScope) == element)
                    {
                        SetFocusedElement(focusScope, null);
                    }
                });
            }

            return null;
        }

        private static void OnFocusedElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement oldElement = (FrameworkElement)e.OldValue;
            FrameworkElement newElement = (FrameworkElement)e.NewValue;

            if (oldElement != null)
            {
                oldElement.RaiseEvent(new RoutedEventArgs(LostFocusEvent, oldElement));
            }

            if (newElement != null)
            {
                newElement.RaiseEvent(new RoutedEventArgs(GotFocusEvent, newElement));
            }
        }
    }
}
