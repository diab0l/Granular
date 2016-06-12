using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace System.Windows.Markup
{
    public interface INameScope : IEnumerable<KeyValuePair<string, object>>
    {
        void RegisterName(string name, object value);
        void UnregisterName(string name);
        object FindName(string name);
    }

    public class NameScope : INameScope
    {
        public static readonly DependencyProperty NameScopeProperty = DependencyProperty.RegisterAttached("NameScope", typeof(INameScope), typeof(NameScope), new FrameworkPropertyMetadata());
        public static INameScope GetNameScope(DependencyObject obj)
        {
            return (INameScope)obj.GetValue(NameScopeProperty);
        }

        public static void SetNameScope(DependencyObject obj, INameScope value)
        {
            obj.SetValue(NameScopeProperty, value);
        }

        private Dictionary<string, object> items;
        private INameScope parent;

        public NameScope(INameScope parent = null)
        {
            this.parent = parent;
            items = new Dictionary<string, object>();
        }

        public void RegisterName(string name, object value)
        {
            if (items.ContainsKey(name))
            {
                throw new Granular.Exception("Scope already contains an item named \"{0}\"", name);
            }

            items.Add(name, value);
        }

        public void UnregisterName(string name)
        {
            if (!items.ContainsKey(name))
            {
                throw new Granular.Exception("Scope doesn't contain an item named \"{0}\"", name);
            }

            items.Remove(name);
        }

        public object FindName(string name)
        {
            return items.ContainsKey(name) ? items[name] : (parent != null ? parent.FindName(name) : null);
        }

        public static INameScope GetTemplateNameScope(FrameworkElement templatedParent)
        {
            Visual templateRoot = templatedParent.VisualChildren.FirstOrDefault();
            return templateRoot != null ? GetNameScope(templateRoot) : null;
        }

        public static INameScope GetContainingNameScope(DependencyObject element)
        {
            while (element != null)
            {
                INameScope nameScope = GetNameScope(element);
                if (nameScope != null)
                {
                    return nameScope;
                }

                if (element is UIElement)
                {
                    element = ((UIElement)element).LogicalParent;
                }
                else if (element is IContextElement)
                {
                    element = ((IContextElement)element).ContextParent as DependencyObject;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return parent != null ? parent.Concat(items).GetEnumerator() : items.GetEnumerator();
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
