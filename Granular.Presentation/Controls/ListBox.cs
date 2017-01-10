using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Granular.Extensions;

namespace System.Windows.Controls
{
    public enum SelectionMode
    {
        Single,
        Multiple,
        Extended
    }

    public class ListBox : Selector
    {
        private interface ISelectionBehavior
        {
            void SetClickSelection(ListBoxItem item, ModifierKeys modifiers);
            void SetFocusChangeSelection(ListBoxItem item, ModifierKeys modifiers);
        }

        private class SingleSelectionBehavior : ISelectionBehavior
        {
            private ListBox listBox;

            public SingleSelectionBehavior(ListBox listBox)
            {
                this.listBox = listBox;
            }

            public void SetClickSelection(ListBoxItem item, ModifierKeys modifiers)
            {
                if (modifiers != ModifierKeys.Control)
                {
                    listBox.SetSingleSelection(item);
                }
                else
                {
                    listBox.SetSelectionAnchor(item);
                    listBox.ToggleSingleSelection(item);
                }
            }

            public void SetFocusChangeSelection(ListBoxItem item, ModifierKeys modifiers)
            {
                if (modifiers != ModifierKeys.Control)
                {
                    listBox.SetSingleSelection(item);
                }
            }
        }

        private class MultipleSelectionBehavior : ISelectionBehavior
        {
            private ListBox listBox;

            public MultipleSelectionBehavior(ListBox listBox)
            {
                this.listBox = listBox;
            }

            public void SetClickSelection(ListBoxItem item, ModifierKeys modifiers)
            {
                listBox.SetSelectionAnchor(item);
                listBox.ToggleSelection(item);
            }

            public void SetFocusChangeSelection(ListBoxItem item, ModifierKeys modifiers)
            {
                //
            }
        }

        private class ExtendedSelectionBehavior : ISelectionBehavior
        {
            private ListBox listBox;

            public ExtendedSelectionBehavior(ListBox listBox)
            {
                this.listBox = listBox;
            }

            public void SetClickSelection(ListBoxItem item, ModifierKeys modifiers)
            {
                if (modifiers == ModifierKeys.None)
                {
                    listBox.SetSelectionAnchor(item);
                    listBox.SetSingleSelection(item);
                }
                else if (modifiers == ModifierKeys.Shift)
                {
                    listBox.SetRangeSelection(item);
                }
                else if (modifiers == ModifierKeys.Control)
                {
                    listBox.SetSelectionAnchor(item);
                    listBox.ToggleSelection(item);
                }
            }

            public void SetFocusChangeSelection(ListBoxItem item, ModifierKeys modifiers)
            {
                if (modifiers == ModifierKeys.None)
                {
                    listBox.SetSelectionAnchor(item);
                    listBox.SetSingleSelection(item);
                }
                else if (modifiers == ModifierKeys.Shift)
                {
                    listBox.SetRangeSelection(item);
                }
            }
        }

        private static readonly DependencyPropertyKey SelectedItemsPropertyKey = DependencyProperty.RegisterReadOnly("SelectedItems", typeof(IEnumerable<object>), typeof(ListBox), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty SelectedItemsProperty = SelectedItemsPropertyKey.DependencyProperty;
        public IEnumerable<object> SelectedItems
        {
            get { return (IEnumerable<object>)GetValue(SelectedItemsPropertyKey); }
            private set { SetValue(SelectedItemsPropertyKey, value); }
        }

        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(ListBox), new FrameworkPropertyMetadata(SelectionMode.Single, (sender, e) => ((ListBox)sender).SetSelectionBehavior()));
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        private ISelectionBehavior selectionBehavior;

        private ListBoxItem selectionAnchor;

        private bool isItemContainerBeingClicked;

        static ListBox()
        {
            Control.IsTabStopProperty.OverrideMetadata(typeof(ListBox), new FrameworkPropertyMetadata(false));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(ListBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(ListBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ListBox), new FrameworkPropertyMetadata(new StyleKey(typeof(ListBox))));
        }

        public ListBox()
        {
            SetSelectionBehavior();
        }

        //public void ScrollIntoView(object item);
        //public void SelectAll();
        //protected bool SetSelectedItems(IEnumerable selectedItems);
        //public void UnselectAll();

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ListBoxItem;
        }

