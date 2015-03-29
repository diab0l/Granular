using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Granular.Extensions;

namespace System.Windows
{
    public enum BaseValueSource
    {
        Unknown = 0,
        Default = 1,
        Inherited = 2,
        ThemeStyle = 3,
        ThemeStyleTrigger = 4,
        Style = 5,
        TemplateTrigger = 6,
        StyleTrigger = 7,
        ImplicitReference = 8,
        ParentTemplate = 9,
        ParentTemplateTrigger = 10,
        Local = 11,
    }

    public class ValueSource
    {
        public BaseValueSource BaseValueSource { get; private set; }
        public bool IsAnimated { get; private set; }
        public bool IsCoerced { get; private set; }
        public bool IsExpression { get; private set; }

        public ValueSource(BaseValueSource baseValueSource, bool isAnimated, bool isCoerced, bool isExpression)
        {
            this.BaseValueSource = baseValueSource;
            this.IsAnimated = isAnimated;
            this.IsCoerced  = isCoerced;
            this.IsExpression = isExpression;
        }
    }

    public class DependencyObject
    {
        public event EventHandler<DependencyPropertyChangedEventArgs> PropertyChanged;

        private Dictionary<DependencyProperty, IDependencyPropertyValueEntry> entries;
        private Dictionary<DependencyProperty, IDependencyPropertyValueEntry> readOnlyEntries;

        private DependencyObject inheritanceParent;

        public DependencyObject()
        {
            this.entries = new Dictionary<DependencyProperty, IDependencyPropertyValueEntry>();
            this.readOnlyEntries = new Dictionary<DependencyProperty, IDependencyPropertyValueEntry>();
        }

        public bool ContainsValue(DependencyProperty dependencyProperty)
        {
            IDependencyPropertyValueEntry entry;
            if (!entries.TryGetValue(dependencyProperty, out entry))
            {
                return false;
            }

            return entry.GetBaseValuePriority() > (int)BaseValueSource.Inherited;
        }

        public bool ContainsValue(DependencyPropertyKey dependencyPropertyKey)
        {
            return ContainsValue(dependencyPropertyKey.DependencyProperty);
        }

        public object GetValue(DependencyProperty dependencyProperty)
        {
            IDependencyPropertyValueEntry entry;
            if (entries.TryGetValue(dependencyProperty, out entry))
            {
                return entry.Value;
            }

            if (dependencyProperty.Inherits && inheritanceParent != null)
            {
                // create an entry to cache the inherited value
                entry = GetInitializedValueEntry(dependencyProperty);
                return entry.Value;
            }

            return dependencyProperty.GetMetadata(GetType()).DefaultValue;
        }

        public object GetValue(DependencyPropertyKey dependencyPropertyKey)
        {
            return GetValue(dependencyPropertyKey.DependencyProperty);
        }

        public void SetValue(DependencyProperty dependencyProperty, object value, BaseValueSource source = BaseValueSource.Local)
        {
            SetValue(dependencyProperty, null, value, source);
        }

        public void SetValue(DependencyPropertyKey dependencyPropertyKey, object value, BaseValueSource source = BaseValueSource.Local)
        {
            SetValue(dependencyPropertyKey.DependencyProperty, dependencyPropertyKey, value, source);
        }

        private void SetValue(DependencyProperty dependencyProperty, DependencyPropertyKey dependencyPropertyKey, object value, BaseValueSource source)
        {
            VerifyReadOnlyProperty(dependencyProperty, dependencyPropertyKey);

            IExpressionProvider newExpressionProvider = value as IExpressionProvider;
            if (newExpressionProvider == null && !dependencyProperty.IsValidValue(value))
            {
                return;
            }

            IDependencyPropertyValueEntry entry = GetInitializedValueEntry(dependencyProperty);

            IExpression oldExpression = entry.GetBaseValue((int)source, false) as IExpression;

            if (newExpressionProvider != null)
            {
                value = newExpressionProvider.CreateExpression(this, dependencyProperty);
            }
            else if (oldExpression != null && oldExpression.SetValue(value))
            {
                return;
            }

            if (oldExpression is IDisposable)
            {
                ((IDisposable)oldExpression).Dispose();
            }

            entry.SetBaseValue((int)source, value);
        }

