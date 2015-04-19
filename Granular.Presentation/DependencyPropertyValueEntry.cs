using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Granular.Extensions;

namespace System.Windows
{
    public interface IDependencyPropertyValueEntry : IObservableValue
    {
        object GetBaseValue(bool flattened);
        object GetBaseValue(int priority, bool flattened);
        void SetBaseValue(int priority, object value);
        void ClearBaseValue(int priority);
        int GetBaseValuePriority();

        object GetCurrentValue(bool flattened);
        void SetCurrentValue(object value);
        void ClearCurrentValue();

        object GetAnimationValue(bool flattened);
        void SetAnimationValue(object value);
        void ClearAnimationValue();
    }

    [DebuggerNonUserCode]
    public class DependencyPropertyValueEntry : IDependencyPropertyValueEntry
    {
        private const int BaseValuePriorities = 12;

        public event EventHandler<ObservableValueChangedArgs> ValueChanged;

        public object Value { get { return observableValue.Value; } }

        private ObservableValue observableValue;
        private object[] baseValues;
        private object currentValue;
        private object animationValue;
        private DependencyProperty dependencyProperty;
        private object defaultValue;

        public DependencyPropertyValueEntry(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            this.dependencyProperty = dependencyProperty;
            this.defaultValue = dependencyProperty.GetMetadata(dependencyObject.GetType()).DefaultValue;

            observableValue = new ObservableValue();
            observableValue.ValueChanged += (sender, e) => ValueChanged.Raise(this, e);

            baseValues = new object[BaseValuePriorities];

            for (int i = 0; i < baseValues.Length; i++)
            {
                baseValues[i] = ObservableValue.UnsetValue;
            }

            currentValue = ObservableValue.UnsetValue;
            animationValue = ObservableValue.UnsetValue;
        }

        public object GetBaseValue(bool flattened)
        {
            for (int i = baseValues.Length - 1; i >= 0; i--)
            {
                object flattenedValue = GetFlattenedValue(baseValues[i]);
                if (flattenedValue != ObservableValue.UnsetValue)
                {
                    return flattened ? flattenedValue : baseValues[i];
                }
            }

            return ObservableValue.UnsetValue;
        }

        public object GetBaseValue(int priority, bool flattened)
        {
            return flattened ? GetFlattenedValue(baseValues[priority]) : baseValues[priority];
        }

        public void SetBaseValue(int priority, object value)
        {
            IObservableValue oldObservableValue = baseValues[priority] as IObservableValue;
            if (oldObservableValue != null)
            {
                oldObservableValue.ValueChanged -= OnObservableValueChanged;
            }

            baseValues[priority] = value;

            IObservableValue newObservableValue = baseValues[priority] as IObservableValue;
            if (newObservableValue != null)
            {
                newObservableValue.ValueChanged += OnObservableValueChanged;
            }

            SetValue();
        }

        public void ClearBaseValue(int priority)
        {
            SetBaseValue(priority, ObservableValue.UnsetValue);
        }

        public int GetBaseValuePriority()
        {
            return Granular.Compatibility.Array.FindLastIndex(baseValues, value => value != ObservableValue.UnsetValue);
        }

        public object GetCurrentValue(bool flattened)
        {
            return flattened ? GetFlattenedValue(currentValue) : currentValue;
        }

        public void SetCurrentValue(object value)
        {
            IObservableValue oldObservableValue = currentValue as IObservableValue;
            if (oldObservableValue != null)
            {
                oldObservableValue.ValueChanged -= OnObservableValueChanged;
            }

            currentValue = value;

            IObservableValue newObservableValue = currentValue as IObservableValue;
            if (newObservableValue != null)
            {
                newObservableValue.ValueChanged += OnObservableValueChanged;
            }

            SetValue();
        }

        public void ClearCurrentValue()
        {
            SetCurrentValue(ObservableValue.UnsetValue);
        }

        public object GetAnimationValue(bool flattened)
        {
            return flattened ? GetFlattenedValue(animationValue) : animationValue;
        }

        public void SetAnimationValue(object value)
        {
            IObservableValue oldObservableValue = animationValue as IObservableValue;
            if (oldObservableValue != null)
            {
                oldObservableValue.ValueChanged -= OnObservableValueChanged;
            }

            animationValue = value;

            IObservableValue newObservableValue = animationValue as IObservableValue;
            if (newObservableValue != null)
            {
                newObservableValue.ValueChanged += OnObservableValueChanged;
            }

            SetValue();
        }

        public void ClearAnimationValue()
        {
            animationValue = ObservableValue.UnsetValue;
            SetValue();
        }

        private void OnObservableValueChanged(object sender, ObservableValueChangedArgs e)
        {
            SetValue();
        }

