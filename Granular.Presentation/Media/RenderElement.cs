using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace System.Windows.Media
{
    public interface IContainerRenderElement
    {
        IEnumerable<object> Children { get; }

        void InsertChild(int index, object child);
        void RemoveChild(object child);
    }

    public interface IVisualRenderElement : IContainerRenderElement
    {
        Brush Background { get; set; }
        Rect Bounds { get; set; }
        bool ClipToBounds { get; set; }
        bool IsHitTestVisible { get; set; }
        bool IsVisible { get; set; }
        double Opacity { get; set; }
        Matrix Transform { get; set; }
    }

    public interface ITextBoxRenderElement
    {
        event EventHandler TextChanged;
        string Text { get; set; }
        int MaxLength { get; set; }

        event EventHandler CaretIndexChanged;
        int CaretIndex { get; set; }

        event EventHandler SelectionStartChanged;
        int SelectionLength { get; set; }

        event EventHandler SelectionLengthChanged;
        int SelectionStart { get; set; }

        Rect Bounds { get; set; }
        bool AcceptsReturn { get; set; }
        bool AcceptsTab { get; set; }
        bool IsPassword { get; set; }
        bool IsReadOnly { get; set; }
        bool SpellCheck { get; set; }

        Brush Foreground { get; set; }
        double FontSize { get; set; }
        FontFamily FontFamily { get; set; }
        FontStretch FontStretch { get; set; }
        FontStyle FontStyle { get; set; }
        FontWeight FontWeight { get; set; }
        bool IsHitTestVisible { get; set; }
        TextWrapping TextWrapping { get; set; }
        TextAlignment TextAlignment { get; set; }

        ScrollBarVisibility HorizontalScrollBarVisibility { get; set; }
        ScrollBarVisibility VerticalScrollBarVisibility { get; set; }

        void Focus();
        void ClearFocus();
        void ProcessKeyEvent(KeyEventArgs e);
    }

    public interface ITextBlockRenderElement
    {
        Rect Bounds { get; set; }
        FontFamily FontFamily { get; set; }
        Brush Foreground { get; set; }
        double FontSize { get; set; }
        FontStyle FontStyle { get; set; }
        FontStretch FontStretch { get; set; }
        FontWeight FontWeight { get; set; }
        string Text { get; set; }
        TextAlignment TextAlignment { get; set; }
        TextTrimming TextTrimming { get; set; }
        TextWrapping TextWrapping { get; set; }
    }

    public interface IBorderRenderElement
    {
        Brush Background { get; set; }
        Thickness BorderThickness { get; set; }
        Brush BorderBrush { get; set; }
        Rect Bounds { get; set; }
        CornerRadius CornerRadius { get; set; }
        bool IsHitTestVisible { get; set; }
    }

    public interface IImageRenderElement
    {
        Rect Bounds { get; set; }
        ImageSource Source { get; set; }
    }

    public interface IDrawingContainerRenderElement : IContainerRenderElement
    {
        double Opacity { get; set; }
    }

    public interface IDrawingShapeRenderElement
    {
        Brush Fill { get; set; }
        Brush Stroke { get; set; }
        double StrokeThickness { get; set; }
    }

    public interface IDrawingGeometryRenderElement : IDrawingShapeRenderElement
    {
        Geometry Geometry { get; set; }
    }

    public interface IDrawingImageRenderElement
    {
        ImageSource ImageSource { get; set; }
        Rect Rectangle { get; set; }
    }

    public interface IBrushRenderResource
    {
        double Opacity { get; set; }
    }

    public interface ISolidColorBrushRenderResource : IBrushRenderResource
    {
        Color Color { get; set; }
    }

    public class RenderGradientStop
    {
        public Color Color { get; private set; }
        public double Offset { get; private set; }

        public RenderGradientStop(Color color, double offset)
        {
            this.Color = color;
            this.Offset = offset;
        }
    }

    public interface IGradientBrushRenderResource : IBrushRenderResource
    {
        IEnumerable<RenderGradientStop> GradientStops { get; set; }
        GradientSpreadMethod SpreadMethod { get; set; }
        BrushMappingMode MappingMode { get; set; }
    }

    public interface ILinearGradientBrushRenderResource : IGradientBrushRenderResource
    {
        Point StartPoint { get; set; }
        Point EndPoint { get; set; }
    }

    public interface IRadialGradientBrushRenderResource : IGradientBrushRenderResource
    {
        Point Center { get; set; }
        Point GradientOrigin { get; set; }
        double RadiusX { get; set; }
        double RadiusY { get; set; }
    }

    public interface ITileBrushRenderResource : IBrushRenderResource
    {
        TileMode TileMode { get; set; }
        Stretch Stretch { get; set; }
        Rect Viewport { get; set; }
        BrushMappingMode ViewportUnits { get; set; }
    }

    public interface IImageBrushRenderResource : ITileBrushRenderResource
    {
        ImageSource ImageSource { get; set; }
    }

    public enum RenderImageState
    {
        Idle,
        DownloadProgress,
        DownloadCompleted,
        DownloadFailed
    }

    public interface IImageSourceRenderResource
    {
        event EventHandler StateChanged;
        RenderImageState State { get; }
        Size Size { get; }
        void Initialize(Uri uri, Rect sourceRect);
    }

    public interface ITransformRenderResource
    {
        Matrix Matrix { get; set; }
    }

    public interface IGeometryRenderResource
    {
        Transform Transform { get; set; }
        string Data { get; set; }
    }

    public interface IRenderElementFactory
    {
        IVisualRenderElement CreateVisualRenderElement(object owner);
        ITextBoxRenderElement CreateTextBoxRenderElement(object owner);
        ITextBlockRenderElement CreateTextBlockRenderElement(object owner);
        IBorderRenderElement CreateBorderRenderElement(object owner);
        IImageRenderElement CreateImageRenderElement(object owner);
        IContainerRenderElement CreateDrawingRenderElement(object owner);
        IDrawingGeometryRenderElement CreateDrawingGeometryRenderElement();
        IDrawingContainerRenderElement CreateDrawingContainerRenderElement();
        IDrawingImageRenderElement CreateDrawingImageRenderElement();
        ISolidColorBrushRenderResource CreateSolidColorBrushRenderResource();
        ILinearGradientBrushRenderResource CreateLinearGradientBrushRenderResource();
        IRadialGradientBrushRenderResource CreateRadialGradientBrushRenderResource();
        IImageBrushRenderResource CreateImageBrushRenderResource();
        IImageSourceRenderResource CreateImageSourceRenderResource();
        ITransformRenderResource CreateTransformRenderResource();
        IGeometryRenderResource CreateGeometryRenderResource();
    }

    public static class ContainerRenderElementExtensions
    {
        public static void SetChildren(this IContainerRenderElement container, IEnumerable<object> children)
        {
            if (container.Children.SequenceEqual(children))
            {
                return;
            }

            foreach (object child in container.Children.ToArray())
            {
                container.RemoveChild(child);
            }

            int index = 0;
            foreach (object child in children)
            {
                container.InsertChild(index, child);
                index++;
            }
        }
    }
}
