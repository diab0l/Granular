using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Granular.Extensions;

namespace System.Windows.Data
{
    [MarkupExtensionParameter("Path")]
    public class Binding : IMarkupExtension, IExpressionProvider
    {
        private class StringFormatConverter : IValueConverter
        {
            private string format;

            public StringFormatConverter(string format)
            {
                this.format = format;
            }

            public object Convert(object value, Type targetType, object parameter)
            {
                return String.Format(format, value, parameter);
            }

            public object ConvertBack(object value, Type targetType, object parameter)
            {
                return null;
            }
        }

        public PropertyPath Path { get; set; }
        public object Source { get; set; }
        public RelativeSource RelativeSource { get; set; }
        public string ElementName { get; set; }

        public BindingMode Mode { get; set; }
        public UpdateSourceTrigger UpdateSourceTrigger { get; set; }

        public string StringFormat { get; set; }
        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }

        public object FallbackValue { get; set; }
        public object TargetNullValue { get; set; }

        public object ProvideValue(InitializeContext context)
        {
            return this;
        }

        public IExpression CreateExpression(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            return new BindingExpression(
                target: dependencyObject,
                targetProperty: dependencyProperty,
                path: GetDataContextRelativePath(Path ?? PropertyPath.Empty, Source, RelativeSource, ElementName),
                source: Source,
                relativeSource: RelativeSource,
                elementName: ElementName,
                mode: Mode,
                updateSourceTrigger: UpdateSourceTrigger,
                converter: Converter ?? GetStringFormatConverter(StringFormat),
                converterParameter: ConverterParameter,
                fallbackValue: FallbackValue,
                targetNullValue: TargetNullValue);
        }

        private static PropertyPath GetDataContextRelativePath(PropertyPath path, object source, RelativeSource relativeSource, string elementName)
        {
            if (source != null || relativeSource != null || !elementName.IsNullOrEmpty())
            {
                return path;
            }

            return path.Insert(0, new DependencyPropertyPathElement(FrameworkElement.DataContextProperty));
        }

        private static IValueConverter GetStringFormatConverter(string format)
        {
            return !format.IsNullOrEmpty() ? new StringFormatConverter(format) : null;
        }
    }
}