        protected override FrameworkElement GetContainerForItemOverride()
        {
            return new ListBoxItem();
        }

        protected override void OnPrepareContainerForItem(object item, FrameworkElement container)
        {
            container.PreviewMouseDown += OnItemContainerPreviewMouseDown; // handled too
            container.MouseDown += OnItemContainerMouseDown;
            container.KeyDown += OnItemContainerKeyDown;
            container.GotKeyboardFocus += OnItemContainerGotKeyboardFocus;
        }

        protected override void OnClearContainerForItem(object item, FrameworkElement container)
        {
            container.PreviewMouseDown -= OnItemContainerPreviewMouseDown; // handled too
            container.MouseDown -= OnItemContainerMouseDown;
            container.KeyDown -= OnItemContainerKeyDown;
            container.GotKeyboardFocus -= OnItemContainerGotKeyboardFocus;
        }

        private void OnItemContainerPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right)
            {
                isItemContainerBeingClicked = true;
            }
        }

        private void OnItemContainerMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right)
            {
                selectionBehavior.SetClickSelection((ListBoxItem)sender, ApplicationHost.Current.GetKeyboardDeviceFromElement(this).Modifiers);
                isItemContainerBeingClicked = false;
            }
        }

        private void OnItemContainerKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                selectionBehavior.SetClickSelection((ListBoxItem)sender, e.KeyboardDevice.Modifiers);
            }
        }

        private void OnItemContainerGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!isItemContainerBeingClicked)
            {
                selectionBehavior.SetFocusChangeSelection((ListBoxItem)sender, e.KeyboardDevice.Modifiers);
            }
        }

        private void SetSelectionBehavior()
        {
            selectionBehavior = CreateSelectionBehavior(this, SelectionMode);
        }

        private static ISelectionBehavior CreateSelectionBehavior(ListBox listBox, SelectionMode selectionMode)
        {
            switch (selectionMode)
            {
                case SelectionMode.Single: return new SingleSelectionBehavior(listBox);
                case SelectionMode.Multiple: return new MultipleSelectionBehavior(listBox);
                case SelectionMode.Extended: return new ExtendedSelectionBehavior(listBox);
            }

            throw new Granular.Exception("Unexpected SelectionMode \"{0}\"", selectionMode);
        }

        private void SetSelectionAnchor(ListBoxItem item)
        {
            selectionAnchor = item;
        }

        private void SetSingleSelection(ListBoxItem item)
        {
            SelectedItem = ItemContainerGenerator.ItemFromContainer(item);

            for (int i = 0; i < ItemContainerGenerator.ItemsCount; i++)
            {
                DependencyObject itemContainer = ItemContainerGenerator.Generate(i);
                itemContainer.SetCurrentValue(Selector.IsSelectedProperty, itemContainer == item);
            }
        }

        private void SetRangeSelection(ListBoxItem item)
        {
            int itemIndex = ItemContainerGenerator.IndexFromContainer(item);
            int selectionAnchorIndex = ItemContainerGenerator.IndexFromContainer(selectionAnchor);

            int rangeStartIndex = itemIndex.Min(selectionAnchorIndex);
            int rangeEndIndex = itemIndex.Max(selectionAnchorIndex);

            for (int i = 0; i < ItemContainerGenerator.ItemsCount; i++)
            {
                DependencyObject itemContainer = ItemContainerGenerator.Generate(i);
                itemContainer.SetCurrentValue(Selector.IsSelectedProperty, rangeStartIndex <= i && i <= rangeEndIndex);
            }
        }

        private void ToggleSelection(ListBoxItem item)
        {
            item.SetCurrentValue(Selector.IsSelectedProperty, !(bool)item.GetValue(Selector.IsSelectedProperty));
        }

        private void ToggleSingleSelection(ListBoxItem item)
        {
            bool isSelected = !(bool)item.GetValue(Selector.IsSelectedProperty);

            SelectedItem = isSelected ? ItemContainerGenerator.ItemFromContainer(item) : null;

            for (int i = 0; i < ItemContainerGenerator.ItemsCount; i++)
            {
                DependencyObject itemContainer = ItemContainerGenerator.Generate(i);
                itemContainer.SetCurrentValue(Selector.IsSelectedProperty, itemContainer == item && isSelected);
            }
        }
    }
}
