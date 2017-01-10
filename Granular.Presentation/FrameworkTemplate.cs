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
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
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

        public Granular.Collections.ObservableCollection<ITrigger> Triggers { get; private set; }

        public FrameworkTemplate()
        {
            Triggers = new Granular.Collections.ObservableCollection<ITrigger>();
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
            foreach (ITrigger trigger in Triggers)
            {
                trigger.Detach(element, BaseValueSource.TemplateTrigger);
            }

            element.TemplateChild = null;
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

        private int hashCode;

        public TemplateKey(Type targetType)
        {
            this.TargetType = targetType;
            this.hashCode = targetType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            TemplateKey other = obj as TemplateKey;

            return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
                Object.Equals(this.TargetType, other.TargetType);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        public override string ToString()
        {
            return String.Format("TemplateKey({0})", TargetType.Name);
        }
    }
}
