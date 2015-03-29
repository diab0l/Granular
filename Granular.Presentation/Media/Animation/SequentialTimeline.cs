using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media.Animation
{
    public class SequentialTimeline : TimelineGroup
    {
        public override TimelineGroupClock CreateGroupClock(IEnumerable<TimelineClock> children)
        {
            return new SequentialTimelineClock(this, children);
        }
    }
}
