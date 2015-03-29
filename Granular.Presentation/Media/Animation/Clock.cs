using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media.Animation
{
    public enum ClockProgressState
    {
        BeforeStarted,
        Active,
        AfterEnded
    }

    public interface IClock
    {
        TimeSpan FirstTick { get; }
        TimeSpan LastTick { get; }
        TimeSpan Duration { get; }

        ClockState Tick(TimeSpan time);
    }

    public class ClockState
    {
        public static readonly ClockState Empty = new ClockState(ClockProgressState.BeforeStarted, 0, 0, Granular.Compatibility.TimeSpan.MinValue, Granular.Compatibility.TimeSpan.MaxValue);

        public ClockProgressState ProgressState { get; private set; }

        public double Progress { get; private set; }
        public double Iteration { get; private set; }

        public TimeSpan PreviousTick { get; private set; }
        public TimeSpan NextTick { get; private set; }

        public ClockState(ClockProgressState progressState, double progress, double iteration, TimeSpan previousTick, TimeSpan nextTick)
        {
            this.ProgressState = progressState;
            this.Progress = progress;
            this.Iteration = iteration;
            this.PreviousTick = previousTick;
            this.NextTick = nextTick;
        }
    }
}
