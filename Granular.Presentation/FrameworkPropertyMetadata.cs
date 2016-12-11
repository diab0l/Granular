using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace System.Windows
{
    public enum FrameworkPropertyMetadataOptions
    {
        None = 0,
        AffectsMeasure = 0x1,
        AffectsArrange = 0x2,
        //AffectsParentMeasure = 0x4,
        //AffectsParentArrange = 0x8,
        //AffectsRender = 0x10,
        Inherits = 0x20,
        //OverridesInheritanceBehavior = 0x40,
        //NotDataBindable = 0x80,
        BindsTwoWayByDefault = 0x100,
        //Journal = 0x200,
        //SubPropertiesDoNotAffectRender = 0x400,
        AffectsVisualState = 0x800,
    }

    public class FrameworkPropertyMetadata : PropertyMetadata
    {
        public bool AffectsArrange { get; set; }
        public bool AffectsMeasure { get; set; }
        public bool AffectsVisualState { get; set; }
        public bool BindsTwoWayByDefault { get; set; }
        public UpdateSourceTrigger DefaultUpdateSourceTrigger { get; set; }

        public FrameworkPropertyMetadata() :
            this(null, FrameworkPropertyMetadataOptions.None, null, null, false, UpdateSourceTrigger.Default)
        {
            //
        }

        public FrameworkPropertyMetadata(object defaultValue) :
            this(defaultValue, FrameworkPropertyMetadataOptions.None, null, null, false, UpdateSourceTrigger.Default)
        {
            //
        }

        public FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions flags) :
            this(null, flags, null, null, false, UpdateSourceTrigger.Default)
        {
            //
        }

        public FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback) :
            this(null, flags, propertyChangedCallback, null, false, UpdateSourceTrigger.Default)
        {
            //
        }

        public FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback) :
            this(null, flags, propertyChangedCallback, coerceValueCallback, false, UpdateSourceTrigger.Default)
        {
            //
        }

        public FrameworkPropertyMetadata(PropertyChangedCallback propertyChangedCallback) :
            this(null, FrameworkPropertyMetadataOptions.None, propertyChangedCallback, null, false, UpdateSourceTrigger.Default)
        {
            //
        }

        public FrameworkPropertyMetadata(PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback) :
            this(null, FrameworkPropertyMetadataOptions.None, propertyChangedCallback, coerceValueCallback, false, UpdateSourceTrigger.Default)
        {
            //
        }

        public FrameworkPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback) :
            this(defaultValue, FrameworkPropertyMetadataOptions.None, propertyChangedCallback, null, false, UpdateSourceTrigger.Default)
        {
            //
        }

        public FrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions flags) :
            this(defaultValue, flags, null, null, false, UpdateSourceTrigger.Default)
        {
            //
        }

        public FrameworkPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback) :
            this(defaultValue, FrameworkPropertyMetadataOptions.None, propertyChangedCallback, coerceValueCallback, false, UpdateSourceTrigger.Default)
        {
            //
        }

        public FrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback) :
            this(defaultValue, flags, propertyChangedCallback, null, false, UpdateSourceTrigger.Default)
        {
            //
        }

        public FrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback) :
            this(defaultValue, flags, propertyChangedCallback, coerceValueCallback, false, UpdateSourceTrigger.Default)
        {
            //
        }

        public FrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback, bool isAnimationProhibited, UpdateSourceTrigger defaultUpdateSourceTrigger) :
            base(defaultValue, propertyChangedCallback, coerceValueCallback)
        {
            this.AffectsArrange = (flags & FrameworkPropertyMetadataOptions.AffectsArrange) != 0;
            this.AffectsMeasure = (flags & FrameworkPropertyMetadataOptions.AffectsMeasure) != 0;
            this.AffectsVisualState = (flags & FrameworkPropertyMetadataOptions.AffectsVisualState) != 0;
            this.BindsTwoWayByDefault = (flags & FrameworkPropertyMetadataOptions.BindsTwoWayByDefault) != 0;
            this.Inherits = (flags & FrameworkPropertyMetadataOptions.Inherits) != 0;
            this.DefaultUpdateSourceTrigger = defaultUpdateSourceTrigger;
        }
    }
}