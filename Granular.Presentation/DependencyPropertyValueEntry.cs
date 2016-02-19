using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Granular.Extensions;

namespace System.Windows
{
    public interface IDependencyPropertyValueEntry
    {
        event DependencyPropertyChangedEventHandler ValueChanged;
        object Value { get; }
        int ValuePriority { get; }

        object GetValue(int priority, bool flattened);
        void SetValue(int priority, object value);

        int GetBaseValuePriority();
        void CoerceValue();
    }

    [DebuggerNonUserCode]
    public class DependencyPropertyValueEntry : IDependencyPropertyValueEntry
    {
        private class IndexedObservableValue : ObservableValue
        {
            public int Index { get; private set; }

            public IndexedObservableValue(int index, object baseValue) :
                base(baseValue)
            {
                this.Index = index;
            }
        }

        public const int BaseValuePriorities = 12;
        public const int ValuePriorities = BaseValuePriorities + 2;

        public const int BaseValueHighestPriority = BaseValuePriorities - 1;
        public const int CurrentValuePriority = BaseValuePriorities;
        public const int AnimationValuePriority = BaseValuePriorities + 1;

        public event DependencyPropertyChangedEventHandler ValueChanged;
        private object value;
        public object Value
        {
            get { return value; }
            private set
            {
                if (this.value is INotifyChanged)
                {
                    ((INotifyChanged)this.value).Changed -= notifyValueChangedEventHandler;
                }

                this.value = value;

                if (this.value is INotifyChanged)
                {
                    ((INotifyChanged)this.value).Changed += notifyValueChangedEventHandler ?? (notifyValueChangedEventHandler = OnValueNotifyChanged);
                }
            }
        }

        public int ValuePriority { get; private set; }

        // [base values, current value, animation value]
        private IndexedObservableValue[] observableValues;
        private object[] values;

        private int baseValuePriority;

        private DependencyObject dependencyObject;
        private DependencyProperty dependencyProperty;
        private CoerceValueCallback coerceValueCallback;

        private EventHandler notifyValueChangedEventHandler;
        private ObservableValueChangedEventHandler indexedObservableValueChangedEventHandler;

        public DependencyPropertyValueEntry(DependencyObject dependencyObject, DependencyProperty dependencyProperty, CoerceValueCallback coerceValueCallback = null) // 
        {
            this.dependencyObject = dependencyObject;
            this.dependencyProperty = dependencyProperty;
            this.coerceValueCallback = coerceValueCallback;

            values = new object[BaseValuePriorities + 2];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = ObservableValue.UnsetValue;
            }
        }

        public object GetValue(int priority, bool flattened)
        {
            if (observableValues != null && observableValues[priority] != null)
            {
                return flattened ? observableValues[priority].Value : observableValues[priority].BaseValue;
            }

            return values[priority];
        }

        public void SetValue(int priority, object value)
        {
            if (observableValues != null && observableValues[priority] != null)
            {
                observableValues[priority].BaseValue = value;
                return;
            }

            object oldValue = values[priority];

            if (value is IObservableValue)
            {
                if (observableValues == null)
                {
                    observableValues = new IndexedObservableValue[BaseValuePriorities + 2];
                }

                IndexedObservableValue indexedObservableValue = new IndexedObservableValue(priority, oldValue);
                indexedObservableValue.ValueChanged += indexedObservableValueChangedEventHandler ?? (indexedObservableValueChangedEventHandler = OnIndexedObservableValueChanged);

                observableValues[priority] = indexedObservableValue;
                values[priority] = ObservableValue.UnsetValue;

                indexedObservableValue.BaseValue = value;
                return;
            }

            if (Granular.Compatibility.EqualityComparer.Default.Equals(oldValue, value))
            {
                return;
            }

            values[priority] = value;
            OnValueChanged(priority, value);
        }

        public int GetBaseValuePriority()
        {
            if (baseValuePriority > BaseValueHighestPriority)
            {
                baseValuePriority = BaseValueHighestPriority;

                while (baseValuePriority > 0 && !IsValueValid(GetValue(baseValuePriority, true)))
                {
                    baseValuePriority--;
                }
            }

            return baseValuePriority;
        }

        public void CoerceValue()
        {
            if (coerceValueCallback == null)
            {
                return;
            }

            object oldValue = Value;
            object newValue = coerceValueCallback(dependencyObject, GetValue(ValuePriority, true));

            if (Granular.Compatibility.EqualityComparer.Default.Equals(oldValue, newValue))
            {
                return;
            }

            Value = newValue;
            ValueChanged.Raise(this, new DependencyPropertyChangedEventArgs(dependencyProperty, oldValue, newValue));
        }

        private void OnIndexedObservableValueChanged(object sender, ObservableValueChangedEventArgs e)
        {
            OnValueChanged(((IndexedObservableValue)sender).Index, e.NewValue);
        }

