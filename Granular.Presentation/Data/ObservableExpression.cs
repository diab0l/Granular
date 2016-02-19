using System;
using System.Collections.Generic;
using System.Linq;
using Granular.Extensions;

namespace System.Windows.Data
{
    public class ObservableExpression : IObservableValue, IDisposable
    {
        public event ObservableValueChangedEventHandler ValueChanged;

        public object Value { get { return observableValue.Value; } }
        public Type ValueType { get; private set; }

        private ObservableValue observableValue;

        private Type baseObserverValueType;
        private ObservableExpression baseObserver;
        private IPropertyObserver delegateObserver;

        private IPropertyPathElement propertyPathElement;

        public ObservableExpression(string propertyPath) :
            this(null, PropertyPath.Parse(propertyPath))
        {
            //
        }

        public ObservableExpression(object baseValue, string propertyPath) :
            this(baseValue, PropertyPath.Parse(propertyPath))
        {
            //
        }

        public ObservableExpression(PropertyPath propertyPath) :
            this(null, propertyPath)
        {
            //
        }

        public ObservableExpression(object baseValue, PropertyPath propertyPath)
        {
            observableValue = new ObservableValue();
            observableValue.ValueChanged += (sender, oldValue, newValue) => ValueChanged.Raise(this, oldValue, newValue);

            if (propertyPath.IsEmpty)
            {
                observableValue.BaseValue = baseValue;
                ValueType = baseValue != null ? baseValue.GetType() : null;
            }
            else
            {
                propertyPathElement = propertyPath.Elements.Last();

                baseObserver = new ObservableExpression(baseValue, propertyPath.GetBasePropertyPath());
                baseObserver.ValueChanged += (sender, oldValue, newValue) => SetDelegateObserverBaseValue();

                SetDelegateObserverBaseValue();
            }
        }

        public void SetBaseValue(object baseValue)
        {
            if (baseObserver != null)
            {
                baseObserver.SetBaseValue(baseValue);
            }
            else
            {
                observableValue.BaseValue = baseValue;
                ValueType = baseValue != null ? baseValue.GetType() : null;
            }
        }

        public bool TrySetValue(object value)
        {
            return delegateObserver != null &&
                (Granular.Compatibility.EqualityComparer.Default.Equals(delegateObserver.Value, value) || delegateObserver.TrySetValue(value));
        }

        private void SetDelegateObserverBaseValue()
        {
            object baseValue = baseObserver.Value;

            if (ObservableValue.IsNullOrUnset(baseValue) || baseObserverValueType == baseValue.GetType())
            {
                if (delegateObserver != null)
                {
                    delegateObserver.SetBaseValue(baseValue);
                }

                return;
            }

            baseObserverValueType = baseValue.GetType();

            if (delegateObserver is IDisposable)
            {
                ((IDisposable)delegateObserver).Dispose();
            }

            delegateObserver = propertyPathElement.CreatePropertyObserver(baseObserverValueType);

            if (delegateObserver != null)
            {
                ValueType = delegateObserver.ValueType;
                delegateObserver.SetBaseValue(baseValue);
                delegateObserver.ValueChanged += (sender, oldValue, newValue) => observableValue.BaseValue = delegateObserver.Value;
                observableValue.BaseValue = delegateObserver.Value;
            }
            else
            {
                ValueType = null;
                observableValue.BaseValue = ObservableValue.UnsetValue;
            }
        }

        public void Dispose()
        {
            if (baseObserver is IDisposable)
            {
                ((IDisposable)baseObserver).Dispose();
            }

            if (delegateObserver is IDisposable)
            {
                ((IDisposable)delegateObserver).Dispose();
            }
        }
    }
}
