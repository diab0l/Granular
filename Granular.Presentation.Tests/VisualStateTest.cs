using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Xaml;
using Granular.Presentation.Media.Animation.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class VisualStateTest
    {
        [TestMethod]
        public void VisualStateParseTest()
        {
            string text = @"
            <FrameworkElement xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                <VisualStateManager.VisualStateGroups>
                    <VisualStateGroup x:Name='CommonStates'>
                        <VisualStateGroup.Transitions>
                            <VisualTransition To='Pressed' />
                        </VisualStateGroup.Transitions>
                        <VisualState x:Name='Normal' />
                        <VisualState x:Name='MouseOver'>
                            <Storyboard>
                            </Storyboard>
                        </VisualState>
                        <VisualState x:Name='Pressed'>
                            <Storyboard>
                            </Storyboard>
                        </VisualState>
                        <VisualState x:Name='Disabled'>
                            <Storyboard>
                            </Storyboard>
                        </VisualState>
                    </VisualStateGroup>
                </VisualStateManager.VisualStateGroups>
            </FrameworkElement>";

            XamlElement rootElement = XamlParser.Parse(text);
            FrameworkElement element = XamlLoader.Load(rootElement) as FrameworkElement;

            FreezableCollection<VisualStateGroup> list = VisualStateManager.GetVisualStateGroups(element);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(1, list[0].Transitions.Count);
            Assert.AreEqual(4, list[0].States.Count);
            Assert.AreEqual("CommonStates", list[0].Name);
            Assert.AreEqual("Normal", list[0].States[0].Name);
        }

        [TestMethod]
        public void VisualStateStoryboardTest()
        {
            string text = @"
            <Control xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' Width='0'>
                <Control.TemplateChild>
                    <FrameworkElement>
                        <VisualStateManager.VisualStateGroups>
                            <VisualStateGroup x:Name='StateGroup1'>
                                <VisualStateGroup.Transitions>

                                    <VisualTransition To='State1'>
                                        <VisualTransition.Storyboard>
                                            <Storyboard>
                                                <DoubleAnimation Storyboard.TargetProperty='Width' To='100'/>
                                            </Storyboard>
                                        </VisualTransition.Storyboard>
                                    </VisualTransition>

                                    <VisualTransition To='State2'>
                                        <VisualTransition.Storyboard>
                                            <Storyboard>
                                                <DoubleAnimation Storyboard.TargetProperty='Width' To='200'/>
                                            </Storyboard>
                                        </VisualTransition.Storyboard>
                                    </VisualTransition>

                                </VisualStateGroup.Transitions>

                                <VisualState x:Name='State1'>
                                    <VisualState.Storyboard>
                                        <Storyboard>
                                            <DoubleAnimation Storyboard.TargetProperty='Width' From='100' To='200'/>
                                        </Storyboard>
                                    </VisualState.Storyboard>
                                </VisualState>

                                <VisualState x:Name='State2'>
                                    <VisualState.Storyboard>
                                        <Storyboard>
                                            <DoubleAnimation Storyboard.TargetProperty='Width' From='200' To='300'/>
                                        </Storyboard>
                                    </VisualState.Storyboard>
                                </VisualState>

                            </VisualStateGroup>
                        </VisualStateManager.VisualStateGroups>
                    </FrameworkElement>
                </Control.TemplateChild>
            </Control>";

            Control control = XamlLoader.Load(XamlParser.Parse(text)) as Control;

            VisualStateGroup group1 = VisualStateManager.GetVisualStateGroups(control.TemplateChild).FirstOrDefault();
            Assert.IsTrue(group1 != null);

            TestRootClock rootClock = new TestRootClock();
            control.SetAnimatableRootClock(new AnimatableRootClock(rootClock, true));

            VisualStateManager.GoToState(control, "State1", true);

            rootClock.Tick(TimeSpan.FromSeconds(0));
            Assert.AreEqual(0, control.Width);

            rootClock.Tick(TimeSpan.FromSeconds(0.5));
            Assert.AreEqual(50, control.Width);

            rootClock.Tick(TimeSpan.FromSeconds(1));
            Assert.AreEqual(100, control.Width);

            rootClock.Tick(TimeSpan.FromSeconds(1.5));
            Assert.AreEqual(150, control.Width);

            VisualStateManager.GoToState(control, "State2", true);

            rootClock.Tick(TimeSpan.FromSeconds(2));
            Assert.AreEqual(175, control.Width);

            rootClock.Tick(TimeSpan.FromSeconds(2.5));
            Assert.AreEqual(200, control.Width);

            rootClock.Tick(TimeSpan.FromSeconds(3));
            Assert.AreEqual(250, control.Width);

            rootClock.Tick(TimeSpan.FromSeconds(4));
            Assert.AreEqual(300, control.Width);

            VisualStateManager.GoToState(control, "State1", false);

            rootClock.Tick(TimeSpan.FromSeconds(4));
            Assert.AreEqual(100, control.Width);

            rootClock.Tick(TimeSpan.FromSeconds(5));
            Assert.AreEqual(200, control.Width);
        }

        [TestMethod]
        public void VisualStateEmptyStoryboardTest()
        {
            string text = @"
            <Control xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' Width='100'>
                <Control.TemplateChild>
                    <FrameworkElement>
                        <VisualStateManager.VisualStateGroups>
                            <VisualStateGroup x:Name='StateGroup1'>
                                <VisualStateGroup.Transitions>

                                    <VisualTransition To='State2'>
                                        <VisualTransition.Storyboard>
                                            <Storyboard>
                                                <DoubleAnimation Storyboard.TargetProperty='Width' To='200'/>
                                            </Storyboard>
                                        </VisualTransition.Storyboard>
                                    </VisualTransition>

                                </VisualStateGroup.Transitions>

                                <VisualState x:Name='State1'/>

                                <VisualState x:Name='State2'>
                                    <VisualState.Storyboard>
                                        <Storyboard>
                                            <DoubleAnimation Storyboard.TargetProperty='Width' From='200' To='300'/>
                                        </Storyboard>
                                    </VisualState.Storyboard>
                                </VisualState>

                            </VisualStateGroup>
                        </VisualStateManager.VisualStateGroups>
                    </FrameworkElement>
                </Control.TemplateChild>
            </Control>";

            Control control = XamlLoader.Load(XamlParser.Parse(text)) as Control;

            TestRootClock rootClock = new TestRootClock();
            control.SetAnimatableRootClock(new AnimatableRootClock(rootClock, true));

            Assert.AreEqual(100, control.Width);

            VisualStateManager.GoToState(control, "State2", true);

            rootClock.Tick(TimeSpan.FromSeconds(0));
            Assert.AreEqual(100, control.Width);

            rootClock.Tick(TimeSpan.FromSeconds(0.5));
            Assert.AreEqual(150, control.Width);

            VisualStateManager.GoToState(control, "State1", true);
            Assert.AreEqual(100, control.Width);

            rootClock.Tick(TimeSpan.FromSeconds(1));

            VisualStateManager.GoToState(control, "State2", true);

            rootClock.Tick(TimeSpan.FromSeconds(1));
            Assert.AreEqual(100, control.Width);

            rootClock.Tick(TimeSpan.FromSeconds(1.5));
            Assert.AreEqual(150, control.Width);

            rootClock.Tick(TimeSpan.FromSeconds(2));
            Assert.AreEqual(200, control.Width);

            rootClock.Tick(TimeSpan.FromSeconds(2.5));
            Assert.AreEqual(250, control.Width);

            VisualStateManager.GoToState(control, "State1", true);
            Assert.AreEqual(100, control.Width);

            rootClock.Tick(TimeSpan.FromSeconds(3));

            VisualStateManager.GoToState(control, "State2", false);

            rootClock.Tick(TimeSpan.FromSeconds(3));
            Assert.AreEqual(200, control.Width);

            rootClock.Tick(TimeSpan.FromSeconds(3.5));
            Assert.AreEqual(250, control.Width);

            VisualStateManager.GoToState(control, "State1", true);
            Assert.AreEqual(100, control.Width);
        }
    }
}
