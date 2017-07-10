using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Granular.Extensions;

namespace System.Windows.Media.Imaging
{
    public class BitmapImage : BitmapSource, IUriContext, ISupportInitialize
    {
        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.Register("UriSource", typeof(Uri), typeof(BitmapImage), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((BitmapImage)sender).OnUriSourceChanged(e)));
        public Uri UriSource
        {
            get { return (Uri)GetValue(UriSourceProperty); }
            set { SetValue(UriSourceProperty, value); }
        }

        public static readonly DependencyProperty SourceRectProperty = DependencyProperty.Register("SourceRect", typeof(Rect), typeof(BitmapImage), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((BitmapImage)sender).OnSourceRectChanged(e)));
        public Rect SourceRect
        {
            get { return (Rect)GetValue(SourceRectProperty); }
            set { SetValue(SourceRectProperty, value); }
        }

        public Uri BaseUri { get; set; }

        private IImageSourceRenderResource renderResource;
        private bool isInitialized;

        public void BeginInit()
        {
            //
        }

        public void EndInit()
        {
            //
        }

        protected override object CreateRenderResource(IRenderElementFactory factory)
        {
            return factory.CreateImageSourceRenderResource();
        }

        protected override void OnRenderResourceCreated(object renderResource)
        {
            base.OnRenderResourceCreated(renderResource);

            this.renderResource = (IImageSourceRenderResource)renderResource;
            InitializeRenderResource();
        }

        private void InitializeRenderResource()
        {
            if (isInitialized || renderResource == null || UriSource == null)
            {
                return;
            }

            renderResource.Initialize(UriSource.ResolveAbsoluteUri(BaseUri), SourceRect);
            isInitialized = true;
        }

        private void OnUriSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (isInitialized)
            {
                throw new Granular.Exception("UriSource cannot be changed after BitmapImage was initialized");
            }

            InitializeRenderResource();
        }

        private void OnSourceRectChanged(DependencyPropertyChangedEventArgs e)
        {
            if (isInitialized)
            {
                throw new Granular.Exception("SourceRect cannot be changed after BitmapImage was initialized");
            }

            InitializeRenderResource();
        }
    }
}
