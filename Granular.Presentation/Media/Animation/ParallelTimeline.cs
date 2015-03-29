using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media.Animation
{
    public class ParallelTimeline : TimelineGroup
    {
        public override TimelineGroupClock CreateGroupClock(IEnumerable<TimelineClock> children)
        {
            return new ParallelTimelineClock(this, children);
        }
    }
}
