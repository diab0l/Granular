using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Granular.Extensions;

namespace System.Windows
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class ApplicationHostAttribute : Attribute
    {
        public Type Type { get; private set; }

        public ApplicationHostAttribute(Type type)
        {
            this.Type = type;
        }
    }

    public interface IApplicationHost
    {
        IPresentationSourceFactory PresentationSourceFactory { get; }
        ITaskScheduler TaskScheduler { get; }
        ITextMeasurementService TextMeasurementService { get; }
        IRenderImageSourceFactory RenderImageSourceFactory { get; }
        void Run(Action applicationEntryPoint);
    }

    public static class ApplicationHostExtensions
    {
        public static MouseDevice GetMouseDeviceFromElement(this IApplicationHost applicationHost, FrameworkElement element)
        {
            IPresentationSource presentationSource = applicationHost.PresentationSourceFactory.GetPresentationSourceFromElement(element);
            return presentationSource != null ? presentationSource.MouseDevice : null;
        }

        public static KeyboardDevice GetKeyboardDeviceFromElement(this IApplicationHost applicationHost, FrameworkElement element)
        {
            IPresentationSource presentationSource = applicationHost.PresentationSourceFactory.GetPresentationSourceFromElement(element);
            return presentationSource != null ? presentationSource.KeyboardDevice : null;
        }
    }

    public interface ITextMeasurementService
    {
        Size Measure(string text, double fontSize, Typeface typeface, double maxWidth);
    }

    public interface IRenderImageSourceFactory
    {
        IRenderImageSource CreateRenderImageSource(RenderImageType imageType, byte[] imageData, Rect sourceRect);
        IRenderImageSource CreateRenderImageSource(string uri, Rect sourceRect);
    }

    public static class ApplicationHost
    {
        private static IApplicationHost current;
        public static IApplicationHost Current
        {
            get
            {
                if (current == null)
                {
                    Initialize();
                }

                return current;
            }
        }

        private static void Initialize()
        {
            if (current != null)
            {
                return;
            }

            ApplicationHostAttribute[] applicationHostAttributes = Granular.Compatibility.AppDomain.GetAssemblies().SelectMany(assembly => assembly.GetCustomAttributesCached<ApplicationHostAttribute>()).ToArray();

            if (applicationHostAttributes.Length == 0)
            {
                throw new Granular.Exception("ApplicationHost assembly attribute was not found");
            }

            if (applicationHostAttributes.Length > 1)
            {
                throw new Granular.Exception("Multiple ApplicationHost assembly attributes were found, leave only one attribute or call explicitly to ApplicationHost.Initialize");
            }

            Initialize(Activator.CreateInstance(applicationHostAttributes[0].Type) as IApplicationHost);
        }

        public static void Initialize(IApplicationHost applicationHost)
        {
            if (current != null && current != applicationHost)
            {
                throw new Granular.Exception("ApplicationHost was already initialized");
            }

            current = applicationHost;
        }
    }
}