        private void SetValue()
        {
            object value = GetAnimationValue(true);

            if (value == ObservableValue.UnsetValue)
            {
                value = GetCurrentValue(true);
            }

            if (value == ObservableValue.UnsetValue)
            {
                value = GetBaseValue(true);
            }

            if (value == ObservableValue.UnsetValue || !dependencyProperty.IsValidValue(value))
            {
                value = defaultValue;
            }

            observableValue.Value = value;
        }

        // get the inner IObservableValue.Value
        private static object GetFlattenedValue(object value)
        {
            IObservableValue observableValue = value as IObservableValue;
            return observableValue != null ? GetFlattenedValue(observableValue.Value) : value;
        }
    }

    public class CoercedDependencyPropertyValueEntry : IDependencyPropertyValueEntry
    {
        public event EventHandler<ObservableValueChangedArgs> ValueChanged;

        public object Value { get { return observableValue.Value; } }

        public bool IsCoerced { get { return !Granular.Compatibility.EqualityComparer<object>.Default.Equals(this.Value, source.Value); } }

        private IDependencyPropertyValueEntry source;
        private CoerceValueCallback coerceValueCallback;
        private DependencyObject dependencyObject;
        private ObservableValue observableValue;

        public CoercedDependencyPropertyValueEntry(IDependencyPropertyValueEntry source, DependencyObject dependencyObject, CoerceValueCallback coerceValueCallback)
        {
            this.source = source;
            this.dependencyObject = dependencyObject;
            this.coerceValueCallback = coerceValueCallback;

            observableValue = new ObservableValue();
            observableValue.ValueChanged += (sender, e) => ValueChanged.Raise(this, e);

            source.ValueChanged += (sender, e) => CoerceValue();
            CoerceValue();
        }

        public void CoerceValue()
        {
            observableValue.Value = source.Value != ObservableValue.UnsetValue ? coerceValueCallback(dependencyObject, source.Value) : ObservableValue.UnsetValue;
        }

        public object GetBaseValue(bool flattened)
        {
            return source.GetBaseValue(flattened);
        }

        public object GetBaseValue(int priority, bool flattened)
        {
            return source.GetBaseValue(priority, flattened);
        }

        public void SetBaseValue(int priority, object value)
        {
            source.SetBaseValue(priority, value);
        }

        public void ClearBaseValue(int priority)
        {
            source.ClearBaseValue(priority);
        }

        public int GetBaseValuePriority()
        {
            return source.GetBaseValuePriority();
        }

        public object GetCurrentValue(bool flattened)
        {
            return source.GetCurrentValue(flattened);
        }

        public void SetCurrentValue(object value)
        {
            source.SetCurrentValue(value);
        }

        public void ClearCurrentValue()
        {
            source.ClearCurrentValue();
        }

        public object GetAnimationValue(bool flattened)
        {
            return source.GetAnimationValue(flattened);
        }

        public void SetAnimationValue(object value)
        {
            source.SetAnimationValue(value);
        }

        public void ClearAnimationValue()
        {
            source.ClearAnimationValue();
        }
    }

    public class ReadOnlyDependencyPropertyValueEntry : IDependencyPropertyValueEntry
    {
        public event EventHandler<ObservableValueChangedArgs> ValueChanged;

        public object Value { get { return source.Value; } }

        private IDependencyPropertyValueEntry source;

        public ReadOnlyDependencyPropertyValueEntry(IDependencyPropertyValueEntry source)
        {
            this.source = source;

            source.ValueChanged += (sender, e) => ValueChanged.Raise(this, e);
        }

        public object GetBaseValue(bool flattened)
        {
            return source.GetBaseValue(flattened);
        }

        public object GetBaseValue(int priority, bool flattened)
        {
            return source.GetBaseValue(priority, flattened);
        }

        public void SetBaseValue(int priority, object value)
        {
            ThrowReadOnlyException();
        }

        public void ClearBaseValue(int priority)
        {
            ThrowReadOnlyException();
        }

        public int GetBaseValuePriority()
        {
            return source.GetBaseValuePriority();
        }

        public object GetCurrentValue(bool flattened)
        {
            return source.GetCurrentValue(flattened);
        }

        public void SetCurrentValue(object value)
        {
            ThrowReadOnlyException();
        }

        public void ClearCurrentValue()
        {
            ThrowReadOnlyException();
        }

        public object GetAnimationValue(bool flattened)
        {
            return source.GetAnimationValue(flattened);
        }

        public void SetAnimationValue(object value)
        {
            ThrowReadOnlyException();
        }

        public void ClearAnimationValue()
        {
            ThrowReadOnlyException();
        }

        private static void ThrowReadOnlyException()
        {
            throw new Granular.Exception("Can't modify a read-only dependency property value");
        }
    }
}
