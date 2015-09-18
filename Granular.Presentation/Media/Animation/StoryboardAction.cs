using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace System.Windows.Media.Animation
{
    [RuntimeNameProperty("Name")]
    [ContentProperty("Storyboard")]
    public class BeginStoryboard : Freezable, ITriggerAction
    {
        public static readonly DependencyProperty StoryboardProperty = DependencyProperty.Register("Storyboard", typeof(Storyboard), typeof(BeginStoryboard), new FrameworkPropertyMetadata());
        public Storyboard Storyboard
        {
            get { return (Storyboard)GetValue(StoryboardProperty); }
            set { SetValue(StoryboardProperty, value); }
        }

        public HandoffBehavior HandoffBehavior { get; set; }

        public string Name { get; set; }

        public void Apply(FrameworkElement target, BaseValueSource valueSource)
        {
            INameScope nameScope = valueSource == BaseValueSource.Local ? NameScope.GetContainingNameScope(target) : NameScope.GetTemplateNameScope(target);
            object layerOwner = valueSource == BaseValueSource.Local ? null : target.TemplatedParent;
            Begin(target, nameScope, layerOwner);
        }

        public void Clean(FrameworkElement target, BaseValueSource valueSource)
        {
            //
        }

        public bool IsActionOverlaps(ITriggerAction action)
        {
            return false;
        }

        private void Begin(FrameworkElement target, INameScope nameScope, object layerOwner)
        {
            if (Storyboard != null)
            {
                Storyboard.Begin(target, nameScope, HandoffBehavior, layerOwner);
            }
        }

        public void Remove(FrameworkElement target, INameScope nameScope, object layerOwner)
        {
            if (Storyboard != null)
            {
                Storyboard.Remove(target, nameScope, layerOwner);
            }
        }

        public void Stop(FrameworkElement target)
        {
            if (Storyboard != null)
            {
                Storyboard.Stop(target);
            }
        }

        public void Pause(FrameworkElement target)
        {
            if (Storyboard != null)
            {
                Storyboard.Pause(target);
            }
        }

        public void Resume(FrameworkElement target)
        {
            if (Storyboard != null)
            {
                Storyboard.Pause(target);
            }
        }

        public void SkipToFill(FrameworkElement target)
        {
            if (Storyboard != null)
            {
                Storyboard.SkipToFill(target);
            }
        }
    }

    public abstract class StoryboardAction : Freezable, ITriggerAction
    {
        public string BeginStoryboardName { get; set; }

        public abstract void Apply(FrameworkElement target, BaseValueSource valueSource);

        public void Clean(FrameworkElement target, BaseValueSource valueSource)
        {
            //
        }

        public bool IsActionOverlaps(ITriggerAction action)
        {
            return false;
        }

        protected BeginStoryboard GetBeginStoryboard(FrameworkElement target)
        {
            INameScope nameScope = NameScope.GetContainingNameScope(target);
            return nameScope != null ? nameScope.FindName(BeginStoryboardName) as BeginStoryboard : null;
        }
    }

    public class RemoveStoryboard : StoryboardAction
    {
        public override void Apply(FrameworkElement target, BaseValueSource valueSource)
        {
            BeginStoryboard beginStoryboard = GetBeginStoryboard(target);

            if (beginStoryboard != null)
            {
                INameScope nameScope = valueSource == BaseValueSource.Local ? NameScope.GetContainingNameScope(target) : NameScope.GetTemplateNameScope(target);
                object layerOwner = valueSource == BaseValueSource.Local ? null : target.TemplatedParent;
                beginStoryboard.Remove(target, nameScope, layerOwner);
            }
        }
    }

    public class PauseStoryboard : StoryboardAction
    {
        public override void Apply(FrameworkElement target, BaseValueSource valueSource)
        {
            BeginStoryboard beginStoryboard = GetBeginStoryboard(target);

            if (beginStoryboard != null)
            {
                beginStoryboard.Pause(target);
            }
        }
    }

    public class ResumeStoryboard : StoryboardAction
    {
        public override void Apply(FrameworkElement target, BaseValueSource valueSource)
        {
            BeginStoryboard beginStoryboard = GetBeginStoryboard(target);

            if (beginStoryboard != null)
            {
                beginStoryboard.Resume(target);
            }
        }
    }

    public class SkipToFillStoryboard : StoryboardAction
    {
        public override void Apply(FrameworkElement target, BaseValueSource valueSource)
        {
            BeginStoryboard beginStoryboard = GetBeginStoryboard(target);

            if (beginStoryboard != null)
            {
                beginStoryboard.SkipToFill(target);
            }
        }
    }

    public class StopStoryboard : StoryboardAction
    {
        public override void Apply(FrameworkElement target, BaseValueSource valueSource)
        {
            BeginStoryboard beginStoryboard = GetBeginStoryboard(target);

            if (beginStoryboard != null)
            {
                beginStoryboard.Stop(target);
            }
        }
    }
}
