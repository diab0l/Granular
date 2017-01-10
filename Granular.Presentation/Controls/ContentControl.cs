using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace System.Windows.Controls
{
    [ContentProperty("Content")]
    public class ContentControl : Control, IItemContainer
    {
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(ContentControl), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((ContentControl)sender).OnContentChanged(e)));
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(ContentControl), new FrameworkPropertyMetadata());
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register("ContentTemplateSelector", typeof(IDataTemplateSelector), typeof(ContentControl), new FrameworkPropertyMetadata());
        public IDataTemplateSelector ContentTemplateSelector
        {
            get { return (IDataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
            set { SetValue(ContentTemplateSelectorProperty, value); }
        }

        private static readonly DependencyPropertyKey HasContentPropertyKey = DependencyProperty.RegisterReadOnly("HasContent", typeof(bool), typeof(ContentControl), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty HasContentProperty = HasContentPropertyKey.DependencyProperty;
        public bool HasContent
        {
            get { return (bool)GetValue(HasContentPropertyKey); }
            private set { SetValue(HasContentPropertyKey, value); }
        }

        private DataTemplate itemTemplate;
        private Style itemContainerStyle;

        static ContentControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ContentControl), new FrameworkPropertyMetadata(new StyleKey(typeof(ContentControl))));
        }

        public ContentControl()
        {
            //
        }

        private void OnContentChanged(DependencyPropertyChangedEventArgs e)
        {
            HasContent = Content != null;
            RemoveLogicalChild(e.OldValue);
            AddLogicalChild(e.NewValue);
        }

        public virtual void PrepareContainerForItem(object item, DataTemplate itemTemplate, Style itemContainerStyle)
        {
            if (!ContainsValue(ContentTemplateProperty) && !ContainsValue(ContentTemplateSelectorProperty) && itemTemplate != null)
            {
                ContentTemplate = itemTemplate;
                this.itemTemplate = itemTemplate;
            }

            if (!ContainsValue(StyleProperty) && itemContainerStyle != null)
            {
                Style = itemContainerStyle;
                this.itemContainerStyle = itemContainerStyle;
            }

            if (item != this)
            {
                Content = item;
            }
        }

        public virtual void ClearContainerForItem(object item)
        {
            if (itemTemplate == ContentTemplate)
            {
                ClearValue(ContentTemplateProperty);
                itemTemplate = null;
            }

            if (itemContainerStyle == Style)
            {
                ClearValue(StyleProperty);
                itemContainerStyle = null;
            }

            if (Content == item)
            {
                ClearValue(ContentProperty);
            }
        }
    }
}
