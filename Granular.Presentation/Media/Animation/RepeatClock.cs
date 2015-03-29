using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace System.Windows.Media.Animation
{
    // repeat inner clock's duration a specific number of times
    public class RepeatClock : IClock
    {
        public TimeSpan FirstTick { get; private set; }
        public TimeSpan LastTick { get; private set; }
        public TimeSpan Duration { get; private set; }

        private IClock clock;
        private TimeSpan iterationDuration;
        private double iterationsCount;

        public RepeatClock(IClock clock, double iterationsCount)
        {
            this.clock = clock;
            this.iterationDuration = clock.Duration;
            this.iterationsCount = iterationsCount;
            this.Duration = Granular.Compatibility.Double.IsInfinity(iterationsCount) ? Granular.Compatibility.TimeSpan.MaxValue : iterationDuration.Scale(iterationsCount);

            if (iterationsCount <= 0)
            {
                throw new Granular.Exception("Invalid iterations count \"{0}\"", iterationsCount);
            }

            if (iterationDuration <= TimeSpan.Zero)
            {
                throw new Granular.Exception("Invalid iteration duration \"{0}\"", iterationDuration);
            }

            SetTickBounds();
        }

        private void SetTickBounds()
        {
            if (Granular.Compatibility.Double.IsInfinity(iterationsCount))
            {
                this.FirstTick = TimeSpan.Zero;
                this.LastTick = Granular.Compatibility.TimeSpan.MaxValue;
                return;
            }

            TimeSpan totalIterationsTime = iterationDuration.Scale(iterationsCount);

            if (totalIterationsTime < clock.FirstTick)
            {
                this.FirstTick = TimeSpan.Zero;
                this.LastTick = totalIterationsTime;
            }
            else
            {
                this.FirstTick = clock.FirstTick;

                if (totalIterationsTime < clock.LastTick)
                {
                    this.LastTick = totalIterationsTime;
                }
                else if (totalIterationsTime < iterationDuration)
                {
                    this.LastTick = clock.LastTick;
                }
                else
                {
                    double lastIterationRemainder = iterationsCount - Math.Floor(iterationsCount);
                    TimeSpan lastIterationDuration = iterationDuration.Scale(lastIterationRemainder);

                    if (lastIterationDuration < clock.FirstTick)
                    {
                        this.LastTick = clock.LastTick + iterationDuration.Scale(Math.Floor(iterationsCount - 1));
                    }
                    else if (lastIterationDuration < clock.LastTick)
                    {
                        this.LastTick = totalIterationsTime;
                    }
                    else
                    {
                        this.LastTick = clock.LastTick + iterationDuration.Scale(Math.Floor(iterationsCount));
                    }
                }
            }
        }

        public ClockState Tick(TimeSpan time)
        {
            double iteration = time.Divide(iterationDuration);

            ClockProgressState progressState = time < FirstTick ? ClockProgressState.BeforeStarted : (time >= LastTick ? ClockProgressState.AfterEnded : ClockProgressState.Active);

            iteration = Math.Min(Math.Max(iteration, 0), iterationsCount);

            double iterationRemainder = iteration - Math.Floor(iteration);

            TimeSpan currentIterationStart;
            TimeSpan currentIterationTime;

            if (progressState == ClockProgressState.AfterEnded && iterationRemainder == 0)
            {
                currentIterationStart = iterationDuration.Scale(iteration - 1);
                currentIterationTime = iterationDuration;
            }
            else
            {
                currentIterationStart = iterationDuration.Scale(iteration - iterationRemainder);
                currentIterationTime = iterationDuration.Scale(iterationRemainder);
            }

            ClockState state = clock.Tick(currentIterationTime);

            TimeSpan previousTick;
            TimeSpan nextTick;

            if (time < FirstTick)
            {
                previousTick = Granular.Compatibility.TimeSpan.MinValue;
                nextTick = FirstTick;
            }
            else if (time >= LastTick)
            {
                previousTick = LastTick;
                nextTick = Granular.Compatibility.TimeSpan.MaxValue;
            }
            else
            {
                if (currentIterationTime > clock.FirstTick || Math.Floor(iteration) == 0)
                {
                    previousTick = (currentIterationStart + state.PreviousTick).Max(FirstTick);
                }
                else
                {
                    previousTick = (currentIterationStart - iterationDuration + clock.LastTick).Max(FirstTick);
                }

                if (currentIterationTime < clock.LastTick || Math.Floor(iteration) == Math.Floor(iterationsCount))
                {
                    nextTick = (currentIterationStart + state.NextTick).Min(LastTick);
                }
                else
                {
                    nextTick = (currentIterationStart + iterationDuration + clock.FirstTick).Min(LastTick);
                }
            }

            if (progressState == ClockProgressState.Active)
            {
                progressState = state.ProgressState;
            }

            return new ClockState(progressState, state.Progress, iteration, previousTick, nextTick);
        }
    }
}
