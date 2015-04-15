using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace System.Windows.Controls
{
    public class Image : FrameworkElement
    {
        public static readonly RoutedEvent ImageFailedEvent = EventManager.RegisterRoutedEvent("ImageFailed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Image));
        public event RoutedEventHandler ImageFailed
        {
            add { AddHandler(ImageFailedEvent, value); }
            remove { RemoveHandler(ImageFailedEvent, value); }
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(Image), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((Image)sender).OnSourceChanged(e)));
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(Image), new FrameworkPropertyMetadata(Stretch.Uniform, affectsMeasure: true));
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public static readonly DependencyProperty StretchDirectionProperty = DependencyProperty.Register("StretchDirection", typeof(StretchDirection), typeof(Image), new FrameworkPropertyMetadata(StretchDirection.Both, affectsMeasure: true));
        public StretchDirection StretchDirection
        {
            get { return (StretchDirection)GetValue(StretchDirectionProperty); }
            set { SetValue(StretchDirectionProperty, value); }
        }

        private BitmapSource bitmapSource;
        private BitmapSource BitmapSource
        {
            get { return bitmapSource; }
            set
            {
                if (bitmapSource == value)
                {
                    return;
                }

                if (bitmapSource != null)
                {
                    bitmapSource.DownloadCompleted -= OnBitmapSourceDownloadCompleted;
                    bitmapSource.DownloadFailed -= OnBitmapSourceDownloadFailed;
                }

                bitmapSource = value;

                if (bitmapSource != null)
                {
                    bitmapSource.DownloadCompleted += OnBitmapSourceDownloadCompleted;
                    bitmapSource.DownloadFailed += OnBitmapSourceDownloadFailed;
                }
            }
        }

        private Dictionary<IRenderElementFactory, IImageRenderElement> imageRenderElements;

        public Image()
        {
            imageRenderElements = new Dictionary<IRenderElementFactory, IImageRenderElement>();
        }

        protected override object CreateContentRenderElementOverride(IRenderElementFactory factory)
        {
            IImageRenderElement imageRenderElement;
            if (imageRenderElements.TryGetValue(factory, out imageRenderElement))
            {
                return imageRenderElement;
            }

            imageRenderElement = factory.CreateImageRenderElement(this);

            if (Source != null)
            {
                imageRenderElement.Bounds = GetStretchRect(Source.Size, VisualBounds.Size, Stretch, StretchDirection);
                imageRenderElement.Source = this.Source;
            }

            imageRenderElements.Add(factory, imageRenderElement);

            return imageRenderElement;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if ((Stretch == Stretch.None || StretchDirection == StretchDirection.UpOnly) && Source != null)
            {
                return Source.Size.DefaultIfNullOrEmpty();
            }

            return Size.Zero;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect imageRenderElementBounds = Source != null ? GetStretchRect(Source.Size, finalSize, Stretch, StretchDirection) : Rect.Zero;

            foreach (IImageRenderElement imageRenderElement in imageRenderElements.Values)
            {
                imageRenderElement.Bounds = imageRenderElementBounds;
                imageRenderElement.Source = Source;
            }

            return finalSize;
        }

        private void OnSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            BitmapSource = Source as BitmapSource;

            InvalidateMeasure();
        }

        private void OnBitmapSourceDownloadCompleted(object sender, EventArgs e)
        {
            InvalidateMeasure();
        }

        private void OnBitmapSourceDownloadFailed(object sender, EventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ImageFailedEvent, this));
        }

        private static Rect GetStretchRect(Size size, Size availableSize, Stretch stretch, StretchDirection stretchDirection)
        {
            if (size.IsNullOrEmpty() || size == Size.Zero || availableSize == Size.Zero)
            {
                return Rect.Zero;
            }

            Size stretchSize = GetStretchSize(size, availableSize, stretch);

            if (stretchDirection == StretchDirection.DownOnly)
            {
                stretchSize = stretchSize.Min(size);
            }

            if (stretchDirection == StretchDirection.UpOnly)
            {
                stretchSize = stretchSize.Max(size);
            }

            return new Rect((availableSize.ToPoint() - stretchSize.ToPoint()) / 2, stretchSize);
        }

        private static Size GetStretchSize(Size imageSize, Size availableSize, Stretch stretch)
        {
            switch (stretch)
            {
                case Stretch.None: return imageSize;

                case Stretch.Fill: return availableSize;

                case Stretch.Uniform: return imageSize.Height * availableSize.Width < imageSize.Width * availableSize.Height ?
                    new Size(availableSize.Width, imageSize.Height * availableSize.Width / imageSize.Width) :
                    new Size(imageSize.Width * availableSize.Height / imageSize.Height, availableSize.Height);

                case Stretch.UniformToFill: return imageSize.Height * availableSize.Width > imageSize.Width * availableSize.Height ?
                    new Size(availableSize.Width, imageSize.Height * availableSize.Width / imageSize.Width) :
                    new Size(imageSize.Width * availableSize.Height / imageSize.Height, availableSize.Height);

                default: throw new Granular.Exception("Unexpected Stretch \"{0}\"", stretch);
            }
        }
    }
}
