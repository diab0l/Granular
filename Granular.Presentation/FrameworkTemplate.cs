using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Markup;

namespace System.Windows
{
    public interface IFrameworkTemplate
    {
        void Attach(FrameworkElement element);
        void Detach(FrameworkElement element);
    }

    [ContentProperty("FrameworkElementFactory")]
    public class FrameworkTemplate : DependencyObject, IFrameworkTemplate
    {
        public static readonly IFrameworkTemplate Empty = new EmptyFrameworkTemplate();

        private class EmptyFrameworkTemplate : IFrameworkTemplate
        {
            public void Attach(FrameworkElement element)
            {
                element.TemplateChild = null;
            }

            public void Detach(FrameworkElement element)
            {
                //
            }
        }

        public IFrameworkElementFactory FrameworkElementFactory { get; set; }

        public List<ITrigger> Triggers { get; private set; }

        public FrameworkTemplate()
        {
            Triggers = new List<ITrigger>();
        }

        public void Attach(FrameworkElement element)
        {
            element.TemplateChild = CreateVisualTree(element);

            foreach (ITrigger trigger in Triggers)
            {
                trigger.Attach(element, BaseValueSource.TemplateTrigger);
            }
        }

        public void Detach(FrameworkElement element)
        {
            element.TemplateChild = null;

            foreach (ITrigger trigger in Triggers)
            {
                trigger.Detach(element, BaseValueSource.TemplateTrigger);
            }
        }

        private FrameworkElement CreateVisualTree(FrameworkElement templatedParent)
        {
            if (FrameworkElementFactory == null)
            {
                throw new Granular.Exception("FrameworkTemplate is not initialized");
            }

            return FrameworkElementFactory.CreateElement(templatedParent);
        }
    }

    public static class FrameworkTemplateExtensions
    {
        public static object FindName(this FrameworkTemplate frameworkTemplate, string name, FrameworkElement templatedParent)
        {
            return NameScope.GetNameScope(templatedParent.TemplateChild).FindName(name);
        }
    }

    public sealed class TemplateKey : IResourceKey
    {
        public Assembly Assembly { get { return TargetType != null ? TargetType.Assembly : null; } }

        public Type TargetType { get; private set; }

        public TemplateKey(Type targetType)
        {
            this.TargetType = targetType;
        }

        public override bool Equals(object obj)
        {
            TemplateKey other = obj as TemplateKey;

            return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
                Object.Equals(this.TargetType, other.TargetType);
        }

        public override int GetHashCode()
        {
            return TargetType.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("TemplateKey({0})", TargetType.Name);
        }
    }
}
