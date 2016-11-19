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
            <ResourceDictionary xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:test='clr-namespace:Granular.Presentation.Tests;assembly=Granular.Presentation.Tests' >
                <ControlTemplate x:Key='template1'>
                    <StackPanel>
                        <test:RoutedEventTestElement x:Name='child1' Width='100' Template='{DynamicResource template2}'/>
                    </StackPanel>
                </ControlTemplate>

                <ControlTemplate x:Key='template2'>
                    <StackPanel>
                        <test:RoutedEventTestElement x:Name='child2' Width='200'/>
                    </StackPanel>
                </ControlTemplate>
            </ResourceDictionary>
            ";

            ResourceDictionary resources = XamlLoader.Load(XamlParser.Parse(text)) as ResourceDictionary;

            RoutedEventTestElement root = new RoutedEventTestElement();
            root.Resources = resources;

            root.Template = resources.GetValue("template1") as ControlTemplate;
            root.ApplyTemplate();

            RoutedEventTestElement child1 = NameScope.GetTemplateNameScope(root).FindName("child1") as RoutedEventTestElement;
            child1.ApplyTemplate();

            Assert.IsNotNull(child1);
            Assert.AreEqual(100, child1.Width);

            RoutedEventTestElement child2 = NameScope.GetTemplateNameScope(child1).FindName("child2") as RoutedEventTestElement;
            Assert.IsNotNull(child2);
            Assert.AreEqual(200, child2.Width);

            object source0 = null;
            object source1 = null;
            object source2 = null;

            root.Bubble += (sender, e) => source0 = e.Source;
            child1.Bubble += (sender, e) => source1 = e.Source;
            child2.Bubble += (sender, e) => source2 = e.Source;

            child2.RaiseEvent(new RoutedEventArgs(RoutedEventTestElement.BubbleEvent, child2));

            Assert.AreEqual(root, source0);
            Assert.AreEqual(child1, source1);
            Assert.AreEqual(child2, source2);
        }
    }
}
