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

        public PropertyMetadata(object defaultValue = null, PropertyChangedCallback propertyChangedCallback = null, CoerceValueCallback coerceValueCallback = null, bool inherits = false)
        {
            this.DefaultValue = defaultValue;
            this.PropertyChangedCallback = propertyChangedCallback;
            this.CoerceValueCallback = coerceValueCallback;
            this.Inherits = inherits;
        }
    }
}
