using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows
{
    public delegate void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e);
    public delegate object CoerceValueCallback(DependencyObject dependencyObject, object value);

    public class PropertyMetadata
    {
        public object DefaultValue { get; set; }
        public PropertyChangedCallback PropertyChangedCallback { get; set; }
        public CoerceValueCallback CoerceValueCallback { get; set; }
        public bool Inherits { get; set; }

        public PropertyMetadata() :
            this(null, null, null)
        {
            //
        }

        public PropertyMetadata(object defaultValue) :
            this(defaultValue, null, null)
        {
            //
        }

        public PropertyMetadata(PropertyChangedCallback propertyChangedCallback) :
            this(null, propertyChangedCallback, null)
        {
            //
        }

        public PropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback) :
            this(defaultValue, propertyChangedCallback, null)
        {
            //
        }

        public PropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
        {
            this.DefaultValue = defaultValue;
            this.PropertyChangedCallback = propertyChangedCallback;
            this.CoerceValueCallback = coerceValueCallback;
        }
    }
}
