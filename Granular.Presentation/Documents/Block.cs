using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Collections;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;

namespace System.Windows.Documents
{
    [ContentProperty("SiblingBlocks")]
    public class Block : TextElement
    {
        public static readonly DependencyProperty BorderBrushProperty = Border.BorderBrushProperty.AddOwner(typeof(Block));
        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public static readonly DependencyProperty BorderThicknessProperty = Border.BorderThicknessProperty.AddOwner(typeof(Block));
        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty LineHeightProperty = DependencyProperty.Register("LineHeight", typeof(double), typeof(Block), new FrameworkPropertyMetadata());
        public double LineHeight
        {
            get { return (double)GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        public static readonly DependencyProperty PaddingProperty = Border.PaddingProperty.AddOwner(typeof(Block));
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.RegisterAttached("TextAlignment", typeof(TextAlignment), typeof(Block), new FrameworkPropertyMetadata(TextAlignment.Left, inherits: true));
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public ObservableCollection<Block> SiblingBlocks { get; private set; }
        //public Block PreviousBlock { get; }
        //public Block NextBlock { get; }

        public Block()
        {
            //FrameworkElement.MarginProperty.OverrideMetadata(typeof(Block), ...
        }

        public override object GetRenderElement(IRenderElementFactory factory)
        {
            return null;
        }

        public override void RemoveRenderElement(IRenderElementFactory factory)
        {
            //
        }
    }
}
