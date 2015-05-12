using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace System.Windows
{
    public partial class UIElement
    {
        #region Mouse
        public static readonly RoutedEvent PreviewMouseMoveEvent = Mouse.PreviewMouseMoveEvent.AddOwner(typeof(UIElement));
        public event MouseEventHandler PreviewMouseMove
        {
            add { AddHandler(Mouse.PreviewMouseMoveEvent, value, false); }
            remove { RemoveHandler(Mouse.PreviewMouseMoveEvent, value); }
        }

        public static readonly RoutedEvent PreviewMouseDownEvent = Mouse.PreviewMouseDownEvent.AddOwner(typeof(UIElement));
        public event MouseButtonEventHandler PreviewMouseDown
        {
            add { AddHandler(Mouse.PreviewMouseDownEvent, value, false); }
            remove { RemoveHandler(Mouse.PreviewMouseDownEvent, value); }
        }

        public static readonly RoutedEvent PreviewMouseUpEvent = Mouse.PreviewMouseUpEvent.AddOwner(typeof(UIElement));
        public event MouseButtonEventHandler PreviewMouseUp
        {
            add { AddHandler(Mouse.PreviewMouseUpEvent, value, false); }
            remove { RemoveHandler(Mouse.PreviewMouseUpEvent, value); }
        }

        public static readonly RoutedEvent PreviewMouseWheelEvent = Mouse.PreviewMouseWheelEvent.AddOwner(typeof(UIElement));
        public event MouseWheelEventHandler PreviewMouseWheel
        {
            add { AddHandler(Mouse.PreviewMouseWheelEvent, value, false); }
            remove { RemoveHandler(Mouse.PreviewMouseWheelEvent, value); }
        }

        public static readonly RoutedEvent MouseMoveEvent = Mouse.MouseMoveEvent.AddOwner(typeof(UIElement));
        public event MouseEventHandler MouseMove
        {
            add { AddHandler(Mouse.MouseMoveEvent, value, false); }
            remove { RemoveHandler(Mouse.MouseMoveEvent, value); }
        }

        public static readonly RoutedEvent MouseDownEvent = Mouse.MouseDownEvent.AddOwner(typeof(UIElement));
        public event MouseButtonEventHandler MouseDown
        {
            add { AddHandler(Mouse.MouseDownEvent, value, false); }
            remove { RemoveHandler(Mouse.MouseDownEvent, value); }
        }

        public static readonly RoutedEvent MouseUpEvent = Mouse.MouseUpEvent.AddOwner(typeof(UIElement));
        public event MouseButtonEventHandler MouseUp
        {
            add { AddHandler(Mouse.MouseUpEvent, value, false); }
            remove { RemoveHandler(Mouse.MouseUpEvent, value); }
        }

        public static readonly RoutedEvent MouseWheelEvent = Mouse.MouseWheelEvent.AddOwner(typeof(UIElement));
        public event MouseWheelEventHandler MouseWheel
        {
            add { AddHandler(Mouse.MouseWheelEvent, value, false); }
            remove { RemoveHandler(Mouse.MouseWheelEvent, value); }
        }

        public static readonly RoutedEvent MouseEnterEvent = Mouse.MouseEnterEvent.AddOwner(typeof(UIElement));
        public event MouseEventHandler MouseEnter
        {
            add { AddHandler(Mouse.MouseEnterEvent, value, false); }
            remove { RemoveHandler(Mouse.MouseEnterEvent, value); }
        }

        public static readonly RoutedEvent MouseLeaveEvent = Mouse.MouseLeaveEvent.AddOwner(typeof(UIElement));
        public event MouseEventHandler MouseLeave
        {
            add { AddHandler(Mouse.MouseLeaveEvent, value, false); }
            remove { RemoveHandler(Mouse.MouseLeaveEvent, value); }
        }

        public static readonly RoutedEvent QueryCursorEvent = Mouse.QueryCursorEvent.AddOwner(typeof(UIElement));
        public event QueryCursorEventHandler QueryCursor
        {
            add { AddHandler(Mouse.QueryCursorEvent, value, false); }
            remove { RemoveHandler(Mouse.QueryCursorEvent, value); }
        }

        private static void OnMouseEnter(object sender, MouseEventArgs e)
        {
            ((UIElement)sender).IsMouseOver = true;
            ((UIElement)sender).OnMouseEnter(e);
        }

        protected virtual void OnMouseEnter(MouseEventArgs e)
        {
            //
        }

        private static void OnMouseLeave(object sender, MouseEventArgs e)
        {
            ((UIElement)sender).IsMouseOver = false;
            ((UIElement)sender).OnMouseLeave(e);
        }

        protected virtual void OnMouseLeave(MouseEventArgs e)
        {
            //
        }

        protected virtual void OnQueryCursor(QueryCursorEventArgs e)
        {
            //
        }

        protected virtual void OnPreviewMouseMove(MouseEventArgs e)
        {
            //
        }

        protected virtual void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            //
        }

        protected virtual void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            //
        }

        protected virtual void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            //
        }

        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            //
        }

        protected virtual void OnMouseDown(MouseButtonEventArgs e)
        {
            //
        }

        protected virtual void OnMouseUp(MouseButtonEventArgs e)
        {
            //
        }

        protected virtual void OnMouseWheel(MouseWheelEventArgs e)
        {
            //
        }
        #endregion Mouse

        #region Keyboard
        public static readonly RoutedEvent PreviewKeyDownEvent = Keyboard.PreviewKeyDownEvent.AddOwner(typeof(UIElement));
        public event KeyEventHandler PreviewKeyDown
        {
            add { AddHandler(Keyboard.PreviewKeyDownEvent, value, false); }
            remove { RemoveHandler(Keyboard.PreviewKeyDownEvent, value); }
        }

        public static readonly RoutedEvent PreviewKeyUpEvent = Keyboard.PreviewKeyUpEvent.AddOwner(typeof(UIElement));
        public event KeyEventHandler PreviewKeyUp
        {
            add { AddHandler(Keyboard.PreviewKeyUpEvent, value, false); }
            remove { RemoveHandler(Keyboard.PreviewKeyUpEvent, value); }
        }

        public static readonly RoutedEvent PreviewGotKeyboardFocusEvent = Keyboard.PreviewGotKeyboardFocusEvent.AddOwner(typeof(UIElement));
        public event KeyboardFocusChangedEventHandler PreviewGotKeyboardFocus
        {
            add { AddHandler(Keyboard.PreviewGotKeyboardFocusEvent, value, false); }
            remove { RemoveHandler(Keyboard.PreviewGotKeyboardFocusEvent, value); }
        }

        public static readonly RoutedEvent PreviewLostKeyboardFocusEvent = Keyboard.PreviewLostKeyboardFocusEvent.AddOwner(typeof(UIElement));
        public event KeyboardFocusChangedEventHandler PreviewLostKeyboardFocus
        {
            add { AddHandler(Keyboard.PreviewLostKeyboardFocusEvent, value, false); }
            remove { RemoveHandler(Keyboard.PreviewLostKeyboardFocusEvent, value); }
        }

        public static readonly RoutedEvent KeyDownEvent = Keyboard.KeyDownEvent.AddOwner(typeof(UIElement));
        public event KeyEventHandler KeyDown
        {
            add { AddHandler(Keyboard.KeyDownEvent, value, false); }
            remove { RemoveHandler(Keyboard.KeyDownEvent, value); }
        }

        public static readonly RoutedEvent KeyUpEvent = Keyboard.KeyUpEvent.AddOwner(typeof(UIElement));
        public event KeyEventHandler KeyUp
        {
            add { AddHandler(Keyboard.KeyUpEvent, value, false); }
            remove { RemoveHandler(Keyboard.KeyUpEvent, value); }
        }

        public static readonly RoutedEvent GotKeyboardFocusEvent = Keyboard.GotKeyboardFocusEvent.AddOwner(typeof(UIElement));
        public event KeyboardFocusChangedEventHandler GotKeyboardFocus
        {
            add { AddHandler(Keyboard.GotKeyboardFocusEvent, value, false); }
            remove { RemoveHandler(Keyboard.GotKeyboardFocusEvent, value); }
        }

        public static readonly RoutedEvent LostKeyboardFocusEvent = Keyboard.LostKeyboardFocusEvent.AddOwner(typeof(UIElement));
        public event KeyboardFocusChangedEventHandler LostKeyboardFocus
        {
            add { AddHandler(Keyboard.LostKeyboardFocusEvent, value, false); }
            remove { RemoveHandler(Keyboard.LostKeyboardFocusEvent, value); }
        }

        private static void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((UIElement)sender).IsKeyboardFocused = e.OriginalSource == sender;
            ((UIElement)sender).IsKeyboardFocusWithin = true;
            ((UIElement)sender).OnGotKeyboardFocus(e);
        }

        protected virtual void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            //
        }

        private static void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((UIElement)sender).IsKeyboardFocused = false;
            ((UIElement)sender).IsKeyboardFocusWithin = false;
            ((UIElement)sender).OnLostKeyboardFocus(e);
        }

        protected virtual void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            //
        }

        protected virtual void OnPreviewKeyDown(KeyEventArgs e)
        {
            //
        }

        protected virtual void OnPreviewKeyUp(KeyEventArgs e)
        {
            //
        }

        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            //
        }

        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            //
        }
        #endregion Keyboard

        #region Focus
        public static readonly RoutedEvent GotFocusEvent = FocusManager.GotFocusEvent.AddOwner(typeof(UIElement));
        public event RoutedEventHandler GotFocus
        {
            add { AddHandler(GotFocusEvent, value); }
            remove { RemoveHandler(GotFocusEvent, value); }
        }

        public static readonly RoutedEvent LostFocusEvent = FocusManager.LostFocusEvent.AddOwner(typeof(UIElement));
        public event RoutedEventHandler LostFocus
        {
            add { AddHandler(LostFocusEvent, value); }
            remove { RemoveHandler(LostFocusEvent, value); }
        }

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            ((UIElement)sender).IsFocused = true;
            ((UIElement)sender).OnGotFocus(e);
        }

        protected virtual void OnGotFocus(RoutedEventArgs e)
        {
            //
        }

        private static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            ((UIElement)sender).IsFocused = false;
            ((UIElement)sender).OnLostFocus(e);
        }

        protected virtual void OnLostFocus(RoutedEventArgs e)
        {
            //
        }
        #endregion Focus

        static UIElement()
        {
            EventManager.RegisterClassHandler(typeof(UIElement), Mouse.MouseEnterEvent, (MouseEventHandler)OnMouseEnter, false);
            EventManager.RegisterClassHandler(typeof(UIElement), Mouse.MouseLeaveEvent, (MouseEventHandler)OnMouseLeave, false);
            EventManager.RegisterClassHandler(typeof(UIElement), Mouse.QueryCursorEvent, (QueryCursorEventHandler)((sender, e) => ((UIElement)sender).OnQueryCursor(e)), true);

            EventManager.RegisterClassHandler(typeof(UIElement), Mouse.PreviewMouseMoveEvent, (MouseEventHandler)((sender, e) => ((UIElement)sender).OnPreviewMouseMove(e)), false);
            EventManager.RegisterClassHandler(typeof(UIElement), Mouse.PreviewMouseDownEvent, (MouseButtonEventHandler)((sender, e) => ((UIElement)sender).OnPreviewMouseDown(e)), false);
            EventManager.RegisterClassHandler(typeof(UIElement), Mouse.PreviewMouseUpEvent, (MouseButtonEventHandler)((sender, e) => ((UIElement)sender).OnPreviewMouseUp(e)), false);
            EventManager.RegisterClassHandler(typeof(UIElement), Mouse.PreviewMouseWheelEvent, (MouseWheelEventHandler)((sender, e) => ((UIElement)sender).OnPreviewMouseWheel(e)), false);

            EventManager.RegisterClassHandler(typeof(UIElement), Mouse.MouseMoveEvent, (MouseEventHandler)((sender, e) => ((UIElement)sender).OnMouseMove(e)), false);
            EventManager.RegisterClassHandler(typeof(UIElement), Mouse.MouseDownEvent, (MouseButtonEventHandler)((sender, e) => ((UIElement)sender).OnMouseDown(e)), false);
            EventManager.RegisterClassHandler(typeof(UIElement), Mouse.MouseUpEvent, (MouseButtonEventHandler)((sender, e) => ((UIElement)sender).OnMouseUp(e)), false);
            EventManager.RegisterClassHandler(typeof(UIElement), Mouse.MouseWheelEvent, (MouseWheelEventHandler)((sender, e) => ((UIElement)sender).OnMouseWheel(e)), false);

            EventManager.RegisterClassHandler(typeof(UIElement), Keyboard.GotKeyboardFocusEvent, (KeyboardFocusChangedEventHandler)OnGotKeyboardFocus, true);
            EventManager.RegisterClassHandler(typeof(UIElement), Keyboard.LostKeyboardFocusEvent, (KeyboardFocusChangedEventHandler)OnLostKeyboardFocus, true);

            EventManager.RegisterClassHandler(typeof(UIElement), Keyboard.PreviewKeyDownEvent, (KeyEventHandler)((sender, e) => ((UIElement)sender).OnPreviewKeyDown(e)), false);
            EventManager.RegisterClassHandler(typeof(UIElement), Keyboard.PreviewKeyUpEvent, (KeyEventHandler)((sender, e) => ((UIElement)sender).OnPreviewKeyUp(e)), false);
            EventManager.RegisterClassHandler(typeof(UIElement), Keyboard.KeyDownEvent, (KeyEventHandler)((sender, e) => ((UIElement)sender).OnKeyDown(e)), false);
            EventManager.RegisterClassHandler(typeof(UIElement), Keyboard.KeyUpEvent, (KeyEventHandler)((sender, e) => ((UIElement)sender).OnKeyUp(e)), false);

            EventManager.RegisterClassHandler(typeof(UIElement), FocusManager.GotFocusEvent, (RoutedEventHandler)OnGotFocus, false);
            EventManager.RegisterClassHandler(typeof(UIElement), FocusManager.LostFocusEvent, (RoutedEventHandler)OnLostFocus, false);
        }
    }
}
