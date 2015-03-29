using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media.Animation
{
    public class SequentialTimelineClock : TimelineGroupClock
    {
        public SequentialTimelineClock(SequentialTimeline SequentialTimeline, IEnumerable<TimelineClock> children) :
            base(new SequentialClock(children), SequentialTimeline, children)
        {
            //
        }
    }
}