        public void ClearValue(DependencyProperty dependencyProperty, BaseValueSource source = BaseValueSource.Local)
        {
            ClearValue(dependencyProperty, null, source);
        }

        public void ClearValue(DependencyPropertyKey dependencyPropertyKey, BaseValueSource source = BaseValueSource.Local)
        {
            ClearValue(dependencyPropertyKey.DependencyProperty, dependencyPropertyKey, source);
        }

        private void ClearValue(DependencyProperty dependencyProperty, DependencyPropertyKey dependencyPropertyKey, BaseValueSource source)
        {
            VerifyReadOnlyProperty(dependencyProperty, dependencyPropertyKey);

            IDependencyPropertyValueEntry entry;
            if (!entries.TryGetValue(dependencyProperty, out entry))
            {
                return;
            }

            IExpression expression = entry.GetBaseValue((int)source, false) as IExpression;
            if (expression is IDisposable)
            {
                ((IDisposable)expression).Dispose();
            }

            entry.ClearBaseValue((int)source);
        }

        public ValueSource GetValueSource(DependencyProperty dependencyProperty)
        {
            IDependencyPropertyValueEntry entry;
            if (entries.TryGetValue(dependencyProperty, out entry))
            {
                return new ValueSource(
                    (BaseValueSource)entry.GetBaseValuePriority(),
                    entry.GetAnimationValue(true) != ObservableValue.UnsetValue,
                    entry is CoercedDependencyPropertyValueEntry,
                    entry.GetBaseValue(false) is IExpression);
            }

            PropertyMetadata propertyMetadata = dependencyProperty.GetMetadata(GetType());
            BaseValueSource baseValueSource = propertyMetadata.Inherits && inheritanceParent != null ? BaseValueSource.Inherited : BaseValueSource.Default;
            return new ValueSource(baseValueSource, false, propertyMetadata.CoerceValueCallback != null, false);
        }

        public BaseValueSource GetBaseValueSource(DependencyProperty dependencyProperty)
        {
            IDependencyPropertyValueEntry entry;

            if (entries.TryGetValue(dependencyProperty, out entry))
            {
                return (BaseValueSource)entry.GetBaseValuePriority();
            }

            return dependencyProperty.Inherits && inheritanceParent != null ? BaseValueSource.Inherited : BaseValueSource.Default;
        }

        public IDependencyPropertyValueEntry GetValueEntry(DependencyProperty dependencyProperty)
        {
            return dependencyProperty.IsReadOnly ? GetInitializedReadOnlyValueEntry(dependencyProperty) : GetInitializedValueEntry(dependencyProperty);
        }

        public IDependencyPropertyValueEntry GetValueEntry(DependencyPropertyKey dependencyPropertyKey)
        {
            VerifyReadOnlyProperty(dependencyPropertyKey.DependencyProperty, dependencyPropertyKey);
            return GetInitializedValueEntry(dependencyPropertyKey.DependencyProperty);
        }

        private IDependencyPropertyValueEntry GetInitializedValueEntry(DependencyProperty dependencyProperty)
        {
            IDependencyPropertyValueEntry entry;

            if (entries.TryGetValue(dependencyProperty, out entry))
            {
                return entry;
            }

            entry = CreateDependencyPropertyValueEntry(dependencyProperty);
            entries.Add(dependencyProperty, entry);

            return entry;
        }

        private IDependencyPropertyValueEntry GetInitializedReadOnlyValueEntry(DependencyProperty dependencyProperty)
        {
            IDependencyPropertyValueEntry readOnlyEntry;

            if (readOnlyEntries.TryGetValue(dependencyProperty, out readOnlyEntry))
            {
                return readOnlyEntry;
            }

            readOnlyEntry = new ReadOnlyDependencyPropertyValueEntry(GetInitializedValueEntry(dependencyProperty));
            readOnlyEntries.Add(dependencyProperty, readOnlyEntry);

            return readOnlyEntry;
        }

