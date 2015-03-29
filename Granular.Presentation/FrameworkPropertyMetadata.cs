using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace System.Windows
{
    public class FrameworkPropertyMetadata : PropertyMetadata
    {
        public bool AffectsMeasure { get; set; }
        public bool AffectsArrange { get; set; }
        public bool BindsTwoWayByDefault { get; set; }
        public UpdateSourceTrigger DefaultUpdateSourceTrigger { get; set; }

        public FrameworkPropertyMetadata(object defaultValue = null, PropertyChangedCallback propertyChangedCallback = null, CoerceValueCallback coerceValueCallback = null, bool inherits = false,
            bool affectsMeasure = false, bool affectsArrange = false, bool bindsTwoWayByDefault = false, UpdateSourceTrigger defaultUpdateSourceTrigger = UpdateSourceTrigger.Default) :
            base(defaultValue, propertyChangedCallback, coerceValueCallback, inherits)
        {
            this.AffectsMeasure = affectsMeasure;
            this.AffectsArrange = affectsArrange;
            this.BindsTwoWayByDefault = bindsTwoWayByDefault;
            this.DefaultUpdateSourceTrigger = defaultUpdateSourceTrigger;
        }
    }
}
