using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace System.Windows.Controls
{
    public class ControlPropertyMetadata : FrameworkPropertyMetadata
    {
        public bool AffectsVisualState { get; set; } // Control.UpdateVisualState will be called on changes

        public ControlPropertyMetadata(object defaultValue = null, PropertyChangedCallback propertyChangedCallback = null, CoerceValueCallback coerceValueCallback = null, bool inherits = false,
            bool affectsMeasure = false, bool affectsArrange = false, bool affectsVisualState = false, bool bindsTwoWayByDefault = true, UpdateSourceTrigger defaultUpdateSourceTrigger = UpdateSourceTrigger.Default) :
            base(defaultValue, propertyChangedCallback, coerceValueCallback, inherits, affectsMeasure, affectsArrange, bindsTwoWayByDefault, defaultUpdateSourceTrigger)
        {
            this.AffectsVisualState = affectsVisualState;
        }
    }
}
