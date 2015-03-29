using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media;
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

        public event EventHandler<ObservableValueChangedArgs> ValueChanged;

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

        private ObservableExpression sourceExpression;
        private ObservableValue targetValue;

        private bool isSourceUpdateMode;
        private bool isTargetUpdateMode;

        private bool isSourceUpdateDisabled;
        private bool isTargetUpdateDisabled;

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

            targetValue = new ObservableValue(Target.GetValue(TargetProperty));
            targetValue.ValueChanged += OnTargetValueChanged;

            BindingMode resolvedBindingMode = Mode == BindingMode.Default ? GetDefaultBindingMode(Target, TargetProperty) : Mode;

            isSourceUpdateMode = resolvedBindingMode == BindingMode.TwoWay || resolvedBindingMode == BindingMode.OneWayToSource;
            isTargetUpdateMode = resolvedBindingMode == BindingMode.TwoWay || resolvedBindingMode == BindingMode.OneWay;

            object resolvedSource = Source ?? GetRelativeSource(Target, RelativeSource, ElementName);
            sourceExpression = new ObservableExpression(resolvedSource, Path);

            // try to update the target (or the source on OneWayToSource)
            if (isTargetUpdateMode)
            {
                sourceExpression.ValueChanged += (sender, e) => UpdateTargetOnSourceChanged();
                UpdateTargetOnSourceChanged();
            }
            else if (isSourceUpdateMode)
            {
                sourceExpression.ValueChanged += (sender, e) =>
                {
                    if (Status == BindingStatus.UpdateSourceError && sourceExpression.Value != ObservableValue.UnsetValue && !isTargetUpdateDisabled)
                    {
                        // source was connected
                        UpdateSourceOnTargetChanged();
                    }
                };

                UpdateSourceOnTargetChanged();
            }

            if (((RelativeSource != null && RelativeSource.Mode != RelativeSourceMode.Self) || !ElementName.IsNullOrEmpty()) && Target is Visual)
            {
                ((Visual)Target).VisualAncestorChanged += OnTargetVisualAncestorChanged;
            }

            if (UpdateSourceTrigger == UpdateSourceTrigger.LostFocus && isSourceUpdateMode && Target is UIElement)
            {
                ((UIElement)Target).LostFocus += OnLostFocus;
            }
        }

        public void Dispose()
        {
            sourceExpression.Dispose();

            if (Target is Visual)
            {
                ((Visual)Target).VisualAncestorChanged -= OnTargetVisualAncestorChanged;
            }

            if (Target is UIElement)
            {
                ((UIElement)Target).LostFocus -= OnLostFocus;
            }

            Target = null;
            TargetProperty = null;

            Status = BindingStatus.Detached;
        }

        public bool SetValue(object value)
        {
            targetValue.Value = value;
            return true;
        }

        private void OnTargetVisualAncestorChanged(object sender, EventArgs e)
        {
            sourceExpression.SetBaseValue(Source ?? GetRelativeSource(Target, RelativeSource, ElementName));
        }

        private void UpdateTargetOnSourceChanged()
        {
            if (isTargetUpdateDisabled)
            {
                return;
            }

            using (DisableSourceUpdate())
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

            targetValue.Value = value;

            Status = value != ObservableValue.UnsetValue ? BindingStatus.Active : BindingStatus.UpdateTargetError;
        }

        private void UpdateSourceOnTargetChanged()
        {
            if (isSourceUpdateDisabled)
            {
                return;
            }

            using (DisableTargetUpdate())
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

        private void OnTargetValueChanged(object sender, ObservableValueChangedArgs e)
        {
            if (UpdateSourceTrigger == UpdateSourceTrigger.Default && isSourceUpdateMode)
            {
                UpdateSourceOnTargetChanged();
            }

            ValueChanged.Raise(this, e);
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            UpdateSourceOnTargetChanged();
        }

        private IDisposable DisableSourceUpdate()
        {
            if (isSourceUpdateDisabled)
            {
                throw new Granular.Exception("Source update is already disabled");
            }

            isSourceUpdateDisabled = true;
            return new Disposable(() => isSourceUpdateDisabled = false);
        }

        private IDisposable DisableTargetUpdate()
        {
            if (isTargetUpdateDisabled)
            {
                throw new Granular.Exception("Target update is already disabled");
            }

            isTargetUpdateDisabled = true;
            return new Disposable(() => isTargetUpdateDisabled = false);
        }

        private static object GetRelativeSource(DependencyObject target, RelativeSource relativeSource, string elementName)
        {
            if (!elementName.IsNullOrEmpty())
            {
                INameScope nameScope = NameScope.GetContainingNameScope(target);
                return nameScope != null ? nameScope.FindName(elementName) : null;
            }

            if (relativeSource == null || relativeSource.Mode == RelativeSourceMode.Self)
            {
                return target;
            }

            if (relativeSource.Mode == RelativeSourceMode.TemplatedParent)
            {
                return target is FrameworkElement ? ((FrameworkElement)target).TemplatedParent : null;
            }

            if (relativeSource.Mode == RelativeSourceMode.FindAncestor)
            {
                if (!(target is Visual))
                {
                    return null;
                }

                Visual visual = (target as Visual).VisualParent;
                int level = relativeSource.AncestorLevel - 1;

                while (visual != null && (level > 0 || relativeSource.AncestorType != null && !relativeSource.AncestorType.IsInstanceOfType(visual)))
                {
                    if (relativeSource.AncestorType == null || relativeSource.AncestorType.IsInstanceOfType(visual))
                    {
                        level--;
                    }

                    visual = visual.VisualParent;
                }

                return visual;
            }

            throw new Granular.Exception("RelativeSourceMode \"{0}\" is unexpected", relativeSource.Mode);
        }

        private static BindingMode GetDefaultBindingMode(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            FrameworkPropertyMetadata frameworkPropertyMetadata = dependencyProperty.GetMetadata(dependencyObject.GetType()) as FrameworkPropertyMetadata;
            return frameworkPropertyMetadata != null && frameworkPropertyMetadata.BindsTwoWayByDefault ? BindingMode.TwoWay : BindingMode.OneWay;
        }
    }
}
