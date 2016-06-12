using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media;
using Granular;
using Granular.Extensions;

namespace System.Windows.Data
{
    public enum BindingMode
    {
        TwoWay,
        OneWay,
        //OneTime,
        OneWayToSource,
        Default
    }

    public enum UpdateSourceTrigger
    {
        Default,
        PropertyChanged,
        LostFocus,
        Explicit
    }

    public enum BindingStatus
    {
        //Unattached,
        Inactive,
        Active,
        Detached,
        //AsyncRequestPending,
        //PathError,
        UpdateTargetError,
        UpdateSourceError,
    }

    public class BindingExpression : IExpression, IDisposable
    {
        public static readonly NamedObject DisconnectedItem = new NamedObject("BindingExpression.DisconnectedItem");

        public event ObservableValueChangedEventHandler ValueChanged;

        public object Value { get { return targetValue.Value; } }

        public DependencyObject Target { get; private set; }
        public DependencyProperty TargetProperty { get; private set; }

        public PropertyPath Path { get; private set; }
        public object Source { get; private set; }
        public RelativeSource RelativeSource { get; private set; }
        public string ElementName { get; private set; }

        public BindingMode Mode { get; private set; }
        public UpdateSourceTrigger UpdateSourceTrigger { get; private set; }

        public IValueConverter Converter { get; private set; }
        public object ConverterParameter { get; private set; }

        public object FallbackValue { get; private set; }
        public object TargetNullValue { get; private set; }

        public BindingStatus Status { get; private set; }

        private IObservableValue sourceObserver;
        private ObservableExpression sourceExpression;
        private ObservableValue targetValue;

        private bool isSourceUpdateMode;
        private bool isTargetUpdateMode;

        private ReentrancyLock disableSourceUpdate;
        private ReentrancyLock disableTargetUpdate;

        public BindingExpression(DependencyObject target, DependencyProperty targetProperty, PropertyPath path,
            object source = null, RelativeSource relativeSource = null, string elementName = null,
            BindingMode mode = BindingMode.Default, UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
            IValueConverter converter = null, object converterParameter = null, object fallbackValue = null, object targetNullValue = null)
        {
            this.Target = target;
            this.TargetProperty = targetProperty;
            this.Path = path;
            this.Source = source;
            this.RelativeSource = relativeSource;
            this.ElementName = elementName;
            this.Mode = mode;
            this.UpdateSourceTrigger = updateSourceTrigger;
            this.Converter = converter;
            this.ConverterParameter = converterParameter;
            this.FallbackValue = fallbackValue;
            this.TargetNullValue = targetNullValue;

            Status = BindingStatus.Inactive;

            disableSourceUpdate = new ReentrancyLock();
            disableTargetUpdate = new ReentrancyLock();

            targetValue = new ObservableValue(Target.GetValue(TargetProperty));
            targetValue.ValueChanged += OnTargetValueChanged;

            BindingMode resolvedBindingMode = Mode == BindingMode.Default ? GetDefaultBindingMode(Target, TargetProperty) : Mode;

            isSourceUpdateMode = resolvedBindingMode == BindingMode.TwoWay || resolvedBindingMode == BindingMode.OneWayToSource;
            isTargetUpdateMode = resolvedBindingMode == BindingMode.TwoWay || resolvedBindingMode == BindingMode.OneWay;

            sourceObserver = CreateSourceObserver(Target, Source, RelativeSource, ElementName);
            sourceExpression = new ObservableExpression(sourceObserver, Path ?? PropertyPath.Empty);

            // try to update the target (or the source on OneWayToSource)
            if (isTargetUpdateMode)
            {
                sourceExpression.ValueChanged += (sender, oldValue, newValue) => UpdateTargetOnSourceChanged();
                UpdateTargetOnSourceChanged();
            }
            else if (isSourceUpdateMode)
            {
                sourceExpression.ValueChanged += (sender, oldValue, newValue) =>
                {
                    if (Status == BindingStatus.UpdateSourceError && sourceExpression.Value != ObservableValue.UnsetValue && !disableTargetUpdate)
                    {
                        // source was connected
                        UpdateSourceOnTargetChanged();
                    }
                };

                UpdateSourceOnTargetChanged();
            }

            if (UpdateSourceTrigger == UpdateSourceTrigger.LostFocus && isSourceUpdateMode && Target is UIElement)
            {
                ((UIElement)Target).LostFocus += OnLostFocus;
            }
        }

