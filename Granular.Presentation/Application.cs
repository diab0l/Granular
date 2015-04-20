using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xaml;
using Granular.Extensions;

namespace System.Windows
{
    public class Application : IResourceContainer
    {
        public static Application Current { get; private set; }

        public static readonly SystemResources SystemResources = new SystemResources();

        public event EventHandler<ResourcesChangedEventArgs> ResourcesChanged;
        public event EventHandler<StartupEventArgs> Startup;
        public event EventHandler LoadCompleted;

        public string StartupUri { get; set; }

        public Window MainWindow { get; set; }

        private ResourceDictionary resources;
        public ResourceDictionary Resources
        {
            get { return resources; }
            set
            {
                if (resources == value)
                {
                    return;
                }

                if (resources != null)
                {
                    resources.ResourcesChanged -= OnResourcesChanged;
                }

                resources = value;

                if (resources != null)
                {
                    resources.ResourcesChanged += OnResourcesChanged;
                }

                ResourcesChanged.Raise(this, ResourcesChangedEventArgs.Reset);
            }
        }

        public Application()
        {
            if (Current != null)
            {
                throw new Granular.Exception("Application instance was already created");
            }

            Current = this;
        }

        public void Run()
        {
            OnStartup();
            Startup.Raise(this, StartupEventArgs.Empty);

            LoadStartupUri();

            OnLoadCompleted();
            LoadCompleted.Raise(this);
        }

        private void LoadStartupUri()
        {
            if (StartupUri.IsNullOrEmpty())
            {
                return;
            }

            XamlElement rootElement = XamlParser.Parse(Granular.Compatibility.String.FromByteArray(EmbeddedResourceLoader.LoadResourceData(StartupUri)));
            XamlMember classDirective = rootElement.Directives.FirstOrDefault(directive => directive.Name == XamlLanguage.ClassDirective);

            Window window = Activator.CreateInstance(Type.GetType(String.Format("{0}, {1}", classDirective.GetSingleValue(), GetType().Assembly.GetName().Name))) as Window;
            if (window != null)
            {
                window.Show();
            }
        }

        private void OnResourcesChanged(object sender, ResourcesChangedEventArgs e)
        {
            ResourcesChanged.Raise(this, e);
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        public bool TryGetResource(object resourceKey, out object value)
        {
            if (Resources != null && Resources.TryGetValue(resourceKey, out value))
            {
                return true;
            }

            if (SystemResources.TryGetResource(resourceKey, out value))
            {
                return true;
            }

            value = null;
            return false;
        }

        protected virtual void OnStartup()
        {
            //
        }

        protected virtual void OnLoadCompleted()
        {
            //
        }

        public static object LoadComponent(string resourceUri)
        {
            return EmbeddedResourceLoader.LoadResourceElement(resourceUri);
        }

        public static void LoadComponent(object target, string resourceUri)
        {
            XamlLoader.Load(target, XamlParser.Parse(Granular.Compatibility.String.FromByteArray(EmbeddedResourceLoader.LoadResourceData(resourceUri))));
        }
    }
}
