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
        public bool IsExpression { get; private set; }
        public bool IsCurrent { get; private set; }
        public bool IsAnimated { get; private set; }
        public bool IsCoerced { get; private set; }

        public ValueSource(BaseValueSource baseValueSource, bool isExpression, bool isCurrent, bool isAnimated, bool isCoerced)
        {
            this.BaseValueSource = baseValueSource;
            this.IsExpression = isExpression;
            this.IsCurrent = isCurrent;
            this.IsAnimated = isAnimated;
            this.IsCoerced = isCoerced;
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
            if (!entries.TryGetValue(dependencyProperty, out entry))
            {
                PropertyMetadata propertyMetadata = dependencyProperty.GetMetadata(GetType());

                // no need to create a new entry if the value is not inherited or coerced
                if (!propertyMetadata.Inherits && (propertyMetadata.CoerceValueCallback == null || !dependencyProperty.IsAttached && !dependencyProperty.IsContainedBy(GetType())))
                {
                    return propertyMetadata.DefaultValue;
                }

                entry = CreateDependencyPropertyValueEntry(dependencyProperty, propertyMetadata);
                entries.Add(dependencyProperty, entry);
            }

            return entry.Value;
        }

        public object GetValue(DependencyPropertyKey dependencyPropertyKey)
        {
            return GetValue(dependencyPropertyKey.DependencyProperty);
        }

        public void SetValue(DependencyProperty dependencyProperty, object value, BaseValueSource source = BaseValueSource.Local)
        {
            SetValue(dependencyProperty, null, value, source: source);
        }

        public void SetValue(DependencyPropertyKey dependencyPropertyKey, object value, BaseValueSource source = BaseValueSource.Local)
        {
            SetValue(dependencyPropertyKey.DependencyProperty, dependencyPropertyKey, value, source: source);
        }

        public void SetCurrentValue(DependencyProperty dependencyProperty, object value)
        {
            SetValue(dependencyProperty, null, value, setCurrentValue: true);
        }

        public void SetCurrentValue(DependencyPropertyKey dependencyPropertyKey, object value)
        {
            SetValue(dependencyPropertyKey.DependencyProperty, dependencyPropertyKey, value, setCurrentValue: true);
        }

        private void SetValue(DependencyProperty dependencyProperty, DependencyPropertyKey dependencyPropertyKey, object value, bool setCurrentValue = false, BaseValueSource source = BaseValueSource.Unknown)
        {
            VerifyReadOnlyProperty(dependencyProperty, dependencyPropertyKey);

            IExpressionProvider newExpressionProvider = value as IExpressionProvider;
            if (newExpressionProvider == null && !dependencyProperty.IsValidValue(value))
            {
                return; // invalid value
            }

            IDependencyPropertyValueEntry entry = GetInitializedValueEntry(dependencyProperty);

            IExpression oldExpression = setCurrentValue ?
                entry.GetBaseValue(false) as IExpression : // current value may be set in the top priority expression
                entry.GetBaseValue((int)source, false) as IExpression;

            if (newExpressionProvider != null)
            {
                value = newExpressionProvider.CreateExpression(this, dependencyProperty);
            }
            else if (oldExpression != null && oldExpression.SetValue(value))
            {
                return; // value (current or not) was set in the existing expression, nothing else to do
            }

            if (setCurrentValue)
            {
                entry.SetCurrentValue(value);
                return; // base value isn't changed
            }

            if (oldExpression is IDisposable) // expression is being replaced
            {
                ((IDisposable)oldExpression).Dispose();
            }

            entry.SetBaseValue((int)source, value);
            entry.ClearCurrentValue();
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
            entry.ClearCurrentValue();
        }

        public void CoerceValue(DependencyProperty dependencyProperty)
        {
            IDependencyPropertyValueEntry entry = GetInitializedValueEntry(dependencyProperty);
            if (entry is CoercedDependencyPropertyValueEntry)
            {
                ((CoercedDependencyPropertyValueEntry)entry).CoerceValue();
            }
        }

        public ValueSource GetValueSource(DependencyProperty dependencyProperty)
        {
            IDependencyPropertyValueEntry entry;
            if (entries.TryGetValue(dependencyProperty, out entry))
            {
                return new ValueSource(
                    (BaseValueSource)entry.GetBaseValuePriority(),
                    entry.GetBaseValue(false) is IExpression || entry.GetCurrentValue(false) is IExpression,
                    entry.GetCurrentValue(true) != ObservableValue.UnsetValue,
                    entry.GetAnimationValue(true) != ObservableValue.UnsetValue,
                    (entry is CoercedDependencyPropertyValueEntry) && ((CoercedDependencyPropertyValueEntry)entry).IsCoerced);
            }

            PropertyMetadata propertyMetadata = dependencyProperty.GetMetadata(GetType());
            BaseValueSource baseValueSource = propertyMetadata.Inherits && inheritanceParent != null ? BaseValueSource.Inherited : BaseValueSource.Default;
            return new ValueSource(baseValueSource, false, false, false, false);
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
            if (!entries.TryGetValue(dependencyProperty, out entry))
            {
                entry = CreateDependencyPropertyValueEntry(dependencyProperty, dependencyProperty.GetMetadata(GetType()));
                entries.Add(dependencyProperty, entry);
            }

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
        private IDependencyPropertyValueEntry CreateDependencyPropertyValueEntry(DependencyProperty dependencyProperty, PropertyMetadata propertyMetadata)
        {
            IDependencyPropertyValueEntry entry = new DependencyPropertyValueEntry(this, dependencyProperty);
            entry.SetBaseValue((int)BaseValueSource.Default, propertyMetadata.DefaultValue);

            if (dependencyProperty.IsAttached || dependencyProperty.IsContainedBy(GetType()))
            {
                if (propertyMetadata.CoerceValueCallback != null)
                {
                    entry = new CoercedDependencyPropertyValueEntry(entry, this, propertyMetadata.CoerceValueCallback);
                }

                entry.ValueChanged += (sender, e) => OnContainedEntryValueChanged(new DependencyPropertyChangedEventArgs(dependencyProperty, e.OldValue, e.NewValue));
            }
            else
            {
                entry.ValueChanged += (sender, e) => OnEntryValueChanged(new DependencyPropertyChangedEventArgs(dependencyProperty, e.OldValue, e.NewValue));
            }

            return entry;
        }

        private void OnEntryValueChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
            PropertyChanged.Raise(this, e);
        }

        private void OnContainedEntryValueChanged(DependencyPropertyChangedEventArgs e)
        {
            e.Property.RaiseMetadataPropertyChangedCallback(this, e);
            OnPropertyChanged(e);
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

            if (inheritanceParent == null)
            {
                // clear inherited values
                foreach (KeyValuePair<DependencyProperty, IDependencyPropertyValueEntry> pair in entries)
                {
                    if (pair.Key.Inherits)
                    {
                        pair.Value.ClearBaseValue((int)BaseValueSource.Inherited);
                    }
                }
            }
            else
            {
                // update existing inherited values
                foreach (KeyValuePair<DependencyProperty, IDependencyPropertyValueEntry> pair in entries)
                {
                    if (pair.Key.Inherits)
                    {
                        pair.Value.SetBaseValue((int)BaseValueSource.Inherited, inheritanceParent.GetValue(pair.Key));
                    }
                }

                // add missing inherited values
                foreach (KeyValuePair<DependencyProperty, IDependencyPropertyValueEntry> pair in inheritanceParent.entries)
                {
                    if (pair.Key.Inherits)
                    {
                        GetInitializedValueEntry(pair.Key).SetBaseValue((int)BaseValueSource.Inherited, pair.Value.Value);
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
            if (e.Property.Inherits)
            {
                GetInitializedValueEntry(e.Property).SetBaseValue((int)BaseValueSource.Inherited, e.NewValue);
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
