using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Granular.Extensions;

namespace System.Windows.Media.Animation
{
    // dependency property animated value, contains animation layers with clocks
    public class AnimationExpression : IExpression
    {
        public event EventHandler<ObservableValueChangedArgs> ValueChanged;
        public object Value { get { return observableValue.Value; } }

        private AnimationLayerCollection layers;
        private ObservableValue observableValue;
        private DependencyObject dependencyObject;
        private DependencyProperty dependencyProperty;

        public AnimationExpression(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            this.dependencyObject = dependencyObject;
            this.dependencyProperty = dependencyProperty;

            observableValue = new ObservableValue();
            observableValue.ValueChanged += (sender, e) => ValueChanged.Raise(this, e);

            layers = new AnimationLayerCollection();
            layers.LayerInvalidated += OnLayerInvalidated;

            SetAnimationValue();
        }

        public bool SetValue(object value)
        {
            return false;
        }

        public void SetClocks(IEnumerable<AnimationTimelineClock> clocks, object layerOwner)
        {
            layers.SetClocks(clocks, GetAnimationBaseValue(), AnimationLayerKey.FromLayerOwner(layerOwner));
        }

        public void AddClocks(IEnumerable<AnimationTimelineClock> clocks, object layerOwner)
        {
            layers.AddClocks(clocks, AnimationLayerKey.FromLayerOwner(layerOwner));
        }

        public void RemoveClocks(IEnumerable<AnimationTimelineClock> clocks, object layerOwner)
        {
            layers.RemoveClocks(clocks, AnimationLayerKey.FromLayerOwner(layerOwner));
        }

        private void SetAnimationValue()
        {
            observableValue.Value = layers.HasValue ? layers.GetValue(GetAnimationBaseValue()) : ObservableValue.UnsetValue;
        }

        private object GetAnimationBaseValue()
        {
            if (dependencyObject == null)
            {
                return ObservableValue.UnsetValue;
            }

            IDependencyPropertyValueEntry entry = dependencyObject.GetValueEntry(dependencyProperty);
            return entry.GetBaseValue(true);
        }

        private void OnLayerInvalidated(object sender, EventArgs e)
        {
            SetAnimationValue();
        }
    }
}
