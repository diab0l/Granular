using System;
using System.Collections.Generic;
using Bridge.Html5;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Granular.Host.Render;
using System.Windows.Media;

namespace Granular.Host
{
    public class PresentationSourceFactory : IPresentationSourceFactory
    {
        private List<PresentationSource> presentationSources;
        private HtmlRenderElementFactory htmlRenderElementFactory;
        private HtmlValueConverter htmlValueConverter;
        private SvgDefinitionContainer svgDefinitionContainer;

        public PresentationSourceFactory(HtmlRenderElementFactory htmlRenderElementFactory, HtmlValueConverter htmlValueConverter, SvgDefinitionContainer svgDefinitionContainer)
        {
            this.htmlRenderElementFactory = htmlRenderElementFactory;
            this.htmlValueConverter = htmlValueConverter;
            this.svgDefinitionContainer = svgDefinitionContainer;

            presentationSources = new List<PresentationSource>();
        }

        public IPresentationSource CreatePresentationSource(UIElement rootElement)
        {
            PresentationSource presentationSource = new PresentationSource(rootElement, htmlRenderElementFactory, htmlValueConverter, svgDefinitionContainer);
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
            get { return Bridge.Html5.Window.Document.Title; }
            set { Bridge.Html5.Window.Document.Title = value; }
        }

        private HtmlValueConverter converter;

        private Bridge.Html5.WindowInstance window;

        private bool mouseDownHandled;
        private bool mouseMoveHandled;
        private bool mouseUpHandled;

        private bool keyDownHandled;
        private bool keyUpHandled;

        public PresentationSource(UIElement rootElement, HtmlRenderElementFactory htmlRenderElementFactory, HtmlValueConverter converter, SvgDefinitionContainer svgDefinitionContainer)
        {
            this.RootElement = rootElement;
            this.converter = converter;

            RootElement.IsRootElement = true;

            MouseDevice = new MouseDevice(this);
            KeyboardDevice = new KeyboardDevice(this);

            window = Bridge.Html5.Window.Instance;

            MouseDevice.CursorChanged += (sender, e) => Bridge.Html5.Window.Document.Body.SetHtmlStyleProperty("cursor", converter.ToCursorString(MouseDevice.Cursor));
            Bridge.Html5.Window.Document.Body.SetHtmlStyleProperty("cursor", converter.ToCursorString(MouseDevice.Cursor));

            Bridge.Html5.Window.OnKeyDown = OnKeyDown;
            Bridge.Html5.Window.OnKeyUp = OnKeyUp;
            Bridge.Html5.Window.OnKeyPress = PreventKeyboardHandled;
            Bridge.Html5.Window.OnMouseMove = OnMouseMove;
            Bridge.Html5.Window.OnMouseDown = OnMouseDown;
            Bridge.Html5.Window.OnMouseUp = OnMouseUp;
            Bridge.Html5.Window.OnScroll = OnMouseWheel;
            Bridge.Html5.Window.OnFocus = e => MouseDevice.Activate();
            Bridge.Html5.Window.OnBlur = e => MouseDevice.Deactivate();
            Bridge.Html5.Window.OnResize = e => SetRootElementSize();
            Bridge.Html5.Window.OnClick = PreventMouseHandled;
            Bridge.Html5.Window.OnContextMenu = PreventMouseHandled;
            Bridge.Html5.Window.AddEventListener("ondblclick", PreventMouseHandled);
            Bridge.Html5.Window.AddEventListener("wheel", OnMouseWheel);

            SetRootElementSize();
            ((FrameworkElement)RootElement).Arrange(new Rect(window.InnerWidth, window.InnerHeight));

            IHtmlRenderElement renderElement = ((IHtmlRenderElement)RootElement.GetRenderElement(htmlRenderElementFactory));
            renderElement.Load();

            Bridge.Html5.Window.Document.Body.Style.Overflow = Overflow.Hidden;
            Bridge.Html5.Window.Document.Body.AppendChild(svgDefinitionContainer.HtmlElement);
            Bridge.Html5.Window.Document.Body.AppendChild(renderElement.HtmlElement);

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

            Key key = converter.ConvertBackKey(keyboardEvent.KeyCode, (int)keyboardEvent.Location);

            keyDownHandled = ProcessKeyboardEvent(new RawKeyboardEventArgs(key, KeyStates.Down, keyboardEvent.Repeat, GetTimestamp()));

            if (keyDownHandled)
            {
                e.PreventDefault();
            }
        }

        private void OnKeyUp(Event e)
        {
            KeyboardEvent keyboardEvent = (KeyboardEvent)e;

            Key key = converter.ConvertBackKey(keyboardEvent.KeyCode, (int)keyboardEvent.Location);

            keyUpHandled = ProcessKeyboardEvent(new RawKeyboardEventArgs(key, KeyStates.None, keyboardEvent.Repeat, GetTimestamp()));

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

            mouseDownHandled = ProcessMouseEvent(new RawMouseButtonEventArgs(button, MouseButtonState.Pressed, position, GetTimestamp()));

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

            mouseUpHandled = ProcessMouseEvent(new RawMouseButtonEventArgs(button, MouseButtonState.Released, position, GetTimestamp()));

            if (mouseDownHandled || mouseMoveHandled || mouseUpHandled || MouseDevice.CaptureTarget != null)
            {
                e.PreventDefault();
            }
        }

        private void OnMouseWheel(Event e)
        {
            UIEvent uiEvent = (UIEvent)e;
            WheelEvent wheelEvent = (WheelEvent)e;

            Point position = new Point(uiEvent.PageX, uiEvent.PageY);
            int delta = (wheelEvent).DeltaY > 0 ? -100 : 100;

            if (ProcessMouseEvent(new RawMouseWheelEventArgs(delta, position, GetTimestamp())))
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

            mouseMoveHandled = ProcessMouseEvent(new RawMouseEventArgs(position, GetTimestamp()));

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

        private bool ProcessKeyboardEvent(RawKeyboardEventArgs keyboardEventArgs)
        {
            return Dispatcher.CurrentDispatcher.Invoke(() => KeyboardDevice.ProcessRawEvent(keyboardEventArgs), DispatcherPriority.Input);
        }

        private bool ProcessMouseEvent(RawMouseEventArgs mouseEventArgs)
        {
            return Dispatcher.CurrentDispatcher.Invoke(() => MouseDevice.ProcessRawEvent(mouseEventArgs), DispatcherPriority.Input);
        }
    }
}