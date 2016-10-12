using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;
using Granular.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Data
{
    public class BindingTestIntToDoubleConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter)
        {
            if (targetType != typeof(double))
            {
                return 0.0;
            }

            return (double)((int)value * Int32.Parse(parameter.ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter)
        {
            if (targetType != typeof(int))
            {
                return 0;
            }

            return (int)((double)value / Int32.Parse(parameter.ToString()));
        }

        public object ProvideValue(InitializeContext context)
        {
            return this;
        }
    }

    public class BindingTestDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double doubleValue { get; set; }
        public double DoubleValue
        {
            get { return doubleValue; }
            set
            {
                if (doubleValue == value)
                {
                    return;
                }

                doubleValue = value;
                PropertyChanged.Raise(this, new PropertyChangedEventArgs("DoubleValue"));
            }
        }

        private int intValue { get; set; }
        public int IntValue
        {
            get { return intValue; }
            set
            {
                if (intValue == value)
                {
                    return;
                }

                intValue = value;
                PropertyChanged.Raise(this, new PropertyChangedEventArgs("IntValue"));
            }
        }
    }

    [TestClass]
    public class BindingExtensionTest
    {
        public static readonly DependencyProperty Value1Property = DependencyProperty.RegisterAttached("Value1", typeof(double), typeof(BindingExtensionTest), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty Value2Property = DependencyProperty.RegisterAttached("Value2", typeof(double), typeof(BindingExtensionTest), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty Value3Property = DependencyProperty.RegisterAttached("Value3", typeof(double), typeof(BindingExtensionTest), new FrameworkPropertyMetadata());

        [TestMethod]
        public void BindingExtensionBaseTest()
        {
            string text = @"<FrameworkElement xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' Width='{Binding DoubleValue}'/>";
            FrameworkElement element = XamlLoader.Load(XamlParser.Parse(text)) as FrameworkElement;

            BindingTestDataContext dataContext = new BindingTestDataContext();
            dataContext.DoubleValue = 100;

            element.DataContext = dataContext;

            Assert.AreEqual(100, element.Width);

            dataContext.DoubleValue = 200;

            Assert.AreEqual(200, element.Width);

            element.Width = 300;

            Assert.AreEqual(300, dataContext.DoubleValue);
        }

        [TestMethod]
        public void BindingExtensionConverterTest()
        {
            string text = @"<FrameworkElement xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:test='clr-namespace:Granular.Presentation.Tests.Data;assembly=Granular.Presentation.Tests' Width='{Binding IntValue, Converter={test:BindingTestIntToDoubleConverter}, ConverterParameter=10}'/>";

            FrameworkElement element = XamlLoader.Load(XamlParser.Parse(text)) as FrameworkElement;

            BindingTestDataContext dataContext = new BindingTestDataContext();
            dataContext.IntValue = 10;

            element.DataContext = dataContext;

            Assert.AreEqual(100, element.Width);

            dataContext.IntValue = 20;

            Assert.AreEqual(200, element.Width);

            element.Width = 300;

            Assert.AreEqual(30, dataContext.IntValue);
        }

        [TestMethod]
        public void BindingExtensionRelativeSourceFindAncestorTest()
        {
            string text = @"
            <StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:test='clr-namespace:Granular.Presentation.Tests.Data;assembly=Granular.Presentation.Tests' x:Name='panel1' Width='300'>
                <Grid x:Name='panel2' Width='100' Height='200'>
                    <StackPanel x:Name='panel3'>
                        <Control x:Name='control'
                                 test:BindingExtensionTest.Value1='{Binding Width, RelativeSource={RelativeSource FindAncestor, AncestorLevel=2}}'
                                 test:BindingExtensionTest.Value2='{Binding Height, RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Grid}}}'
                                 test:BindingExtensionTest.Value3='{Binding Width, RelativeSource={RelativeSource FindAncestor, AncestorLevel=2, AncestorType={x:Type StackPanel}}}'/>
                    </StackPanel>
                </Grid>
            </StackPanel>";

            Panel panel1 = XamlLoader.Load(XamlParser.Parse(text)) as Panel;
            Panel panel2 = NameScope.GetNameScope(panel1).FindName("panel2") as Panel;
            Panel panel3 = NameScope.GetNameScope(panel1).FindName("panel3") as Panel;

            Control control = NameScope.GetNameScope(panel1).FindName("control") as Control;

            Assert.AreEqual(100.0, control.GetValue(Value1Property));
            Assert.AreEqual(200.0, control.GetValue(Value2Property));
            Assert.AreEqual(300.0, control.GetValue(Value3Property));
        }

        [TestMethod]
        public void BindingExtensionElementNameTest()
        {
            string text = @"
            <StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' x:Name='panel' Width='100'>
                <FrameworkElement x:Name='child' Width='{Binding Width, ElementName=panel}'/>
            </StackPanel>";

            Panel panel = XamlLoader.Load(XamlParser.Parse(text)) as Panel;

            FrameworkElement child = NameScope.GetNameScope(panel).FindName("child") as FrameworkElement;

            Assert.AreEqual(100, child.Width);
        }

        [TestMethod]
        public void BindingExtensionRelativeSourceTemplatedParentTest()
        {
            string text = @"
            <ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' x:Name='root'>
                <FrameworkElement x:Name='child' Width='{Binding Height, RelativeSource={RelativeSource TemplatedParent}}'/>
            </ControlTemplate>";

            ControlTemplate template = XamlLoader.Load(XamlParser.Parse(text)) as ControlTemplate;

            Control control = new Control();

            control.Template = template;
            control.ApplyTemplate();

            FrameworkElement child = NameScope.GetTemplateNameScope(control).FindName("child") as FrameworkElement;

            Assert.AreEqual(control, child.TemplatedParent);

            control.Height = 100;
            Assert.AreEqual(100, child.Width);

            control.ClearValue(FrameworkElement.HeightProperty);
            child.SetValue(FrameworkElement.WidthProperty, 200.0, BaseValueSource.ParentTemplate);
            Assert.AreEqual(200, control.Height);
        }

        [TestMethod]
        public void BindingExtensionRelativeSourceSelfTest()
        {
            string text = @"<TextBlock xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                                       DataContext='DataContextValue' Text=""{Binding RelativeSource={RelativeSource Self}, Path=DataContext, StringFormat='Self value is {0}'}""/>";

            TextBlock textBlock = XamlLoader.Load(XamlParser.Parse(text)) as TextBlock;

            Assert.AreEqual("Self value is DataContextValue", textBlock.Text);
        }
    }
}
