using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Granular.Presentation.Tests.Input
{
    [TestClass]
    public class KeyboardNavigationTargetTest
    {
        [TestMethod]
        public void KeyboardTabNavigationContinueTest()
        {
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Continue, Orientation.Horizontal, null, FocusNavigationDirection.Next, new[] { 1, 2, 3, 4, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Continue, Orientation.Horizontal, null, FocusNavigationDirection.Previous, new[] { 4, 0, 1, 2, 3 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Continue, Orientation.Horizontal, null, FocusNavigationDirection.First, new[] { 0, 0, 0, 0, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Continue, Orientation.Horizontal, null, FocusNavigationDirection.Last, new[] { 4, 4, 4, 4, 4 });
        }

        [TestMethod]
        public void KeyboardTabNavigationOnceTest()
        {
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Once, Orientation.Horizontal, null, FocusNavigationDirection.Next, new[] { 1, 4, 4, 4, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Once, Orientation.Horizontal, null, FocusNavigationDirection.Previous, new[] { 4, 0, 0, 0, 3 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Once, Orientation.Horizontal, null, FocusNavigationDirection.First, new[] { 0, 0, 0, 0, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Once, Orientation.Horizontal, null, FocusNavigationDirection.Last, new[] { 4, 4, 4, 4, 4 });
        }

        [TestMethod]
        public void KeyboardTabNavigationCycleTest()
        {
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Cycle, Orientation.Horizontal, null, FocusNavigationDirection.Next, new[] { 1, 2, 3, 1, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Cycle, Orientation.Horizontal, null, FocusNavigationDirection.Previous, new[] { 4, 3, 1, 2, 3 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Cycle, Orientation.Horizontal, null, FocusNavigationDirection.First, new[] { 0, 1, 1, 1, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Cycle, Orientation.Horizontal, null, FocusNavigationDirection.Last, new[] { 4, 3, 3, 3, 4 });
        }

        [TestMethod]
        public void KeyboardTabNavigationNoneTest()
        {
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.None, Orientation.Horizontal, null, FocusNavigationDirection.Next, new[] { 4, 4, 4, 4, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.None, Orientation.Horizontal, null, FocusNavigationDirection.Previous, new[] { 4, 0, 0, 0, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.None, Orientation.Horizontal, null, FocusNavigationDirection.First, new[] { 0, 0, 0, 0, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.None, Orientation.Horizontal, null, FocusNavigationDirection.Last, new[] { 4, 4, 4, 4, 4 });
        }

        [TestMethod]
        public void KeyboardTabNavigationContainedTest()
        {
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Contained, Orientation.Horizontal, null, FocusNavigationDirection.Next, new[] { 1, 2, 3, 3, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Contained, Orientation.Horizontal, null, FocusNavigationDirection.Previous, new[] { 4, 1, 1, 2, 3 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Contained, Orientation.Horizontal, null, FocusNavigationDirection.First, new[] { 0, 1, 1, 1, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Contained, Orientation.Horizontal, null, FocusNavigationDirection.Last, new[] { 4, 3, 3, 3, 4 });
        }

        [TestMethod]
        public void KeyboardTabNavigationLocalTest()
        {
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Local, Orientation.Horizontal, new[] { 0, 2, 1, 4, 3 }, FocusNavigationDirection.Next, new[] { 4, 3, 1, 0, 2 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Local, Orientation.Horizontal, new[] { 0, 2, 1, 4, 3 }, FocusNavigationDirection.Previous, new[] { 3, 2, 4, 1, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Local, Orientation.Horizontal, new[] { 0, 2, 1, 4, 3 }, FocusNavigationDirection.First, new[] { 0, 0, 0, 0, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Local, Orientation.Horizontal, new[] { 0, 2, 1, 4, 3 }, FocusNavigationDirection.Last, new[] { 3, 3, 3, 3, 3 });
        }

        [TestMethod]
        public void KeyboardDirectionalNavigationContinueTest()
        {
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Continue, Orientation.Horizontal, null, FocusNavigationDirection.Right, new[] { 1, 2, 3, 4, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Continue, Orientation.Horizontal, null, FocusNavigationDirection.Left, new[] { 4, 0, 1, 2, 3 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Continue, Orientation.Vertical, null, FocusNavigationDirection.Down, new[] { 1, 2, 3, 4, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Continue, Orientation.Vertical, null, FocusNavigationDirection.Up, new[] { 4, 0, 1, 2, 3 });
        }

        [TestMethod]
        public void KeyboardDirectionalNavigationCycleTest()
        {
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Cycle, Orientation.Horizontal, null, FocusNavigationDirection.Right, new[] { 1, 2, 3, 1, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Cycle, Orientation.Horizontal, null, FocusNavigationDirection.Left, new[] { 4, 3, 1, 2, 3 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Cycle, Orientation.Vertical, null, FocusNavigationDirection.Down, new[] { 1, 2, 3, 1, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Cycle, Orientation.Vertical, null, FocusNavigationDirection.Up, new[] { 4, 3, 1, 2, 3 });
        }

        [TestMethod]
        public void KeyboardDirectionalNavigationContainedTest()
        {
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Contained, Orientation.Horizontal, null, FocusNavigationDirection.Right, new[] { 1, 2, 3, 3, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Contained, Orientation.Horizontal, null, FocusNavigationDirection.Left, new[] { 4, 1, 1, 2, 3 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Contained, Orientation.Vertical, null, FocusNavigationDirection.Down, new[] { 1, 2, 3, 3, 0 });
            KeyboardNavigationInnerScopeTest(KeyboardNavigationMode.Contained, Orientation.Vertical, null, FocusNavigationDirection.Up, new[] { 4, 1, 1, 2, 3 });
        }

        [TestMethod]
        public void KeyboardDirectionalNavigationCanvasTest()
        {
            Control child1 = new Control { Name = "child1", Width = 100, Height = 100 };
            Control child2 = new Control { Name = "child2", Width = 100, Height = 100 };
            Control child3 = new Control { Name = "child3", Width = 100, Height = 100 };
            Control child4 = new Control { Name = "child4", Width = 100, Height = 100 };
            Control child5 = new Control { Name = "child5", Width = 100, Height = 100 };

            Canvas.SetLeft(child1, 0);
            Canvas.SetTop(child1, 0);

            Canvas.SetLeft(child2, 400);
            Canvas.SetTop(child2, 10);

            Canvas.SetLeft(child3, 200);
            Canvas.SetTop(child3, 200);

            Canvas.SetLeft(child4, 10);
            Canvas.SetTop(child4, 400);

            Canvas.SetLeft(child5, 410);
            Canvas.SetTop(child5, 410);

            Canvas canvas = new Canvas { Width = 600, Height = 600, IsRootElement = true };

            canvas.Children.Add(child1);
            canvas.Children.Add(child2);
            canvas.Children.Add(child3);
            canvas.Children.Add(child4);
            canvas.Children.Add(child5);

            canvas.Measure(new Size(1000, 1000));
            canvas.Arrange(new Rect(1000, 1000));

            KeyboardNavigation.SetTabNavigation(canvas, KeyboardNavigationMode.Contained);

            Assert.AreEqual(child1, KeyboardNavigationTarget.FindTarget(child1, FocusNavigationDirection.Left, KeyboardNavigation.TabNavigationProperty));
            Assert.AreEqual(child3, KeyboardNavigationTarget.FindTarget(child1, FocusNavigationDirection.Right, KeyboardNavigation.TabNavigationProperty));
            Assert.AreEqual(child1, KeyboardNavigationTarget.FindTarget(child1, FocusNavigationDirection.Up, KeyboardNavigation.TabNavigationProperty));
            Assert.AreEqual(child3, KeyboardNavigationTarget.FindTarget(child1, FocusNavigationDirection.Down, KeyboardNavigation.TabNavigationProperty));

            Assert.AreEqual(child3, KeyboardNavigationTarget.FindTarget(child2, FocusNavigationDirection.Left, KeyboardNavigation.TabNavigationProperty));
            Assert.AreEqual(child2, KeyboardNavigationTarget.FindTarget(child2, FocusNavigationDirection.Right, KeyboardNavigation.TabNavigationProperty));
            Assert.AreEqual(child2, KeyboardNavigationTarget.FindTarget(child2, FocusNavigationDirection.Up, KeyboardNavigation.TabNavigationProperty));
            Assert.AreEqual(child3, KeyboardNavigationTarget.FindTarget(child2, FocusNavigationDirection.Down, KeyboardNavigation.TabNavigationProperty));

            Assert.AreEqual(child4, KeyboardNavigationTarget.FindTarget(child3, FocusNavigationDirection.Left, KeyboardNavigation.TabNavigationProperty));
            Assert.AreEqual(child2, KeyboardNavigationTarget.FindTarget(child3, FocusNavigationDirection.Right, KeyboardNavigation.TabNavigationProperty));
            Assert.AreEqual(child2, KeyboardNavigationTarget.FindTarget(child3, FocusNavigationDirection.Up, KeyboardNavigation.TabNavigationProperty));
            Assert.AreEqual(child4, KeyboardNavigationTarget.FindTarget(child3, FocusNavigationDirection.Down, KeyboardNavigation.TabNavigationProperty));

            Assert.AreEqual(child4, KeyboardNavigationTarget.FindTarget(child4, FocusNavigationDirection.Left, KeyboardNavigation.TabNavigationProperty));
            Assert.AreEqual(child3, KeyboardNavigationTarget.FindTarget(child4, FocusNavigationDirection.Right, KeyboardNavigation.TabNavigationProperty));
            Assert.AreEqual(child3, KeyboardNavigationTarget.FindTarget(child4, FocusNavigationDirection.Up, KeyboardNavigation.TabNavigationProperty));
            Assert.AreEqual(child4, KeyboardNavigationTarget.FindTarget(child4, FocusNavigationDirection.Down, KeyboardNavigation.TabNavigationProperty));

            Assert.AreEqual(child3, KeyboardNavigationTarget.FindTarget(child5, FocusNavigationDirection.Left, KeyboardNavigation.TabNavigationProperty));
            Assert.AreEqual(child5, KeyboardNavigationTarget.FindTarget(child5, FocusNavigationDirection.Right, KeyboardNavigation.TabNavigationProperty));
            Assert.AreEqual(child3, KeyboardNavigationTarget.FindTarget(child5, FocusNavigationDirection.Up, KeyboardNavigation.TabNavigationProperty));
            Assert.AreEqual(child5, KeyboardNavigationTarget.FindTarget(child5, FocusNavigationDirection.Down, KeyboardNavigation.TabNavigationProperty));
        }

        private void KeyboardNavigationInnerScopeTest(KeyboardNavigationMode innerScopeMode, Orientation orientation, int[] childrenTabIndex, FocusNavigationDirection direction, int[] expectedTargetChildrenIndex)
        {
            Control[] children = new[]
            {
                new Control { Name = "child0", Width = 100, Height = 100 },
                new Control { Name = "child1", Width = 100, Height = 100 },
                new Control { Name = "child2", Width = 100, Height = 100 },
                new Control { Name = "child3", Width = 100, Height = 100 },
                new Control { Name = "child4", Width = 100, Height = 100 },
            };

            if (childrenTabIndex != null)
            {
                for (int i = 0; i < childrenTabIndex.Length; i++)
                {
                    KeyboardNavigation.SetTabIndex(children[i], childrenTabIndex[i]);
                }
            }

            StackPanel panel = new StackPanel { Name = "panel", Orientation = orientation};
            panel.Children.Add(children[1]);
            panel.Children.Add(children[2]);
            panel.Children.Add(children[3]);

            StackPanel root = new StackPanel { Name = "root", Orientation = orientation, IsRootElement = true };
            root.Children.Add(children[0]);
            root.Children.Add(panel);
            root.Children.Add(children[4]);

            KeyboardNavigation.SetTabNavigation(root, KeyboardNavigationMode.Cycle);
            KeyboardNavigation.SetTabNavigation(panel, innerScopeMode);

            for (int i = 0; i < expectedTargetChildrenIndex.Length; i++)
            {
                Visual currentChild = children[i];
                Visual expectedTargetChild = children[expectedTargetChildrenIndex[i]];
                Visual targetChild = KeyboardNavigationTarget.FindTarget(currentChild, direction, KeyboardNavigation.TabNavigationProperty);

                Assert.AreEqual(expectedTargetChild, targetChild);
            }
        }
    }
}