        private void OnValueChanged(int newValuePriority, object newValue)
        {
            if (ValuePriority > newValuePriority) // a higher priority value is hiding the new value
            {
                if (baseValuePriority <= newValuePriority && newValuePriority <= BaseValueHighestPriority)
                {
                    baseValuePriority = BaseValueHighestPriority + 1; // invalidate baseValuePriority
                }

                return;
            }

            object oldValue = Value;
            bool isNewValueValid = IsValueValid(newValue);

            if (ValuePriority == newValuePriority && isNewValueValid && coerceValueCallback == null)
            {
                Value = newValue;
                ValueChanged.Raise(this, new DependencyPropertyChangedEventArgs(dependencyProperty, oldValue, newValue)); // since this was already the value priority and there is no coercion, Value must have been changed here
                return;
            }

            if (ValuePriority < newValuePriority && !isNewValueValid) // a higher priority value was changed but it's not valid, so it can be ignored
            {
                return;
            }

            while (!isNewValueValid && newValuePriority > 0) // try to find the highest priority value that is valid
            {
                newValuePriority--;
                newValue = GetValue(newValuePriority, true);
                isNewValueValid = IsValueValid(newValue);
            }

            if (ValuePriority != newValuePriority)
            {
                ValuePriority = newValuePriority;
                baseValuePriority = newValuePriority; // possible invalidation of baseValuePriority
            }

            if (coerceValueCallback != null)
            {
                newValue = coerceValueCallback(dependencyObject, newValue);
            }

            if (Granular.Compatibility.EqualityComparer.Default.Equals(oldValue, newValue))
            {
                return;
            }

            Value = newValue;
            ValueChanged.Raise(this, new DependencyPropertyChangedEventArgs(dependencyProperty, oldValue, newValue));
        }

        private void OnValueNotifyChanged(object sender, EventArgs e)
        {
            ValueChanged.Raise(this, new DependencyPropertyChangedEventArgs(dependencyProperty, Value));
        }

        private bool IsValueValid(object newValue)
        {
            return newValue != ObservableValue.UnsetValue && dependencyProperty.IsValidValue(newValue);
        }
    }

    [DebuggerNonUserCode]
    public class ReadOnlyDependencyPropertyValueEntry : IDependencyPropertyValueEntry
    {
        public event DependencyPropertyChangedEventHandler ValueChanged;
        public object Value { get { return source.Value; } }
        public int ValuePriority { get { return source.ValuePriority; } }

        private IDependencyPropertyValueEntry source;

        public ReadOnlyDependencyPropertyValueEntry(IDependencyPropertyValueEntry source)
        {
            this.source = source;

            source.ValueChanged += (sender, e) => ValueChanged.Raise(this, e);
        }

        public object GetValue(bool flattened)
        {
            return source.GetValue(source.ValuePriority, flattened);
        }

        public object GetValue(int priority, bool flattened)
        {
            return source.GetValue(priority, flattened);
        }

        public void SetValue(int priority, object value)
        {
            throw new Granular.Exception("Can't modify a readonly dependency property value");
        }

        public int GetBaseValuePriority()
        {
            return source.GetBaseValuePriority();
        }

        public void CoerceValue()
        {
            source.CoerceValue();
        }
    }

    [DebuggerNonUserCode]
    public static class DependencyPropertyValueEntryExtensions
    {
        public static object GetBaseValue(this IDependencyPropertyValueEntry entry, bool flattened)
        {
            return entry.GetValue(entry.GetBaseValuePriority(), flattened);
        }

        public static object GetBaseValue(this IDependencyPropertyValueEntry entry, int priority, bool flattened)
        {
            return entry.GetValue(priority, flattened);
        }

        public static void SetBaseValue(this IDependencyPropertyValueEntry entry, int priority, object value)
        {
            entry.SetValue(priority, value);
        }

        public static void ClearBaseValue(this IDependencyPropertyValueEntry entry, int priority)
        {
            entry.SetValue(priority, ObservableValue.UnsetValue);
        }

        public static object GetCurrentValue(this IDependencyPropertyValueEntry entry, bool flattened)
        {
            return entry.GetValue(DependencyPropertyValueEntry.CurrentValuePriority, flattened);
        }

        public static void SetCurrentValue(this IDependencyPropertyValueEntry entry, object value)
        {
            entry.SetValue(DependencyPropertyValueEntry.CurrentValuePriority, value);
        }

        public static void ClearCurrentValue(this IDependencyPropertyValueEntry entry)
        {
            entry.SetValue(DependencyPropertyValueEntry.CurrentValuePriority, ObservableValue.UnsetValue);
        }

        public static object GetAnimationValue(this IDependencyPropertyValueEntry entry, bool flattened)
        {
            return entry.GetValue(DependencyPropertyValueEntry.AnimationValuePriority, flattened);
        }

        public static void SetAnimationValue(this IDependencyPropertyValueEntry entry, object value)
        {
            entry.SetValue(DependencyPropertyValueEntry.AnimationValuePriority, value);
        }

        public static void ClearAnimationValue(this IDependencyPropertyValueEntry entry)
        {
            entry.SetValue(DependencyPropertyValueEntry.AnimationValuePriority, ObservableValue.UnsetValue);
        }
    }
}
