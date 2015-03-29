extern alias wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace Granular.Host.Wpf
{
    public class WpfApplicationHost : IApplicationHost
    {
        public IPresentationSourceFactory PresentationSourceFactory { get { return WpfPresentationSourceFactory.Default; } }

        public ITaskScheduler TaskScheduler { get { return WpfTaskScheduler.Default; } }

        public ITextMeasurementService TextMeasurementService { get { return WpfTextMeasurementService.Default; } }

        public WpfApplicationHost()
        {
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
