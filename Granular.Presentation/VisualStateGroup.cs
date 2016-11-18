using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using Granular.Extensions;

namespace System.Windows
{
    [ContentProperty("States")]
    [RuntimeNameProperty("Name")]
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class VisualStateGroup : Freezable
    {
        public string Name { get; set; }

        public FreezableCollection<VisualState> States { get; private set; }
        public FreezableCollection<VisualTransition> Transitions { get; private set; }

        public VisualState CurrentState { get; private set; }

        private Storyboard currentStoryboard;
        private FrameworkElement container;

        public VisualStateGroup()
        {
            States = new FreezableCollection<VisualState>();
            States.TrySetContextParent(this);

            Transitions = new FreezableCollection<VisualTransition>();
            Transitions.TrySetContextParent(this);
        }

        public void SetContainer(FrameworkElement container)
        {
            if (this.container != null && this.container != container)
            {
                throw new Granular.Exception("Can't change VisualStateGroup container");
            }

            this.container = container;
        }

        public bool GoToState(VisualState state, bool useTransitions)
        {
            if (state == CurrentState)
            {
                return true;
            }

            string currentStateName = CurrentState != null ? CurrentState.Name : String.Empty;
            VisualTransition transition = useTransitions ? GetTransition(Transitions, currentStateName, state.Name) : null;
            Storyboard transitionStoryboard = transition != null ? transition.Storyboard : null;

            Storyboard storyboard;
            if (transitionStoryboard != null && state.Storyboard != null)
            {
                // create a sequential animation with the transition storyboard first and then the state storyboard
                SequentialTimeline sequentialTimeline = new SequentialTimeline();
                sequentialTimeline.Children.Add(transitionStoryboard);
                sequentialTimeline.Children.Add(state.Storyboard);

                storyboard = new Storyboard();
                storyboard.Children.Add(sequentialTimeline);
            }
            else
            {
                storyboard = transitionStoryboard ?? state.Storyboard;
            }

            StartNewStoryboard(storyboard);

            CurrentState = state;
            return true;
        }

        private void StartNewStoryboard(Storyboard newStoryboard)
        {
            if (newStoryboard != null)
            {
                newStoryboard.Begin(container, NameScope.GetTemplateNameScope(container));

                if (currentStoryboard != null)
                {
                    currentStoryboard.Stop(container);
                }
            }
            else if (currentStoryboard != null)
            {
                currentStoryboard.Remove(container);
            }

            currentStoryboard = newStoryboard;
        }

        private static VisualTransition GetTransition(IEnumerable<VisualTransition> transitions, string fromStateName, string toStateName)
        {
            return transitions.FirstOrDefault(transition => transition.From == fromStateName && transition.To == toStateName) ??
                transitions.FirstOrDefault(transition => transition.From.IsNullOrEmpty() && transition.To == toStateName) ??
                transitions.FirstOrDefault(transition => transition.From == fromStateName && transition.To.IsNullOrEmpty()) ??
                transitions.FirstOrDefault(transition => transition.From.IsNullOrEmpty() && transition.To.IsNullOrEmpty());
        }
    }
}
