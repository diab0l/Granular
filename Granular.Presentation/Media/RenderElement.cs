using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace System.Windows.Media
{
    public interface IVisualRenderElement
    {
        Brush Background { get; set; }
        Rect Bounds { get; set; }
        bool ClipToBounds { get; set; }
        bool IsHitTestVisible { get; set; }
        bool IsVisible { get; set; }
        double Opacity { get; set; }
        Matrix Transform { get; set; }

        IEnumerable<object> Children { get; }
        void InsertChild(int index, object child);
        void RemoveChild(object child);
    }

    public interface IDrawingRenderElement
    {
        void DrawLine(Point from, Point to);
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

    public interface IRenderElementFactory
    {
        IVisualRenderElement CreateVisualRenderElement(object owner);
        IDrawingRenderElement CreateDrawingRenderElement(object owner);
        ITextBoxRenderElement CreateTextBoxRenderElement(object owner);
        ITextBlockRenderElement CreateTextBlockRenderElement(object owner);
        IBorderRenderElement CreateBorderRenderElement(object owner);
        IImageRenderElement CreateImageRenderElement(object owner);
    }
}
