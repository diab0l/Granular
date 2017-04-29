using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Granular.Host.Render;

namespace Granular.Host
{
    public class WebApplicationHost : IApplicationHost
    {
        public IPresentationSourceFactory PresentationSourceFactory { get; private set; }

        public ITaskScheduler TaskScheduler { get; private set; }

        public ITextMeasurementService TextMeasurementService { get; private set; }

        public IRenderImageSourceFactory RenderImageSourceFactory { get; private set; }

        public WebApplicationHost()
        {
            RenderQueue renderQueue = new RenderQueue();
            HtmlValueConverter htmlValueConverter = new HtmlValueConverter();

            HtmlRenderElementFactory htmlRenderElementFactory = new HtmlRenderElementFactory(renderQueue, htmlValueConverter);

            PresentationSourceFactory = new PresentationSourceFactory(htmlRenderElementFactory, htmlValueConverter);
            TaskScheduler = new TaskScheduler();
            TextMeasurementService = new TextMeasurementService(htmlValueConverter);
            RenderImageSourceFactory = new RenderImageSourceFactory(htmlValueConverter);
        }

        public void Run(Action applicationEntryPoint)
        {
            Bridge.Html5.Window.OnLoad += e => applicationEntryPoint();
        }
    }
}
