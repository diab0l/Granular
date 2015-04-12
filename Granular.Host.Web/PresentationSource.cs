using System;
using System.Collections.Generic;
using System.Html;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Granular.Host.Render;

namespace Granular.Host
{
    public class PresentationSourceFactory : IPresentationSourceFactory
    {
        public static readonly IPresentationSourceFactory Default = new PresentationSourceFactory();

        private List<PresentationSource> presentationSources;

        private PresentationSourceFactory()
        {
            presentationSources = new List<PresentationSource>();
        }

        public IPresentationSource CreatePresentationSource(UIElement rootElement)
        {
            PresentationSource presentationSource = new PresentationSource(rootElement, HtmlValueConverter.Default);
            presentationSources.Add(presentationSource);

            return presentationSource;
        }

        public IPresentationSource GetPresentationSourceFromElement(UIElement element)
        {
            while (element.VisualParent is FrameworkElement)
            {
                element = (FrameworkElement)element.VisualParent;
            }

            return presentationSources.FirstOrDefault(presentationSource => presentationSource.RootElement == element);
        }
    }

    public class PresentationSource : IPresentationSource
    {
        public event EventHandler HitTestInvalidated { add { } remove { } }

        public UIElement RootElement { get; private set; }

        public MouseDevice MouseDevice { get; private set; }
        public KeyboardDevice KeyboardDevice { get; private set; }

        public string Title
        {
            get { return System.Html.Window.Document.Title; }
            set { System.Html.Window.Document.Title = value; }
        }

        private IHtmlValueConverter converter;

        private System.Html.WindowInstance window;

        private bool mouseDownHandled;
        private bool mouseMoveHandled;
        private bool mouseUpHandled;

        private bool keyDownHandled;
        private bool keyUpHandled;

        public PresentationSource(UIElement rootElement, IHtmlValueConverter converter)
        {
            this.RootElement = rootElement;
            this.converter = converter;

            RootElement.IsRootElement = true;

            MouseDevice = new MouseDevice(this);
            KeyboardDevice = new KeyboardDevice(this);

            window = System.Html.Window.Instance;

            MouseDevice.CursorChanged += (sender, e) => System.Html.Window.Document.Body.Style.Cursor = converter.ToCursorString(MouseDevice.Cursor);
            System.Html.Window.Document.Body.Style.Cursor = converter.ToCursorString(MouseDevice.Cursor);

            System.Html.Window.OnKeydown = OnKeyDown;
            System.Html.Window.OnKeyup = OnKeyUp;
            System.Html.Window.OnKeypress = PreventKeyboardHandled;
            System.Html.Window.OnMousemove = OnMouseMove;
            System.Html.Window.OnMousedown = OnMouseDown;
            System.Html.Window.OnMouseup = OnMouseUp;
            System.Html.Window.OnScroll = OnMouseWheel;
            System.Html.Window.OnFocus = e => MouseDevice.Activate();
            System.Html.Window.OnBlur = e => MouseDevice.Deactivate();
            System.Html.Window.OnResize = e => SetRootElementSize();
            System.Html.Window.OnClick = PreventMouseHandled;
            System.Html.Window.OnDblclick = PreventMouseHandled;
            System.Html.Window.OnContextmenu = PreventMouseHandled;
            System.Html.Window.AddEventListener("wheel", OnMouseWheel);

            SetRootElementSize();
            System.Html.Window.Document.Body.Style.Overflow = "hidden";
            System.Html.Window.Document.Body.AppendChild(((HtmlRenderElement)RootElement.GetRenderElement(HtmlRenderElementFactory.Default)).HtmlElement);

            MouseDevice.Activate();
            KeyboardDevice.Activate();
        }

        private void SetRootElementSize()
        {
            ((FrameworkElement)RootElement).Width = window.InnerWidth;
            ((FrameworkElement)RootElement).Height = window.InnerHeight;
        }

        private void OnKeyDown(Event e)
        {
            KeyboardEvent keyboardEvent = (KeyboardEvent)e;

            Key key = converter.ConvertBackKey(keyboardEvent.KeyCode, keyboardEvent.Location);

            keyDownHandled = KeyboardDevice.ProcessRawEvent(new RawKeyboardEventArgs(key, KeyStates.Down, keyboardEvent.Repeat, GetTimestamp()));

            if (keyDownHandled)
            {
                e.PreventDefault();
            }
        }

        private void OnKeyUp(Event e)
        {
            KeyboardEvent keyboardEvent = (KeyboardEvent)e;

            Key key = converter.ConvertBackKey(keyboardEvent.KeyCode, keyboardEvent.Location);

            keyUpHandled = KeyboardDevice.ProcessRawEvent(new RawKeyboardEventArgs(key, KeyStates.None, keyboardEvent.Repeat, GetTimestamp()));

            if (keyDownHandled || keyUpHandled)
            {
                e.PreventDefault();
            }
        }

        private void PreventKeyboardHandled(Event e)
        {
            if (keyDownHandled || keyUpHandled)
            {
                e.PreventDefault();
            }
        }

        private void OnMouseDown(Event e)
        {
            MouseEvent mouseEvent = (MouseEvent)e;

            Point position = new Point(mouseEvent.PageX, mouseEvent.PageY);
            MouseButton button = converter.ConvertBackMouseButton(mouseEvent.Button);

            mouseDownHandled = MouseDevice.ProcessRawEvent(new RawMouseButtonEventArgs(button, MouseButtonState.Pressed, position, GetTimestamp()));

            if (mouseDownHandled || MouseDevice.CaptureTarget != null)
            {
                e.PreventDefault();
            }
        }

        private void OnMouseUp(Event e)
        {
            MouseEvent mouseEvent = (MouseEvent)e;

            Point position = new Point(mouseEvent.PageX, mouseEvent.PageY);
            MouseButton button = converter.ConvertBackMouseButton(mouseEvent.Button);

            mouseUpHandled = MouseDevice.ProcessRawEvent(new RawMouseButtonEventArgs(button, MouseButtonState.Released, position, GetTimestamp()));

            if (mouseDownHandled || mouseMoveHandled || mouseUpHandled || MouseDevice.CaptureTarget != null)
            {
                e.PreventDefault();
            }
        }

        private void OnMouseWheel(Event e)
        {
            WheelEvent wheelEvent = (WheelEvent)e;

            Point position = new Point(wheelEvent.PageX, wheelEvent.PageY);
            int delta = wheelEvent.DeltaY > 0 ? -100 : 100;

            if (MouseDevice.ProcessRawEvent(new RawMouseWheelEventArgs(delta, position, GetTimestamp())))
            {
                e.PreventDefault();
            }
        }

        private void OnMouseMove(Event e)
        {
            if (!(e is MouseEvent))
            {
                return;
            }

            MouseEvent mouseEvent = (MouseEvent)e;

            Point position = new Point(mouseEvent.PageX, mouseEvent.PageY);

            mouseMoveHandled = MouseDevice.ProcessRawEvent(new RawMouseEventArgs(position, GetTimestamp()));

            if (mouseDownHandled || mouseMoveHandled || MouseDevice.CaptureTarget != null)
            {
                e.PreventDefault();
            }
        }

        private void PreventMouseHandled(Event e)
        {
            if (mouseDownHandled || mouseMoveHandled || mouseUpHandled || MouseDevice.CaptureTarget != null)
            {
                e.PreventDefault();
            }
        }

        public IInputElement HitTest(Point position)
        {
            return RootElement.HitTest(position) as IInputElement;
        }

        public int GetTimestamp()
        {
            return 0;//(int)(DateTime.Now.GetTime());
        }
    }
}