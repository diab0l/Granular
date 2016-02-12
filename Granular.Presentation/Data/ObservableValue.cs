using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace System.Windows.Data
{
    public delegate void ObservableValueChangedEventHandler(object sender, ObservableValueChangedEventArgs e);

    public class ObservableValueChangedEventArgs : EventArgs
    {
        public object NewValue { get; private set; }
        public object OldValue { get; private set; }

        public ObservableValueChangedEventArgs(object oldValue, object newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }

    public interface IObservableValue
    {
        event ObservableValueChangedEventHandler ValueChanged;
        object Value { get; }
    }

    public class NamedObject
    {
        private string name;

        public NamedObject(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return String.Format("{{{0}}}", name);
        }
    }

    [DebuggerNonUserCode]
    public class ObservableValue : IObservableValue
    {
        public static readonly NamedObject UnsetValue = new NamedObject("ObservableValue.UnsetValue");

        public event ObservableValueChangedEventHandler ValueChanged;
        public object Value { get; private set; }

        private IObservableValue baseObservableValue;
        private object baseValue;
        public object BaseValue
        {
            get { return baseValue; }
            set
            {
                if (baseValue == value)
                {
                    return;
                }

                if (baseObservableValue != null)
                {
                    baseObservableValue.ValueChanged -= OnBaseObservableValueChanged;
                }

                baseValue = value;
                baseObservableValue = value as IObservableValue;

                if (baseObservableValue != null)
                {
                    baseObservableValue.ValueChanged += OnBaseObservableValueChanged;
                }

                object oldValue = Value;
                object newValue = baseObservableValue != null ? baseObservableValue.Value : baseValue;

                if (Granular.Compatibility.EqualityComparer.Default.Equals(oldValue, newValue))
                {
                    return;
                }

                Value = newValue;
                ValueChanged.Raise(this, new ObservableValueChangedEventArgs(oldValue, newValue));
            }
        }

        public ObservableValue() :
            this(UnsetValue)
        {
            //
        }

        public ObservableValue(object baseValue)
        {
            this.BaseValue = baseValue;
        }

        private void OnBaseObservableValueChanged(object sender, ObservableValueChangedEventArgs e)
        {
            Value = e.NewValue;
            ValueChanged.Raise(this, e);
        }

        public static bool IsNullOrUnset(object value)
        {
            return value == null || value == UnsetValue;
        }
    }

    [DebuggerNonUserCode]
    public static class ObservableValueChangedEventHandlerExtensions
    {
        public static void Raise(this ObservableValueChangedEventHandler handler, object sender, ObservableValueChangedEventArgs e)
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}