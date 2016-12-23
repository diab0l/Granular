using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests
{
    public class RoutedEventTestElement : Control
    {
        public static readonly RoutedEvent DirectEvent = EventManager.RegisterRoutedEvent("Direct", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(RoutedEventTestElement));
        public static readonly RoutedEvent BubbleEvent = EventManager.RegisterRoutedEvent("Bubble", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RoutedEventTestElement));
        public static readonly RoutedEvent TunnelEvent = EventManager.RegisterRoutedEvent("Tunnel", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(RoutedEventTestElement));

        public event RoutedEventHandler Direct
        {
            add { AddHandler(DirectEvent, value); }
            remove { RemoveHandler(DirectEvent, value); }
        }

        public event RoutedEventHandler Bubble
        {
            add { AddHandler(BubbleEvent, value); }
            remove { RemoveHandler(BubbleEvent, value); }
        }

        public event RoutedEventHandler Tunnel
        {
            add { AddHandler(TunnelEvent, value); }
            remove { RemoveHandler(TunnelEvent, value); }
        }
    }

    [TestClass]
    public class RoutedEventTest
    {
        public static readonly RoutedEvent BubbleTestEvent = EventManager.RegisterRoutedEvent("BubbleTest", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RoutedEventTest));

        [TestMethod]
        public void RoutingStrategyTest()
        {
            RoutedEventTestElement root = new RoutedEventTestElement();
            RoutedEventTestElement child1 = new RoutedEventTestElement();
            RoutedEventTestElement child2 = new RoutedEventTestElement();

            root.AddVisualChild(child1);
            child1.AddVisualChild(child2);

            int rootLastEvent = 0;
            int child1LastEvent = 0;
            int child2LastEvent = 0;

            int currentEvent = 0;

            root.Direct += (sender, e) => { rootLastEvent = ++currentEvent; };
            root.Bubble += (sender, e) => { rootLastEvent = ++currentEvent; };
            root.Tunnel += (sender, e) => { rootLastEvent = ++currentEvent; };
            child1.Direct += (sender, e) => { child1LastEvent = ++currentEvent; };
            child1.Bubble += (sender, e) => { child1LastEvent = ++currentEvent; };
            child1.Tunnel += (sender, e) => { child1LastEvent = ++currentEvent; };
            child2.Direct += (sender, e) => { child2LastEvent = ++currentEvent; };
            child2.Bubble += (sender, e) => { child2LastEvent = ++currentEvent; };
            child2.Tunnel += (sender, e) => { child2LastEvent = ++currentEvent; };

            child1.RaiseEvent(new RoutedEventArgs(RoutedEventTestElement.DirectEvent, child1));

            Assert.AreEqual(0, rootLastEvent);
            Assert.AreEqual(1, child1LastEvent);
            Assert.AreEqual(0, child2LastEvent);

            child1.RaiseEvent(new RoutedEventArgs(RoutedEventTestElement.BubbleEvent, child1));

            Assert.AreEqual(3, rootLastEvent);
            Assert.AreEqual(2, child1LastEvent);
            Assert.AreEqual(0, child2LastEvent);

            child1.RaiseEvent(new RoutedEventArgs(RoutedEventTestElement.TunnelEvent, child1));

            Assert.AreEqual(4, rootLastEvent);
            Assert.AreEqual(5, child1LastEvent);
            Assert.AreEqual(0, child2LastEvent);
        }

        [TestMethod]
        public void HandledEventTest()
        {
            RoutedEventTestElement root = new RoutedEventTestElement();
            RoutedEventTestElement child1 = new RoutedEventTestElement();
            RoutedEventTestElement child2 = new RoutedEventTestElement();

            root.AddVisualChild(child1);
            child1.AddVisualChild(child2);

            int rootEventCount = 0;
            int rootHandledEventCount = 0;
            int child1EventCount = 0;
            int child2EventCount = 0;

            root.Bubble += (sender, e) => { rootEventCount++; };
            child1.Bubble += (sender, e) => { child1EventCount++; e.Handled = true; };
            child2.Bubble += (sender, e) => { child2EventCount++; };
            root.AddHandler(RoutedEventTestElement.BubbleEvent, new RoutedEventHandler((sender, e) => { rootHandledEventCount++; }), true);

            child2.RaiseEvent(new RoutedEventArgs(RoutedEventTestElement.BubbleEvent, child2));

            Assert.AreEqual(0, rootEventCount);
            Assert.AreEqual(1, rootHandledEventCount);
            Assert.AreEqual(1, child1EventCount);
            Assert.AreEqual(1, child2EventCount);
        }

        [TestMethod]
        public void RoutedEventSourceTest()
        {
            string text = @"
            <Decorator x:Name='decorator1' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:test='clr-namespace:Granular.Presentation.Tests;assembly=Granular.Presentation.Tests' >
                <Decorator.Resources>
                    <ControlTemplate x:Key='template2'>
                        <Decorator x:Name='decorator2'>
                            <Decorator x:Name='decorator2a'>
                                <test:RoutedEventTestElement x:Name='element2' Width='20' Template='{DynamicResource template3}'/>
                            </Decorator>
                        </Decorator>
                    </ControlTemplate>

                    <ControlTemplate x:Key='template3'>
                        <Decorator x:Name='decorator3'>
                            <Decorator x:Name='decorator3a'>
                                <test:RoutedEventTestElement x:Name='element3' Width='30'/>
                            </Decorator>
                        </Decorator>
                    </ControlTemplate>
                </Decorator.Resources>

                <Decorator x:Name='decorator1a'>
                    <test:RoutedEventTestElement x:Name='element1' Width='10' Template='{DynamicResource template2}'/>
                </Decorator>
            </Decorator>";

            Decorator decorator1 = XamlLoader.Load(XamlParser.Parse(text)) as Decorator;
            Assert.IsNotNull(decorator1);

            RoutedEventTestElement element1 = NameScope.GetNameScope(decorator1).FindName("element1") as RoutedEventTestElement;
            Assert.IsNotNull(element1);
            Assert.AreEqual(10, element1.Width);

            Decorator decorator2 = NameScope.GetTemplateNameScope(element1).FindName("decorator2") as Decorator;
            Assert.IsNotNull(decorator2);

            RoutedEventTestElement element2 = NameScope.GetTemplateNameScope(element1).FindName("element2") as RoutedEventTestElement;
            Assert.IsNotNull(element2);
            Assert.AreEqual(20, element2.Width);

            Decorator decorator3 = NameScope.GetTemplateNameScope(element2).FindName("decorator3") as Decorator;
            Assert.IsNotNull(decorator3);

            RoutedEventTestElement element3 = NameScope.GetTemplateNameScope(element2).FindName("element3") as RoutedEventTestElement;
            Assert.IsNotNull(element3);
            Assert.AreEqual(30, element3.Width);

            object source1 = null;
            object source2 = null;
            object source3 = null;

            decorator1.AddHandler(RoutedEventTestElement.BubbleEvent, (RoutedEventHandler)((sender, e) => source1 = e.Source));
            decorator2.AddHandler(RoutedEventTestElement.BubbleEvent, (RoutedEventHandler)((sender, e) => source2 = e.Source));
            decorator3.AddHandler(RoutedEventTestElement.BubbleEvent, (RoutedEventHandler)((sender, e) => source3 = e.Source));

            element3.RaiseEvent(new RoutedEventArgs(RoutedEventTestElement.BubbleEvent, element3));

            Assert.AreEqual(element1, source1);
            Assert.AreEqual(element2, source2);
            Assert.AreEqual(element3, source3);
        }
    }
}
