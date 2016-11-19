using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Markup;
using Granular.Collections;
using Granular.Extensions;

namespace System.Windows.Data
{
    public interface IPropertyObserver : IObservableValue
    {
        Type ValueType { get; }
        bool TrySetValue(object value);
        void SetBaseValue(object baseValue);
    }

    public class ClrPropertyObserver : IPropertyObserver, IDisposable
    {
        public event ObservableValueChangedEventHandler ValueChanged;

        public object Value { get { return observableValue.Value; } }
        public Type ValueType { get { return propertyInfo.PropertyType; } }

        private ObservableValue observableValue;
        private PropertyInfo propertyInfo;
        private MethodInfo propertyGetMethod;
        private MethodInfo propertySetMethod;
        private IEnumerable<object> index;
        private INotifyPropertyChanged currentPropertyNotifier;
        private INotifyCollectionChanged currentCollectionNotifier;
        private object baseValue;

        public ClrPropertyObserver(PropertyInfo propertyInfo, IEnumerable<object> index)
        {
            this.propertyInfo = propertyInfo;
            this.propertyGetMethod = propertyInfo.GetGetMethod();
            this.propertySetMethod = propertyInfo.GetSetMethod();
            this.index = index;

            observableValue = new ObservableValue();
            observableValue.ValueChanged += (sender, oldValue, newValue) => ValueChanged.Raise(this, oldValue, newValue);
        }

        public void SetBaseValue(object baseValue)
        {
            this.baseValue = baseValue;
            RegisterNotifiers();
            observableValue.BaseValue = GetValue();
        }

        public bool TrySetValue(object value)
        {
            if (ObservableValue.IsNullOrUnset(baseValue) || propertySetMethod == null)
            {
                return false;
            }

            propertySetMethod.Invoke(baseValue, index.Concat(new object[] { value }).ToArray());
            observableValue.BaseValue = GetValue();
            return true;
        }

        private object GetValue()
        {
            if (ObservableValue.IsNullOrUnset(baseValue) || propertyGetMethod == null)
            {
                return ObservableValue.UnsetValue;
            }

            return propertyGetMethod.Invoke(baseValue, index.ToArray());
        }

        private void RegisterNotifiers()
        {
            if (currentPropertyNotifier != null)
            {
                currentPropertyNotifier.PropertyChanged -= OnNotifierPropertyChanged;
            }

            if (currentCollectionNotifier != null)
            {
                currentCollectionNotifier.CollectionChanged -= OnNotifierCollectionChanged;
            }

            currentPropertyNotifier = baseValue as INotifyPropertyChanged;

            if (index.Any())
            {
                currentCollectionNotifier = baseValue as INotifyCollectionChanged;
            }

            if (currentPropertyNotifier != null)
            {
                currentPropertyNotifier.PropertyChanged += OnNotifierPropertyChanged;
            }

            if (currentCollectionNotifier != null)
            {
                currentCollectionNotifier.CollectionChanged += OnNotifierCollectionChanged;
            }
        }

