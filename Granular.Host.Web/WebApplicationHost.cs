using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace Granular.Host
{
    public class WebApplicationHost : IApplicationHost
    {
        public IPresentationSourceFactory PresentationSourceFactory { get { return Host.PresentationSourceFactory.Default; } }

        public ITaskScheduler TaskScheduler { get { return Host.TaskScheduler.Default; } }

        public ITextMeasurementService TextMeasurementService { get { return Host.TextMeasurementService.Default; } }

        public WebApplicationHost()
        {
            //
        }

        public void Run(Action applicationEntryPoint)
        {
            System.Html.Window.OnLoad += e => applicationEntryPoint();
        }
    }
}
