using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Granular.Extensions;
using System.Reflection;

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

            Type type = GetApplicationHostTypeByAttribute() ?? GetApplicationHostTypeByReference();

            if (type == null)
            {
                throw new Granular.Exception("Can't find an explicit ApplicationHost assembly attribute or an implicit IApplicationHost implementation in the loaded assemblies");
            }

            Initialize((IApplicationHost)Activator.CreateInstance(type));
        }

        private static Type GetApplicationHostTypeByAttribute()
        {
            ApplicationHostAttribute[] applicationHostAttributes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetCustomAttributesCached<ApplicationHostAttribute>()).ToArray();

            if (applicationHostAttributes.Length > 1)
            {
                throw new Granular.Exception("Multiple ApplicationHost assembly attributes were found, leave only one attribute or call ApplicationHost.Initialize() at the application entry point.");
            }

            return applicationHostAttributes.FirstOrDefault()?.Type;
        }

        private static Type GetApplicationHostTypeByReference()
        {
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsClass && typeof(IApplicationHost).IsAssignableFrom(type)).ToArray();

            if (types.Count() > 1)
            {
                throw new Granular.Exception("Multiple types that are implementing IApplicationHost are loaded, use ApplicationHost assembly attribute or call ApplicationHost.Initialize() at the application entry point. Types found: {0}", String.Join(", ", types.Select(type => type.FullName)));
            }

            return types.FirstOrDefault();
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