        // create dependency property entry and set its default and initial inherited value
        private IDependencyPropertyValueEntry CreateDependencyPropertyValueEntry(DependencyProperty dependencyProperty)
        {
            IDependencyPropertyValueEntry entry = new DependencyPropertyValueEntry(this, dependencyProperty);

            PropertyMetadata propertyMetadata = dependencyProperty.GetMetadata(GetType());

            if (propertyMetadata.CoerceValueCallback != null)
            {
                entry = new CoercedDependencyPropertyValueEntry(entry, this, propertyMetadata.CoerceValueCallback);
            }

            entry.SetBaseValue((int)BaseValueSource.Default, propertyMetadata.DefaultValue);

            if (dependencyProperty.Inherits && inheritanceParent != null)
            {
                entry.SetBaseValue((int)BaseValueSource.Inherited, inheritanceParent.GetValue(dependencyProperty));
            }

            entry.ValueChanged += OnDependencyPropertyValueChanged;

            return entry;
        }

        private void OnDependencyPropertyValueChanged(object sender, ObservableValueChangedArgs e)
        {
            DependencyProperty dependencyProperty = entries.Where(keyValuePair => keyValuePair.Value == sender).Select(keyValuePair => keyValuePair.Key).First() as DependencyProperty;
            RaisePropertyChanged(new DependencyPropertyChangedEventArgs(dependencyProperty, e.OldValue, e.NewValue));
        }

        protected void RaisePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
            e.Property.RaiseMetadataPropertyChangedCallback(this, e);
            PropertyChanged.Raise(this, e);
        }

        protected virtual void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            //
        }

        protected internal void SetInheritanceParent(DependencyObject parent)
        {
            if (inheritanceParent == parent)
            {
                return;
            }

            DependencyObject oldInheritanceParent = inheritanceParent;

            if (inheritanceParent != null)
            {
                inheritanceParent.PropertyChanged -= OnParentPropertyChanged;
            }

            this.inheritanceParent = parent;

            if (inheritanceParent != null)
            {
                inheritanceParent.PropertyChanged += OnParentPropertyChanged;
            }

            IEnumerable<DependencyProperty> inheritedProperties = DependencyProperty.GetFlattenedProperties(GetType()).Where(property => property.Inherits);

            foreach (DependencyProperty property in inheritedProperties)
            {
                IDependencyPropertyValueEntry entry;
                if (entries.TryGetValue(property, out entry))
                {
                    // copy or clear inherited value for existing entry
                    if (inheritanceParent != null)
                    {
                        entry.SetBaseValue((int)BaseValueSource.Inherited, inheritanceParent.GetValue(property));
                    }
                    else
                    {
                        entry.ClearBaseValue((int)BaseValueSource.Inherited);
                    }
                }
                else
                {
                    PropertyMetadata propertyMetadata = property.GetMetadata(GetType());
                    object oldValue = oldInheritanceParent != null ? oldInheritanceParent.GetValue(property) : propertyMetadata.DefaultValue;
                    object newValue = inheritanceParent != null ? inheritanceParent.GetValue(property) : propertyMetadata.DefaultValue;

                    if (!Granular.Compatibility.EqualityComparer<object>.Default.Equals(oldValue, newValue))
                    {
                        // raise inherited value changed between parents
                        RaisePropertyChanged(new DependencyPropertyChangedEventArgs(property, oldValue, newValue));
                    }
                }
            }

            OnInheritanceParentChanged(oldInheritanceParent, inheritanceParent);
        }

        protected virtual void OnInheritanceParentChanged(DependencyObject oldInheritanceParent, DependencyObject newInheritanceParent)
        {
            //
        }

        private void OnParentPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!e.Property.Inherits)
            {
                return;
            }

            IDependencyPropertyValueEntry entry;
            if (entries.TryGetValue(e.Property, out entry))
            {
                // the entry will raise an event if it's changed
                entry.SetBaseValue((int)BaseValueSource.Inherited, e.NewValue);
            }
            else
            {
                // entry doesn't exist so the value was inherited and changed
                RaisePropertyChanged(e);
            }
        }

        private static void VerifyReadOnlyProperty(DependencyProperty dependencyProperty, DependencyPropertyKey dependencyPropertyKey)
        {
            if (dependencyProperty.IsReadOnly &&
                (dependencyPropertyKey == null || dependencyPropertyKey.DependencyProperty != dependencyProperty || !DependencyProperty.IsValidReadOnlyKey(dependencyPropertyKey)))
            {
                throw new Granular.Exception("Can't modify the read-only dependency property \"{0}\" without its key", dependencyProperty);
            }
        }
    }
}
