using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using Granular.Collections;
using Granular.Extensions;
using System.Windows.Media;

namespace System.Windows.Controls
{
    public interface IRadioButtonGroupScope
    {
        ISelectionGroup<RadioButton> GetRadioButtonGroup(string groupName);
    }

    public class RadioButton : ToggleButton
    {
        private static readonly DependencyProperty SelectionGroupProperty = DependencyProperty.RegisterAttached("SelectionGroup", typeof(ISelectionGroup<RadioButton>), typeof(RadioButton), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register("GroupName", typeof(string), typeof(RadioButton), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((RadioButton)sender).OnGroupNameChanged(e)));
        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        private ISelectionGroup<RadioButton> currentGroup;
        private ISelectionGroup<RadioButton> CurrentGroup
        {
            get { return currentGroup; }
            set
            {
                if (currentGroup == value)
                {
                    return;
                }

                if (currentGroup != null)
                {
                    if (currentGroup.Selection == this)
                    {
                        currentGroup.Selection = null;
                    }

                    currentGroup.SelectionChanged -= OnCurrentSelectionGroupChanged;
                }

                currentGroup = value;

                if (currentGroup != null)
                {
                    currentGroup.SelectionChanged += OnCurrentSelectionGroupChanged;

                    if (currentGroup.Selection == null && this.IsChecked == true)
                    {
                        currentGroup.Selection = this;
                    }
                    else
                    {
                        this.IsChecked = currentGroup.Selection == this;
                    }
                }
            }
        }

        public RadioButton()
        {
            //
        }

        protected override void ToggleState()
        {
            IsChecked = true;
        }

        protected override void OnVisualAncestorChanged()
        {
            base.OnVisualAncestorChanged();

            SetCurrentGroup();
        }

        private void OnGroupNameChanged(DependencyPropertyChangedEventArgs e)
        {
            SetCurrentGroup();
        }

        private void SetCurrentGroup()
        {
            CurrentGroup = !GroupName.IsNullOrEmpty() ? GetGroup(GroupName) : GetDefaultGroup();
        }

        protected override void OnIsCheckedChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsCheckedChanged(e);

            if (IsChecked == true && CurrentGroup != null)
            {
                CurrentGroup.Selection = this;
            }
        }

        private void OnCurrentSelectionGroupChanged(object sender, EventArgs e)
        {
            IsChecked = CurrentGroup.Selection == this;
        }

        private ISelectionGroup<RadioButton> GetGroup(string name)
        {
            IRadioButtonGroupScope scope = GetSelectionGroupScope(this);
            return scope != null ? scope.GetRadioButtonGroup(name) : null;
        }

        private ISelectionGroup<RadioButton> GetDefaultGroup()
        {
            if (LogicalParent == null)
            {
                return null;
            }

            ISelectionGroup<RadioButton> group = (ISelectionGroup<RadioButton>)LogicalParent.GetValue(SelectionGroupProperty);

            if (group == null)
            {
                group = new SelectionGroup<RadioButton>();
                LogicalParent.SetValue(SelectionGroupProperty, group);
            }

            return group;
        }

        private static IRadioButtonGroupScope GetSelectionGroupScope(Visual visual)
        {
            while (visual != null)
            {
                if (visual is IRadioButtonGroupScope)
                {
                    return (IRadioButtonGroupScope)visual;
                }

                visual = visual.VisualParent;
            }

            return null;
        }
    }
}
