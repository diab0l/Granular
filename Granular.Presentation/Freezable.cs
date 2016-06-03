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

    public interface IInheritableObject
    {
        void SetInheritanceContext(DependencyObject inheritanceContext);
    }

    public class Freezable : DependencyObject, IResourceContainer, INotifyChanged, IInheritableObject
    {
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

                if (parentResourceContainer != null)
                {
                    parentResourceContainer.ResourcesChanged -= OnParentResourcesChanged;
                }

                parentResourceContainer = value;

                if (parentResourceContainer != null)
                {
                    parentResourceContainer.ResourcesChanged += OnParentResourcesChanged;
                }

                ResourcesChanged.Raise(this, ResourcesChangedEventArgs.Reset);
            }
        }

        private void OnParentResourcesChanged(object sender, ResourcesChangedEventArgs e)
        {
            ResourcesChanged.Raise(this, e);
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
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
        }

        protected override void OnInheritanceParentChanged(DependencyObject oldInheritanceParent, DependencyObject newInheritanceParent)
        {
            ParentResourceContainer = newInheritanceParent as IResourceContainer;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            VerifyNotFrozen(e.Property);

            if (!e.IsSubPropertyChange)
            {
                if (e.OldValue is IInheritableObject)
                {
                    ((IInheritableObject)e.OldValue).SetInheritanceContext(null);
                }

                if (e.NewValue is IInheritableObject)
                {
                    ((IInheritableObject)e.NewValue).SetInheritanceContext(this);
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

        void IInheritableObject.SetInheritanceContext(DependencyObject inheritanceContext)
        {
            base.SetInheritanceParent(inheritanceContext);
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
