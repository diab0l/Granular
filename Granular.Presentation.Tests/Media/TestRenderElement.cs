using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace System.Windows.Media
{
    public class TestVisualRenderElement : IVisualRenderElement
    {
        public Brush Background { get; set; }
        public Rect Bounds { get; set; }
        public bool ClipToBounds { get; set; }
        public bool IsHitTestVisible { get; set; }
        public bool IsVisible { get; set; }
        public double Opacity { get; set; }
        public Matrix Transform { get; set; }

        private List<object> children;
        public IEnumerable<object> Children { get { return children; } }

        public TestVisualRenderElement()
        {
            children = new List<object>();
        }

        public void InsertChild(int index, object child)
        {
            children.Insert(index, child);
        }

        public void RemoveChild(object child)
        {
            children.Remove(child);
        }
    }

    public class TestTextBoxRenderElement : ITextBoxRenderElement
    {
        public event EventHandler TextChanged { add { } remove { } }
        public event EventHandler CaretIndexChanged { add { } remove { } }
        public event EventHandler SelectionStartChanged { add { } remove { } }
        public event EventHandler SelectionLengthChanged { add { } remove { } }

        public int CaretIndex { get; set; }
        public int SelectionLength { get; set; }
        public int SelectionStart { get; set; }

        public string Text { get; set; }
        public int MaxLength { get; set; }
        public Rect Bounds { get; set; }
        public bool AcceptsReturn { get; set; }
        public bool AcceptsTab { get; set; }
        public bool IsPassword { get; set; }
        public bool IsReadOnly { get; set; }
        public bool SpellCheck { get; set; }

        public Brush Foreground { get; set; }
        public double FontSize { get; set; }
        public FontFamily FontFamily { get; set; }
        public FontStretch FontStretch { get; set; }
        public FontStyle FontStyle { get; set; }
        public FontWeight FontWeight { get; set; }
        public bool IsHitTestVisible { get; set; }
        public TextWrapping TextWrapping { get; set; }
        public TextAlignment TextAlignment { get; set; }

        public ScrollBarVisibility HorizontalScrollBarVisibility { get; set; }
        public ScrollBarVisibility VerticalScrollBarVisibility { get; set; }

        public Rect GetCharacterRect(int index)
        {
            return Rect.Empty;
        }

        public void Focus()
        {
            //
        }

        public void ClearFocus()
        {
            //
        }

        public void ProcessKeyEvent(KeyEventArgs e)
        {
            //
        }
    }

    public class TestTextBlockRenderElement : ITextBlockRenderElement
    {
        public Rect Bounds { get; set; }
        public FontFamily FontFamily { get; set; }
        public Brush Foreground { get; set; }
        public double FontSize { get; set; }
        public FontStyle FontStyle { get; set; }
        public FontStretch FontStretch { get; set; }
        public FontWeight FontWeight { get; set; }
        public string Text { get; set; }
        public TextAlignment TextAlignment { get; set; }
        public TextTrimming TextTrimming { get; set; }
        public TextWrapping TextWrapping { get; set; }

        public Size Measure(double maxWidth)
        {
            throw new NotImplementedException();
        }
    }

    public class TestBorderRenderElement : IBorderRenderElement
    {
        public Brush Background { get; set; }
        public Thickness BorderThickness { get; set; }
        public Brush BorderBrush { get; set; }
        public Rect Bounds { get; set; }
        public CornerRadius CornerRadius { get; set; }
        public bool IsHitTestVisible { get; set; }
    }

    public class TestImageRenderElement : IImageRenderElement
    {
        public Rect Bounds { get; set; }
        public ImageSource Source { get; set; }
    }

    public class TestContainerRenderElement : IContainerRenderElement
    {
        private List<object> children;
        public IEnumerable<object> Children { get { return children; } }

        public TestContainerRenderElement()
        {
            children = new List<object>();
        }

        public void InsertChild(int index, object child)
        {
            children.Insert(index, child);
        }

        public void RemoveChild(object child)
        {
            children.Remove(child);
        }
    }

    public class TestImageSourceRenderResource : IImageSourceRenderResource
    {
        public event EventHandler StateChanged { add { } remove { } }

        public RenderImageState State { get; set; }

        public Size Size { get; set; }

        public void Initialize(Uri uri, Rect sourceRect)
        {
            //
        }
    }

    public class TestRenderElementFactory : IRenderElementFactory
    {
        public static readonly TestRenderElementFactory Default = new TestRenderElementFactory();

        private TestRenderElementFactory()
        {
            //
        }

        public IVisualRenderElement CreateVisualRenderElement(object owner)
        {
            return new TestVisualRenderElement();
        }

        public ITextBoxRenderElement CreateTextBoxRenderElement(object owner)
        {
            return new TestTextBoxRenderElement();
        }

        public ITextBlockRenderElement CreateTextBlockRenderElement(object owner)
        {
            return new TestTextBlockRenderElement();
        }

        public IBorderRenderElement CreateBorderRenderElement(object owner)
        {
            return new TestBorderRenderElement();
        }

        public IImageRenderElement CreateImageRenderElement(object owner)
        {
            return new TestImageRenderElement();
        }

        public IContainerRenderElement CreateDrawingRenderElement(object owner)
        {
            return new TestContainerRenderElement();
        }

        public IDrawingContainerRenderElement CreateDrawingContainerRenderElement()
        {
            throw new NotImplementedException();
        }

        public ISolidColorBrushRenderResource CreateSolidColorBrushRenderResource()
        {
            throw new NotImplementedException();
        }

        public ILinearGradientBrushRenderResource CreateLinearGradientBrushRenderResource()
        {
            throw new NotImplementedException();
        }

        public IRadialGradientBrushRenderResource CreateRadialGradientBrushRenderResource()
        {
            throw new NotImplementedException();
        }

        public IDrawingGeometryRenderElement CreateDrawingGeometryRenderElement()
        {
            throw new NotImplementedException();
        }

        public IDrawingImageRenderElement CreateDrawingImageRenderElement()
        {
            throw new NotImplementedException();
        }

        public IDrawingTextRenderElement CreateDrawingTextRenderElement()
        {
            throw new NotImplementedException();
        }

        public IImageBrushRenderResource CreateImageBrushRenderResource()
        {
            throw new NotImplementedException();
        }

        public IImageSourceRenderResource CreateImageSourceRenderResource()
        {
            return new TestImageSourceRenderResource();
        }

        public ITransformRenderResource CreateTransformRenderResource()
        {
            throw new NotImplementedException();
        }

        public IGeometryRenderResource CreateGeometryRenderResource()
        {
            throw new NotImplementedException();
        }
    }
}
