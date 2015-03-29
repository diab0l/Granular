using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Granular.Collections;

namespace System.Windows.Media.Animation
{
    [ContentProperty("Children")]
    public abstract class TimelineGroup : Timeline
    {
        public ObservableCollection<Timeline> Children { get; private set; }

        static TimelineGroup()
        {
            Timeline.DurationProperty.OverrideMetadata(typeof(TimelineGroup), new FrameworkPropertyMetadata(Duration.Automatic));
        }

        public TimelineGroup()
        {
            Children = new ObservableCollection<Timeline>();
            Children.CollectionChanged += OnChildrenCollectionChanged;
        }

        public sealed override TimelineClock CreateClock()
        {
            return CreateGroupClock(Children.Select(child => child.CreateClock()).ToArray());
        }

        public abstract TimelineGroupClock CreateGroupClock(IEnumerable<TimelineClock> children);

        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (Timeline timeline in e.OldItems)
            {
                timeline.Parent = null;
            }

            foreach (Timeline timeline in e.NewItems)
            {
                timeline.Parent = this;
            }
        }
    }
}
