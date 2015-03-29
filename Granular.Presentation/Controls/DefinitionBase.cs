using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Controls
{
    internal interface IDefinitionBase
    {
        double ActualLength { get; set; }
        double MinLength { get; }
        double MaxLength { get; }
        GridLength Length { get; }
    }

    public class ColumnDefinition : Freezable, IDefinitionBase
    {
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(GridLength), typeof(ColumnDefinition), new FrameworkPropertyMetadata(GridLength.Star));
        public GridLength Width
        {
            get { return (GridLength)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public static readonly DependencyProperty MinWidthProperty = DependencyProperty.Register("MinWidth", typeof(double), typeof(ColumnDefinition), new FrameworkPropertyMetadata(0.0));
        public double MinWidth
        {
            get { return (double)GetValue(MinWidthProperty); }
            set { SetValue(MinWidthProperty, value); }
        }

        public static readonly DependencyProperty MaxWidthProperty = DependencyProperty.Register("MaxWidth", typeof(double), typeof(ColumnDefinition), new FrameworkPropertyMetadata(Double.PositiveInfinity));
        public double MaxWidth
        {
            get { return (double)GetValue(MaxWidthProperty); }
            set { SetValue(MaxWidthProperty, value); }
        }

        private static readonly DependencyPropertyKey ActualWidthPropertyKey = DependencyProperty.RegisterReadOnly("ActualWidth", typeof(double), typeof(ColumnDefinition), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty ActualWidthProperty = ActualWidthPropertyKey.DependencyProperty;
        public double ActualWidth
        {
            get { return (double)GetValue(ActualWidthPropertyKey); }
            private set { SetValue(ActualWidthPropertyKey, value); }
        }

        double IDefinitionBase.ActualLength
        {
            get { return ActualWidth; }
            set { ActualWidth = value; }
        }

        double IDefinitionBase.MinLength { get { return MinWidth; } }
        double IDefinitionBase.MaxLength { get { return MaxWidth; } }
        GridLength IDefinitionBase.Length { get { return Width; } }
    }

    public class RowDefinition : Freezable, IDefinitionBase
    {
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(GridLength), typeof(RowDefinition), new FrameworkPropertyMetadata(GridLength.Star));
        public GridLength Height
        {
            get { return (GridLength)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public static readonly DependencyProperty MinHeightProperty = DependencyProperty.Register("MinHeight", typeof(double), typeof(RowDefinition), new FrameworkPropertyMetadata(0.0));
        public double MinHeight
        {
            get { return (double)GetValue(MinHeightProperty); }
            set { SetValue(MinHeightProperty, value); }
        }

        public static readonly DependencyProperty MaxHeightProperty = DependencyProperty.Register("MaxHeight", typeof(double), typeof(RowDefinition), new FrameworkPropertyMetadata(Double.PositiveInfinity));
        public double MaxHeight
        {
            get { return (double)GetValue(MaxHeightProperty); }
            set { SetValue(MaxHeightProperty, value); }
        }

        private static readonly DependencyPropertyKey ActualHeightPropertyKey = DependencyProperty.RegisterReadOnly("ActualHeight", typeof(double), typeof(RowDefinition), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty ActualHeightProperty = ActualHeightPropertyKey.DependencyProperty;
        public double ActualHeight
        {
            get { return (double)GetValue(ActualHeightPropertyKey); }
            private set { SetValue(ActualHeightPropertyKey, value); }
        }

        double IDefinitionBase.ActualLength
        {
            get { return ActualHeight; }
            set { ActualHeight = value; }
        }

        double IDefinitionBase.MinLength { get { return MinHeight; } }
        double IDefinitionBase.MaxLength { get { return MaxHeight; } }
        GridLength IDefinitionBase.Length { get { return Height; } }
    }
}
