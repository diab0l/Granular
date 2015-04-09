using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Granular.Extensions;
using Granular.Presentation.Tests.Media;

namespace Granular.Presentation.Tests
{
    public class TestApplicationHost : IApplicationHost
    {
        public IPresentationSourceFactory PresentationSourceFactory { get; set; }
        public ITaskScheduler TaskScheduler { get; set; }
        public ITextMeasurementService TextMeasurementService { get; set; }
        public IRenderImageSourceFactory RenderImageSourceFactory { get; set; }

        public TestApplicationHost()
        {
            TaskScheduler = new SendTaskScheduler();
            TextMeasurementService = new TestTextMeasurementService();
            RenderImageSourceFactory = new TestRenderImageSourceFactory();
        }

        public void Run(Action applicationEntryPoint)
        {
            applicationEntryPoint();
        }
    }

    public class SendTaskScheduler : ITaskScheduler
    {
        public void ScheduleTask(TimeSpan timeSpan, Action action)
        {
            if (timeSpan > TimeSpan.Zero)
            {
                throw new Granular.Exception("SendTaskScheduler supports only immediate tasks");
            }

            action();
        }
    }

    public class TestTextMeasurementService : ITextMeasurementService
    {
        public Size Measure(string text, double fontSize, Typeface typeface, double maxWidth)
        {
            return text.IsNullOrEmpty() ? Size.Zero : new Size(text.Length * 5, 20);
        }
    }

    public class TestRenderImageSourceFactory : IRenderImageSourceFactory
    {
        public IRenderImageSource CreateRenderImageSource(RenderImageType imageType, byte[] data, Rect sourceRect)
        {
            return new TestRenderImageSource();
        }

        public IRenderImageSource CreateRenderImageSource(string uri, Rect sourceRect)
        {
            return new TestRenderImageSource() { State = RenderImageState.DownloadProgress };
        }
    }

    public class TestPresentationSourceFactory : IPresentationSourceFactory
    {
        private IPresentationSource presentationSource;

        public TestPresentationSourceFactory(IPresentationSource presentationSource)
        {
            this.presentationSource = presentationSource;
        }

        public IPresentationSource CreatePresentationSource(UIElement rootElement)
        {
            return presentationSource;
        }

        public IPresentationSource GetPresentationSourceFromElement(UIElement element)
        {
            return presentationSource;
        }
    }
}
