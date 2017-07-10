extern alias wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Granular.Host.Wpf.Render;

namespace Granular.Host.Wpf
{
    public class WpfApplicationHost : IApplicationHost
    {
        public IPresentationSourceFactory PresentationSourceFactory { get; private set; }

        public ITaskScheduler TaskScheduler { get; private set; }

        public ITextMeasurementService TextMeasurementService { get; private set; }

        public WpfApplicationHost()
        {
            WpfValueConverter wpfValueConverter = new WpfValueConverter();
            WpfRenderElementFactory wpfRenderElementFactory = new WpfRenderElementFactory(wpfValueConverter);

            PresentationSourceFactory = new WpfPresentationSourceFactory(wpfRenderElementFactory, wpfValueConverter);
            TaskScheduler = new WpfTaskScheduler();
            TextMeasurementService = new WpfTextMeasurementService(wpfValueConverter);

            wpf::System.Windows.Application application = new wpf::System.Windows.Application();
        }

        public void Run(Action applicationEntryPoint)
        {
            applicationEntryPoint();
            wpf::System.Windows.Application.Current.MainWindow.Closed += (sender, e) => wpf::System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown();
            wpf::System.Windows.Threading.Dispatcher.Run();
        }
    }
}
