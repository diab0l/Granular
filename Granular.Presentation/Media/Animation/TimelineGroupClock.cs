using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Media.Animation
{
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class TimelineGroupClock : TimelineClock
    {
        public IEnumerable<TimelineClock> Children { get; private set; }

        public TimelineGroupClock(IClock baseGroupClock, TimelineGroup timelineGroup, IEnumerable<TimelineClock> children) :
            base(baseGroupClock, timelineGroup)
        {
            this.Children = children;
        }
    }
}
