using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace System.Windows
{
    public class VisualStateManager
    {
        public static readonly DependencyProperty VisualStateGroupsProperty = DependencyProperty.RegisterAttached("VisualStateGroups", typeof(FreezableCollection<VisualStateGroup>), typeof(VisualStateManager), new FrameworkPropertyMetadata());

        public static FreezableCollection<VisualStateGroup> GetVisualStateGroups(DependencyObject obj)
        {
            return (FreezableCollection<VisualStateGroup>)obj.GetValue(VisualStateGroupsProperty);
        }

        public static void SetVisualStateGroups(DependencyObject obj, FreezableCollection<VisualStateGroup> value)
        {
            obj.SetValue(VisualStateGroupsProperty, value);
        }

        public static bool GoToState(Control control, string stateName, bool useTransitions)
        {
            if (control.TemplateChild == null)
            {
                return false;
            }

            IEnumerable<VisualStateGroup> visualStateGroups = GetVisualStateGroups(control.TemplateChild);
            if (visualStateGroups == null)
            {
                return false;
            }

            VisualStateGroup visualStateGroup = visualStateGroups.FirstOrDefault(group => group.States.Any(state => state.Name == stateName));
            if (visualStateGroup == null)
            {
                return false;
            }

            VisualState visualState = visualStateGroup.States.First(state => state.Name == stateName);

            visualStateGroup.SetContainer(control);
            return visualStateGroup.GoToState(visualState, useTransitions);
        }
    }
}
