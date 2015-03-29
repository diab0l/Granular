using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Granular.Extensions;

namespace System.Windows.Media.Animation
{
    public class AnimationLayer : IDisposable
    {
        private static readonly NamedObject UnsetValue = new NamedObject("AnimationLayer.UnsetValue");

        public event EventHandler ClockInvalidated;

        public bool HasValue { get { return clocks.Any(clock => clock.CurrentState.ProgressState == ClockProgressState.Active || clock.IsFilling); } }

        public bool IsEmpty { get { return !clocks.Any(); } }

        private IEnumerable<AnimationTimelineClock> clocks;

        private object snapshotValue;

        public AnimationLayer()
        {
            clocks = new AnimationTimelineClock[0];
            snapshotValue = UnsetValue;
        }

        public object GetValue(object defaultDestinationValue)
        {
            object defaultOriginValue = snapshotValue == UnsetValue ? defaultDestinationValue : snapshotValue;

            foreach (AnimationTimelineClock clock in clocks)
            {
                if (clock.CurrentState.ProgressState == ClockProgressState.Active || clock.IsFilling)
                {
                    defaultOriginValue = clock.GetValue(defaultOriginValue, defaultDestinationValue);
                }
            }

            return defaultOriginValue;
        }

        public void SetClocks(IEnumerable<AnimationTimelineClock> newClocks, object defaultDestinationValue)
        {
            DetachClocks(clocks);

            snapshotValue = newClocks.Any() ? GetValue(defaultDestinationValue) : UnsetValue;
            clocks = newClocks.ToArray();

            AttachClocks(clocks);
        }

        public void AddClocks(IEnumerable<AnimationTimelineClock> newClocks)
        {
            if (newClocks.Except(clocks).Count() != newClocks.Count())
            {
                throw new Granular.Exception("Can't add clocks that already exist on the animation layer");
            }

            AttachClocks(newClocks);

            clocks = clocks.Concat(newClocks).ToArray();
        }

        public void RemoveClocks(IEnumerable<AnimationTimelineClock> oldClocks)
        {
            if (oldClocks.Except(clocks).Count() != 0)
            {
                throw new Granular.Exception("Can't remove clocks that don't exist on the animation layer");
            }

            DetachClocks(oldClocks);

            clocks = clocks.Except(oldClocks).ToArray();
        }

        private void AttachClocks(IEnumerable<AnimationTimelineClock> newClocks)
        {
            foreach (AnimationTimelineClock clock in newClocks)
            {
                clock.Invalidated += OnClockInvalidated;
            }
        }

        private void DetachClocks(IEnumerable<AnimationTimelineClock> oldClocks)
        {
            foreach (AnimationTimelineClock clock in oldClocks)
            {
                clock.Invalidated -= OnClockInvalidated;
            }
        }

        private void OnClockInvalidated(object sender, EventArgs e)
        {
            ClockInvalidated.Raise(this);
        }

        public void Dispose()
        {
            DetachClocks(clocks);
        }
    }
}
