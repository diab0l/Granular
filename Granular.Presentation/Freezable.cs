using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace System.Windows
{
    public interface INotifyChanged
    {
        event EventHandler Changed;
    }

    public interface IContextElement
    {
        event EventHandler ContextParentChanged;
        IContextElement ContextParent { get; }

        bool TrySetContextParent(IContextElement ContextParent);
    }

    public class Freezable : DependencyObject, IResourceContainer, INotifyChanged, IContextElement
    {
        public event EventHandler ContextParentChanged;
        private IContextElement contextParent;
        public IContextElement ContextParent
        {
            get { return contextParent; }
            private set
            {
                if (contextParent == value)
                {
                    return;
                }

                if (contextParent != null)
                {
                    contextParent.ContextParentChanged -= OnContextParentAncestorChanged;
                }

                contextParent = value;

                if (contextParent != null)
                {
                    contextParent.ContextParentChanged += OnContextParentAncestorChanged;
                }

                ContextParentChanged.Raise(this);
            }
        }

        private EventHandler changed;
        public event EventHandler Changed
        {
            add
            {
                if (IsFrozen)
                {
                    return;
                }

                changed += value;
            }
            remove
            {
                if (IsFrozen)
                {
                    return;
                }

                changed -= value;
            }
        }

        public event EventHandler<ResourcesChangedEventArgs> ResourcesChanged;

        public bool IsFrozen { get; private set; }

        private IResourceContainer parentResourceContainer;
        private IResourceContainer ParentResourceContainer
        {
            get { return parentResourceContainer; }
            set
            {
                if (parentResourceContainer == value)
                {
                    return;
                }

                IResourceContainer oldParentResourceContainer = parentResourceContainer;

                if (parentResourceContainer != null)
                {
                    parentResourceContainer.ResourcesChanged -= OnParentResourcesChanged;
                }

                parentResourceContainer = value;

                if (parentResourceContainer != null)
                {
                    parentResourceContainer.ResourcesChanged += OnParentResourcesChanged;
                }

                if (oldParentResourceContainer != null && !oldParentResourceContainer.IsEmpty || parentResourceContainer != null && !parentResourceContainer.IsEmpty)
                {
                    ResourcesChanged.Raise(this, ResourcesChangedEventArgs.Reset);
                }
            }
        }

        bool IResourceContainer.IsEmpty { get { return ParentResourceContainer == null || ParentResourceContainer.IsEmpty; } }

        private void OnParentResourcesChanged(object sender, ResourcesChangedEventArgs e)
        {
            ResourcesChanged.Raise(this, e);
        }

        public bool TryGetResource(object resourceKey, out object value)
        {
            if (ParentResourceContainer != null)
            {
                return ParentResourceContainer.TryGetResource(resourceKey, out value);
            }

            value = null;
            return false;
        }

        public void Freeze()
        {
            IsFrozen = true;
            changed = null;
            ParentResourceContainer = null;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            VerifyNotFrozen(e.Property);

            if (!e.IsSubPropertyChange)
            {
                if (e.OldValue is IContextElement)
                {
                    ((IContextElement)e.OldValue).TrySetContextParent(null);
                }

                if (e.NewValue is IContextElement)
                {
                    ((IContextElement)e.NewValue).TrySetContextParent(this);
                }
            }

            RaiseChanged();
        }

        private void OnSubPropertyChanged(object sender, EventArgs e)
        {
            VerifyNotFrozen();

            RaiseChanged();
        }

        protected void RaiseChanged()
        {
            changed.Raise(this);
        }

        public bool TrySetContextParent(IContextElement contextParent)
        {
            if (IsFrozen)
            {
                return false;
            }

            this.ContextParent = contextParent;
            ParentResourceContainer = contextParent as IResourceContainer;
            return true;
        }

        private void OnContextParentAncestorChanged(object sender, EventArgs e)
        {
            ContextParentChanged.Raise(this);
        }

        private void VerifyNotFrozen()
        {
            if (IsFrozen)
            {
                throw new Granular.Exception("\"{0}\" is frozen and cannot be changed", this);
            }
        }

        private void VerifyNotFrozen(DependencyProperty changedProperty)
        {
            if (IsFrozen)
            {
                throw new Granular.Exception("\"{0}\" is frozen, property \"{1}\" cannot be changed", this, changedProperty);
            }
        }
    }
}
