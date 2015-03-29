using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media.Animation
{
    public class ParallelTimelineClock : TimelineGroupClock
    {
        public ParallelTimelineClock(ParallelTimeline parallelTimeline, IEnumerable<TimelineClock> children) :
            base(new ParallelClock(children), parallelTimeline, children)
        {
            //
        }
    }
}