        public void Dispose()
        {
            sourceExpression.Dispose();

            if (sourceObserver is IDisposable)
            {
                ((IDisposable)sourceObserver).Dispose();
            }

            if (UpdateSourceTrigger == UpdateSourceTrigger.LostFocus && isSourceUpdateMode && Target is UIElement)
            {
                ((UIElement)Target).LostFocus -= OnLostFocus;
            }

            Target = null;
            TargetProperty = null;

            Status = BindingStatus.Detached;
        }

        public bool SetValue(object value)
        {
            targetValue.BaseValue = value;
            return true;
        }

        private void UpdateTargetOnSourceChanged()
        {
            if (disableTargetUpdate)
            {
                return;
            }

            using (disableSourceUpdate.Enter())
            {
                UpdateTarget();
            }
        }

        public void UpdateTarget()
        {
            object value = sourceExpression != null ? sourceExpression.Value : ObservableValue.UnsetValue;

            if (value == ObservableValue.UnsetValue && FallbackValue != null)
            {
                value = FallbackValue;
            }
            else if (value == null && TargetNullValue != null)
            {
                value = TargetNullValue;
            }
            else if (value != ObservableValue.UnsetValue && Converter != null)
            {
                value = Converter.Convert(value, TargetProperty.PropertyType, ConverterParameter);
            }

            targetValue.BaseValue = value;

            Status = value != ObservableValue.UnsetValue ? BindingStatus.Active : BindingStatus.UpdateTargetError;
        }

        private void UpdateSourceOnTargetChanged()
        {
            if (disableSourceUpdate)
            {
                return;
            }

            using (disableTargetUpdate.Enter())
            {
                UpdateSource();
            }
        }

        public void UpdateSource()
        {
            object convertedValue = Converter != null ? Converter.ConvertBack(targetValue.Value, sourceExpression.ValueType, ConverterParameter) : targetValue.Value;

            if (sourceExpression.TrySetValue(convertedValue))
            {
                Status = BindingStatus.Active;
            }
            else
            {
                Status = BindingStatus.UpdateSourceError;
            }
        }

        private void OnTargetValueChanged(object sender, object oldValue, object newValue)
        {
            if (UpdateSourceTrigger == UpdateSourceTrigger.Default && isSourceUpdateMode)
            {
                UpdateSourceOnTargetChanged();
            }

            ValueChanged.Raise(this, oldValue, newValue);
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            UpdateSourceOnTargetChanged();
        }

        private static IObservableValue CreateSourceObserver(DependencyObject target, object source, RelativeSource relativeSource, string elementName)
        {
            if (source != null)
            {
                return new StaticObservableValue(source);
            }

            if (relativeSource != null)
            {
                return relativeSource.CreateSourceObserver(target);
            }

            if (!elementName.IsNullOrEmpty())
            {
                return new ScopeElementSourceObserver(target, elementName);
            }

            return new DataContextSourceObserver(target);
        }

        private static BindingMode GetDefaultBindingMode(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            FrameworkPropertyMetadata frameworkPropertyMetadata = dependencyProperty.GetMetadata(dependencyObject.GetType()) as FrameworkPropertyMetadata;
            return frameworkPropertyMetadata != null && frameworkPropertyMetadata.BindsTwoWayByDefault ? BindingMode.TwoWay : BindingMode.OneWay;
        }
    }
}
