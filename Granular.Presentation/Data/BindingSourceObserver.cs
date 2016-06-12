using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace System.Windows.Data
{
    public abstract class ContextSourceObserver : IObservableValue, IDisposable
    {
        public event ObservableValueChangedEventHandler ValueChanged;
        public object Value { get { return observableValue.Value; } }

        private DependencyObject target;

        private ObservableValue observableValue;

        public ContextSourceObserver(DependencyObject target, object baseValue)
        {
            this.target = target;

            observableValue = new ObservableValue(baseValue);
            observableValue.ValueChanged += (sender, oldValue, newValue) => ValueChanged.Raise(this, oldValue, newValue);

            if (target is IContextElement)
            {
                ((IContextElement)target).ContextParentChanged += OnTargetContextParentChanged;
            }
        }

        public void Dispose()
        {
            if (target is IContextElement)
            {
                ((IContextElement)target).ContextParentChanged -= OnTargetContextParentChanged;
            }
        }

        protected abstract object GetBaseValue();

        private void OnTargetContextParentChanged(object sender, EventArgs e)
        {
            observableValue.BaseValue = GetBaseValue();
        }
    }

    public class ScopeElementSourceObserver : ContextSourceObserver
    {
        private DependencyObject target;
        private string elementName;

        public ScopeElementSourceObserver(DependencyObject target, string elementName) :
            base(target, GetScopeElement(target, elementName))
        {
            this.target = target;
            this.elementName = elementName;
        }

        protected override object GetBaseValue()
        {
            return GetScopeElement(target, elementName);
        }

        private static object GetScopeElement(DependencyObject target, string elementName)
        {
            INameScope nameScope = NameScope.GetContainingNameScope(target);
            return nameScope != null ? nameScope.FindName(elementName) : ObservableValue.UnsetValue;
        }
    }

    public class FindAncestorSourceObserver : ContextSourceObserver
    {
        private DependencyObject target;

        private Type ancestorType;
        private int ancestorLevel;

        public FindAncestorSourceObserver(DependencyObject target, Type ancestorType, int ancestorLevel) :
            base(target, GetAncestor(target, ancestorType, ancestorLevel))
        {
            this.target = target;
            this.ancestorType = ancestorType;
            this.ancestorLevel = ancestorLevel;
        }

        protected override object GetBaseValue()
        {
            return GetAncestor(target, ancestorType, ancestorLevel);
        }

        public static object GetAncestor(DependencyObject target, Type ancestorType, int ancestorLevel)
        {
            if (!(target is IContextElement))
            {
                return ObservableValue.UnsetValue;
            }

            IContextElement contextElement = ((IContextElement)target).ContextParent;
            int level = ancestorLevel - 1;

            while (contextElement != null && (level > 0 || ancestorType != null && !ancestorType.IsInstanceOfType(contextElement)))
            {
                if (ancestorType == null || ancestorType.IsInstanceOfType(contextElement))
                {
                    level--;
                }

                contextElement = contextElement.ContextParent;
            }

            return (object)contextElement ?? ObservableValue.UnsetValue;
        }
    }

    public class TemplatedParentSourceObserver : ContextSourceObserver
    {
        private DependencyObject target;

        public TemplatedParentSourceObserver(DependencyObject target) :
            base(target, GetTemplatedParent(target))
        {
            this.target = target;
        }

        protected override object GetBaseValue()
        {
            return GetTemplatedParent(target);
        }

        private static object GetTemplatedParent(object target)
        {
            while (!(target is FrameworkElement) && target is IContextElement)
            {
                target = ((IContextElement)target).ContextParent;
            }

            return target is FrameworkElement ? (object)((FrameworkElement)target).TemplatedParent : ObservableValue.UnsetValue;
        }
    }

    public class DataContextSourceObserver : IObservableValue, IDisposable
    {
        public event ObservableValueChangedEventHandler ValueChanged;
        public object Value { get { return dataContextValue.Value; } }

        private DependencyObject target;

        private ObservableValue frameworkElementValue;
        private DependencyPropertyObserver dataContextValue;

        public DataContextSourceObserver(DependencyObject target)
        {
            this.target = target;

            frameworkElementValue = new ObservableValue(GetFrameworkElementAncestor(target));
            frameworkElementValue.ValueChanged += (sender, oldValue, newValue) => dataContextValue.SetBaseValue(newValue);

            dataContextValue = new DependencyPropertyObserver(FrameworkElement.DataContextProperty);
            dataContextValue.SetBaseValue(frameworkElementValue.Value);
            dataContextValue.ValueChanged += (sender, oldValue, newValue) => ValueChanged.Raise(this, oldValue, newValue);

            if (target is IContextElement)
            {
                ((IContextElement)target).ContextParentChanged += OnTargetContextParentChanged;
            }
        }

        public void Dispose()
        {
            if (target is IContextElement)
            {
                ((IContextElement)target).ContextParentChanged -= OnTargetContextParentChanged;
            }

            dataContextValue.Dispose();
        }

        private void OnTargetContextParentChanged(object sender, EventArgs e)
        {
            frameworkElementValue.BaseValue = GetFrameworkElementAncestor(target);
        }

        private static FrameworkElement GetFrameworkElementAncestor(object target)
        {
            while (!(target is FrameworkElement) && target is IContextElement)
            {
                target = ((IContextElement)target).ContextParent;
            }

            return target as FrameworkElement;
        }
    }
}