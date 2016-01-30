using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace System.Windows.Data
{
    public class ObservableValueChangedArgs : EventArgs
    {
        public object NewValue { get; private set; }
        public object OldValue { get; private set; }

        public ObservableValueChangedArgs(object oldValue, object newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }

    public interface IObservableValue
    {
        event EventHandler<ObservableValueChangedArgs> ValueChanged;
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

        public event EventHandler<ObservableValueChangedArgs> ValueChanged;

        private object value;
        public object Value
        {
            get { return value; }
            set
            {
                if (Granular.Compatibility.EqualityComparer.Default.Equals(this.value, value))
                {
                    return;
                }

                ObservableValueChangedArgs e = new ObservableValueChangedArgs(this.value, value);

                IObservableValue oldObservableValue = this.value as IObservableValue;
                if (oldObservableValue != null)
                {
                    oldObservableValue.ValueChanged -= OnObservableValueChanged;
                }

                this.value = value;

                IObservableValue newObservableValue = this.value as IObservableValue;
                if (newObservableValue != null)
                {
                    newObservableValue.ValueChanged += OnObservableValueChanged;
                }

                ValueChanged.Raise(this, e);
            }
        }

        public ObservableValue() :
            this(UnsetValue)
        {
            //
        }

        public ObservableValue(object value)
        {
            this.Value = value;
        }

        private void OnObservableValueChanged(object sender, ObservableValueChangedArgs e)
        {
            ValueChanged.Raise(this, e);
        }

        public static bool IsNullOrUnset(object value)
        {
            return value == null || value == UnsetValue;
        }
    }

    public class ReadOnlyObservableValue : IObservableValue
    {
        public event EventHandler<ObservableValueChangedArgs> ValueChanged;

        public object Value { get { return source.Value; } }

        private IObservableValue source;

        public ReadOnlyObservableValue(IObservableValue source)
        {
            this.source = source;
            source.ValueChanged += (sender, e) => ValueChanged.Raise(this, e);
        }
    }

    public class ConstantObservableValue : IObservableValue
    {
        public event EventHandler<ObservableValueChangedArgs> ValueChanged { add { } remove { } }

        public object Value { get; private set; }

        public ConstantObservableValue(object value)
        {
            this.Value = value;
        }
    }
}