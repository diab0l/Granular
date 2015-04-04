using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Granular.Extensions;

namespace System.Windows.Media.Imaging
{
    public enum BitmapCacheOption
    {
        OnDemand = 0,
        Default = 0,
        OnLoad = 1,
        //None = 2,
    }

    public class BitmapImage : BitmapSource, ISupportInitialize
    {
        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.Register("UriSource", typeof(string), typeof(BitmapImage), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((BitmapImage)sender).OnUriSourceChanged(e)));
        public string UriSource
        {
            get { return (string)GetValue(UriSourceProperty); }
            set { SetValue(UriSourceProperty, value); }
        }

        public static readonly DependencyProperty CacheOptionProperty = DependencyProperty.Register("CacheOption", typeof(BitmapCacheOption), typeof(BitmapImage), new FrameworkPropertyMetadata());
        public BitmapCacheOption CacheOption
        {
            get { return (BitmapCacheOption)GetValue(CacheOptionProperty); }
            set { SetValue(CacheOptionProperty, value); }
        }

        public static readonly DependencyProperty SourceRectProperty = DependencyProperty.Register("SourceRect", typeof(Rect), typeof(BitmapImage), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((BitmapImage)sender).OnSourceRectChanged(e)));
        public Rect SourceRect
        {
            get { return (Rect)GetValue(SourceRectProperty); }
            set { SetValue(SourceRectProperty, value); }
        }

        private IRenderImageSource renderImageSource;
        public override IRenderImageSource RenderImageSource
        {
            get
            {
                if (renderImageSource == null)
                {
                    CreateRenderImageSource();
                }

                return renderImageSource;
            }
        }

        private bool isInitializing;

        public BitmapImage()
        {
            //
        }

        public BitmapImage(string uriSource, BitmapCacheOption cacheOption = BitmapCacheOption.Default)
        {
            BeginInit();
            this.UriSource = uriSource;
            this.CacheOption = cacheOption;
            EndInit();
        }

        public void BeginInit()
        {
            if (isInitializing)
            {
                throw new Granular.Exception("BitmapImage is already initializing");
            }

            isInitializing = true;
        }

        public void EndInit()
        {
            if (!isInitializing)
            {
                return;
            }

            if (CacheOption == BitmapCacheOption.OnLoad)
            {
                CreateRenderImageSource();
            }

            isInitializing = false;
        }

        private void OnUriSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (renderImageSource != null)
            {
                throw new Granular.Exception("UriSource cannot be changed after BitmapImage was initialized");
            }

            if (!isInitializing && CacheOption == BitmapCacheOption.OnLoad)
            {
                CreateRenderImageSource();
            }
        }

        private void OnSourceRectChanged(DependencyPropertyChangedEventArgs e)
        {
            if (renderImageSource != null)
            {
                throw new Granular.Exception("UriSource cannot be changed after BitmapImage was initialized");
            }
        }

        private void CreateRenderImageSource()
        {
            if (renderImageSource != null)
            {
                return;
            }

            if (!UriSource.IsNullOrEmpty())
            {
                renderImageSource = ApplicationHost.Current.RenderImageSourceFactory.CreateRenderImageSource(UriSource, SourceRect);
                renderImageSource.StateChanged += (sender, e) => SetRenderImageState(renderImageSource.State);
                SetRenderImageState(renderImageSource.State);
            }
        }
    }
}
