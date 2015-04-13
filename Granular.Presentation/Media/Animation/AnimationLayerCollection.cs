using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace System.Windows.Media.Animation
{
    public sealed class AnimationLayerKey
    {
        public static readonly AnimationLayerKey Default = new AnimationLayerKey(null);

        private object layerOwner;

        private AnimationLayerKey(object layerOwner)
        {
            this.layerOwner = layerOwner;
        }

        public override bool Equals(object obj)
        {
            AnimationLayerKey other = obj as AnimationLayerKey;

            return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
                Object.Equals(this.layerOwner, other.layerOwner);
        }

        public override int GetHashCode()
        {
            return layerOwner == null ? 0 : layerOwner.GetHashCode();
        }

        public static AnimationLayerKey FromLayerOwner(object layerOwner)
        {
            return layerOwner == null ? Default : new AnimationLayerKey(layerOwner);
        }
    }

    public class AnimationLayerCollection
    {
        public event EventHandler LayerInvalidated;

        public bool HasValue { get { return defaultLayer.HasValue || layers.Values.Any(layer => layer.HasValue); } }

        private AnimationLayer defaultLayer;
        private Dictionary<AnimationLayerKey, AnimationLayer> layers;

        public AnimationLayerCollection()
        {
            defaultLayer = new AnimationLayer();
            defaultLayer.ClockInvalidated += OnAnimationLayerClockInvalidated;

            layers = new Dictionary<AnimationLayerKey, AnimationLayer>();
        }

        public object GetValue(object defaultDestinationValue)
        {
            foreach (AnimationLayer layer in layers.Values)
            {
                defaultDestinationValue = layer.GetValue(defaultDestinationValue);
            }

            return defaultLayer.GetValue(defaultDestinationValue);
        }

        public void SetClocks(IEnumerable<AnimationTimelineClock> newClocks, object defaultDestinationValue, AnimationLayerKey key)
        {
            if (newClocks.Any() || key == AnimationLayerKey.Default)
            {
                GetAnimationLayer(key).SetClocks(newClocks, defaultDestinationValue);
            }
            else
            {
                RemoveAnimationLayer(key);
            }

            LayerInvalidated.Raise(this);
        }

        public void AddClocks(IEnumerable<AnimationTimelineClock> newClocks, AnimationLayerKey key)
        {
            GetAnimationLayer(key).AddClocks(newClocks);
            LayerInvalidated.Raise(this);
        }

        public void RemoveClocks(IEnumerable<AnimationTimelineClock> newClocks, AnimationLayerKey key)
        {
            AnimationLayer animationLayer = GetAnimationLayer(key);

            animationLayer.RemoveClocks(newClocks);

            if (key != AnimationLayerKey.Default && animationLayer.IsEmpty)
            {
                RemoveAnimationLayer(key);
            }

            LayerInvalidated.Raise(this);
        }

        private AnimationLayer GetAnimationLayer(AnimationLayerKey key)
        {
            if (key == AnimationLayerKey.Default)
            {
                return defaultLayer;
            }

            AnimationLayer layer;

            if (!layers.TryGetValue(key, out layer))
            {
                layer = new AnimationLayer();
                layer.ClockInvalidated += OnAnimationLayerClockInvalidated;

                layers.Add(key, layer);
            }

            return layer;
        }

        private void RemoveAnimationLayer(AnimationLayerKey key)
        {
            AnimationLayer layer;

            if (key == AnimationLayerKey.Default)
            {
                throw new Granular.Exception("Can't remove default animation layer");
            }

            if (layers.TryGetValue(key, out layer))
            {
                layer.ClockInvalidated -= OnAnimationLayerClockInvalidated;
                layer.Dispose();

                layers.Remove(key);
            }
        }

        private void OnAnimationLayerClockInvalidated(object sender, EventArgs e)
        {
            LayerInvalidated.Raise(this);
        }
    }
}
