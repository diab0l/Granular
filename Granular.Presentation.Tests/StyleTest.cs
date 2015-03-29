using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xaml;
using Granular.Presentation.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests
{
    public class TestStyle : Style
    {
        public int InitializedHandlerCallsCount { get; private set; }

        private void InitializedHandler(object sender, RoutedEventArgs e)
        {
            InitializedHandlerCallsCount++;
        }
    }

    public class TestTriggerAction : ITriggerAction
    {
        public int EnterCount { get; private set; }
        public int ExitCount { get; private set; }

        public string Key { get; private set; }

        public TestTriggerAction(string key)
        {
            this.Key = key;
        }

        public void EnterAction(FrameworkElement target, BaseValueSource valueSource)
        {
            EnterCount++;
        }

        public void ExitAction(FrameworkElement target, BaseValueSource valueSource)
        {
            ExitCount++;
        }

        public bool IsActionOverlaps(ITriggerAction action)
        {
            return this.Key == ((TestTriggerAction)action).Key;
        }
    }

    [TestClass]
    public class StyleTest
    {
        [TestMethod]
        public void StyleEventTriggerTest()
        {
            string text = @"
            <Style xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:test='clr-namespace:Granular.Presentation.Tests;assembly=Granular.Presentation.Tests'>
                <Setter Property='FrameworkElement.Width' Value='100'/>
                <EventSetter Event='FrameworkElement.Initialized' Handler='InitializedHandler'/>
                <Style.Triggers>
                    <EventTrigger RoutedEvent='FrameworkElement.Initialized'>
                        <Setter Property='FrameworkElement.Width' Value='200'/>
                    </EventTrigger>
                </Style.Triggers>
            </Style>";

            TestStyle style = new TestStyle();
            XamlLoader.Load(style, XamlParser.Parse(text));

            Control control = new Control();

            control.Style = style;
            Assert.AreEqual(100, control.Width);
            Assert.AreEqual(BaseValueSource.Style, control.GetValueSource(FrameworkElement.WidthProperty).BaseValueSource);
            Assert.AreEqual(0, style.InitializedHandlerCallsCount);

            control.RaiseEvent(new RoutedEventArgs(FrameworkElement.InitializedEvent, control));
            Assert.AreEqual(200, control.Width);
            Assert.AreEqual(BaseValueSource.StyleTrigger, control.GetValueSource(FrameworkElement.WidthProperty).BaseValueSource);
            Assert.AreEqual(1, style.InitializedHandlerCallsCount);

            control.Style = null;
            Assert.AreEqual(Double.NaN, control.Width);
            Assert.AreEqual(BaseValueSource.Default, control.GetValueSource(FrameworkElement.WidthProperty).BaseValueSource);

            control.RaiseEvent(new RoutedEventArgs(FrameworkElement.InitializedEvent, control));
            Assert.AreEqual(Double.NaN, control.Width);
            Assert.AreEqual(BaseValueSource.Default, control.GetValueSource(FrameworkElement.WidthProperty).BaseValueSource);
            Assert.AreEqual(1, style.InitializedHandlerCallsCount);
        }

        [TestMethod]
        public void StyleDataTriggerTest()
        {
            string text = @"
            <Style xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:test='clr-namespace:Granular.Presentation.Tests;assembly=Granular.Presentation.Tests'>
                <Setter Property='FrameworkElement.Width' Value='100'/>
                <Style.Triggers>
                    <DataTrigger Binding='{Binding DoubleValue}' Value='1'>
                        <Setter Property='FrameworkElement.Width' Value='200'/>
                    </DataTrigger>
                </Style.Triggers>
            </Style>";

            TestStyle style = new TestStyle();
            XamlLoader.Load(style, XamlParser.Parse(text));

            Control control = new Control();

            BindingTestDataContext dataContext = new BindingTestDataContext();

            dataContext.DoubleValue = 1;

            control.Style = style;

            Assert.AreEqual(100, control.Width);

            control.DataContext = dataContext;

            Assert.AreEqual(200, control.Width);

            dataContext.DoubleValue = 2;

            Assert.AreEqual(100, control.Width);

            dataContext.DoubleValue = 1;

            Assert.AreEqual(200, control.Width);

            control.DataContext = null;

            Assert.AreEqual(100, control.Width);
        }

        [TestMethod]
        public void StyleFindDefaultTest()
        {
            Style style = new Style { TargetType = typeof(Control) };
            style.Setters.Add(new Setter { Property = new DependencyPropertyPathElement(FrameworkElement.WidthProperty), Value = 100 });

            ResourceDictionary resources = new ResourceDictionary();
            resources.Add(new StyleKey(typeof(Control)), style);

            Control control = new Control();
            control.Resources = resources;

            Assert.AreEqual(style, control.Style);
        }

        [TestMethod]
        public void StyleBasedOnTest()
        {
            Style base1 = new Style();
            base1.Setters.Add(new TestTriggerAction("action1"));
            base1.Setters.Add(new TestTriggerAction("action2"));

            Style base2 = new Style { BasedOn = base1 };
            base2.Setters.Add(new TestTriggerAction("action2"));
            base2.Setters.Add(new TestTriggerAction("action3"));

            Style style = new Style { BasedOn = base2 };
            style.Setters.Add(new TestTriggerAction("action3"));
            style.Setters.Add(new TestTriggerAction("action4"));

            FrameworkElement element = new FrameworkElement();

            element.Style = style;
            Assert.AreEqual(1, ((TestTriggerAction)base1.Setters[0]).EnterCount);
            Assert.AreEqual(0, ((TestTriggerAction)base1.Setters[1]).EnterCount);
            Assert.AreEqual(1, ((TestTriggerAction)base2.Setters[0]).EnterCount);
            Assert.AreEqual(0, ((TestTriggerAction)base2.Setters[1]).EnterCount);
            Assert.AreEqual(1, ((TestTriggerAction)style.Setters[0]).EnterCount);
            Assert.AreEqual(1, ((TestTriggerAction)style.Setters[1]).EnterCount);

            element.Style = null;
            Assert.AreEqual(1, ((TestTriggerAction)base1.Setters[0]).ExitCount);
            Assert.AreEqual(0, ((TestTriggerAction)base1.Setters[1]).ExitCount);
            Assert.AreEqual(1, ((TestTriggerAction)base2.Setters[0]).ExitCount);
            Assert.AreEqual(0, ((TestTriggerAction)base2.Setters[1]).ExitCount);
            Assert.AreEqual(1, ((TestTriggerAction)style.Setters[0]).ExitCount);
            Assert.AreEqual(1, ((TestTriggerAction)style.Setters[1]).ExitCount);
        }
    }
}
