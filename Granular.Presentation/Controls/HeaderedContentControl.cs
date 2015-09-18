using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Controls
{
    public class HeaderedContentControl : ContentControl
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(HeaderedContentControl), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((HeaderedContentControl)sender).OnHeaderChanged(e)));
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(HeaderedContentControl), new FrameworkPropertyMetadata());
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateSelectorProperty = DependencyProperty.Register("HeaderTemplateSelector", typeof(IDataTemplateSelector), typeof(HeaderedContentControl), new FrameworkPropertyMetadata());
        public IDataTemplateSelector HeaderTemplateSelector
        {
            get { return (IDataTemplateSelector)GetValue(HeaderTemplateSelectorProperty); }
            set { SetValue(HeaderTemplateSelectorProperty, value); }
        }

        private static readonly DependencyPropertyKey HasHeaderPropertyKey = DependencyProperty.RegisterReadOnly("HasHeader", typeof(bool), typeof(HeaderedContentControl), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty HasHeaderProperty = HasHeaderPropertyKey.DependencyProperty;
        public bool HasHeader
        {
            get { return (bool)GetValue(HasHeaderPropertyKey); }
            private set { SetValue(HasHeaderPropertyKey, value); }
        }

        private void OnHeaderChanged(DependencyPropertyChangedEventArgs e)
        {
            HasHeader = Header != null;
            RemoveLogicalChild(e.OldValue);
            AddLogicalChild(e.NewValue);
        }
    }
}