        private void OnNotifierPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == propertyInfo.Name)
            {
                observableValue.BaseValue = GetValue();
            }
        }

        private void OnNotifierCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            observableValue.BaseValue = GetValue();
        }

        public void Dispose()
        {
            if (currentPropertyNotifier != null)
            {
                currentPropertyNotifier.PropertyChanged -= OnNotifierPropertyChanged;
            }
        }
    }

    public class DependencyPropertyObserver : IPropertyObserver, IDisposable
    {
        public event ObservableValueChangedEventHandler ValueChanged;

        public object Value { get { return observableValue.Value; } }
        public Type ValueType { get { return dependencyProperty.PropertyType; } }

        private ObservableValue observableValue;
        private DependencyProperty dependencyProperty;
        private DependencyObject currentDependencyObject;
        private object baseValue;

        public DependencyPropertyObserver(DependencyProperty dependencyProperty)
        {
            this.dependencyProperty = dependencyProperty;

            observableValue = new ObservableValue();
            observableValue.ValueChanged += (sender, oldValue, newValue) => ValueChanged.Raise(this, oldValue, newValue);
        }

        public void SetBaseValue(object baseValue)
        {
            this.baseValue = baseValue;
            RegisterDependencyObject();
            observableValue.BaseValue = GetValue();
        }

        public bool TrySetValue(object value)
        {
            DependencyObject dependencyObject = baseValue as DependencyObject;

            if (dependencyObject == null || dependencyProperty.IsReadOnly)
            {
                return false;
            }

            dependencyObject.SetValue(dependencyProperty, value);
            observableValue.BaseValue = GetValue();
            return true;
        }

        private object GetValue()
        {
            DependencyObject dependencyObject = baseValue as DependencyObject;
            return dependencyObject != null ? dependencyObject.GetValue(dependencyProperty) : ObservableValue.UnsetValue;
        }

        private void RegisterDependencyObject()
        {
            if (currentDependencyObject != null)
            {
                currentDependencyObject.PropertyChanged -= OnDependencyObjectPropertyChanged;
            }

            currentDependencyObject = baseValue as DependencyObject;

            if (currentDependencyObject != null)
            {
                currentDependencyObject.PropertyChanged += OnDependencyObjectPropertyChanged;
            }
        }

        private void OnDependencyObjectPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == dependencyProperty)
            {
                observableValue.BaseValue = GetValue();
            }
        }

        public void Dispose()
        {
            if (currentDependencyObject != null)
            {
                currentDependencyObject.PropertyChanged -= OnDependencyObjectPropertyChanged;
            }
        }
    }

    public class IndexPropertyObserver : IPropertyObserver, IDisposable
    {
        public event ObservableValueChangedEventHandler ValueChanged;

        public object Value { get { return indexerObserver.Value; } }
        public Type ValueType { get { return indexerObserver.ValueType; } }

        private IPropertyObserver baseObserver;
        private IPropertyObserver indexerObserver;

        public IndexPropertyObserver(Type propertyContainingType, IndexPropertyPathElement propertyPathElement, XamlNamespaces namespaces)
        {
            baseObserver = CreateBaseObserver(propertyContainingType, propertyPathElement.PropertyName);

            PropertyInfo indexPropertyInfo = baseObserver != null ? baseObserver.ValueType.GetDefaultIndexProperty() :
                propertyPathElement.PropertyName.IsEmpty ? propertyContainingType.GetDefaultIndexProperty() : propertyContainingType.GetInstanceProperty(propertyPathElement.PropertyName.MemberName);

            if (indexPropertyInfo == null)
            {
                throw new Granular.Exception("Property \"{0}.{1}\" does not have an indexer", propertyContainingType.Name, propertyPathElement.PropertyName.MemberName);
            }

            if (indexPropertyInfo.GetIndexParameters().Count() != propertyPathElement.IndexRawValues.Count())
            {
                throw new Granular.Exception("Invalid number of index parameters for \"{0}.{1}\"", indexPropertyInfo.DeclaringType.Name, indexPropertyInfo.Name);
            }

            indexerObserver = new ClrPropertyObserver(indexPropertyInfo, propertyPathElement.ParseIndexValues(indexPropertyInfo));
            indexerObserver.ValueChanged += (sender, oldValue, newValue) => ValueChanged.Raise(this, oldValue, newValue);

            if (baseObserver != null)
            {
                baseObserver.ValueChanged += (sender, oldValue, newValue) => indexerObserver.SetBaseValue(baseObserver.Value);
                indexerObserver.SetBaseValue(baseObserver.Value);
            }
        }

        public void Dispose()
        {
            if (baseObserver is IDisposable)
            {
                ((IDisposable)baseObserver).Dispose();
            }

            if (indexerObserver is IDisposable)
            {
                ((IDisposable)indexerObserver).Dispose();
            }
        }

        public bool TrySetValue(object value)
        {
            return indexerObserver != null ? indexerObserver.TrySetValue(value) : false;
        }

        public void SetBaseValue(object baseValue)
        {
            if (baseObserver != null)
            {
                baseObserver.SetBaseValue(baseValue);
            }
            else
            {
                indexerObserver.SetBaseValue(baseValue);
            }
        }

        private static IPropertyObserver CreateBaseObserver(Type propertyContainingType, XamlName propertyName)
        {
            if (propertyName.IsEmpty)
            {
                return null;
            }

            DependencyProperty dependencyProperty = DependencyProperty.GetProperty(propertyContainingType, propertyName);
            if (dependencyProperty != null)
            {
                return new DependencyPropertyObserver(dependencyProperty);
            }

            PropertyInfo propertyInfo = propertyContainingType.GetInstanceProperty(propertyName.MemberName);
            if (propertyInfo != null && !propertyInfo.GetIndexParameters().Any())
            {
                return new ClrPropertyObserver(propertyInfo, new object[0]);
            }

            return null;
        }
    }
}
