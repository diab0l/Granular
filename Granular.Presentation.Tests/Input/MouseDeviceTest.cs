using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Input
{
    [TestClass]
    public class MouseDeviceTest
    {
        [TestMethod]
        public void MouseDeviceEnterLeaveTest()
        {
            Border child1 = new Border { Background = Brushes.Transparent, Width = 40, Height = 40 };
            Border child2 = new Border { Background = Brushes.Transparent, Width = 40, Height = 40 };
            Canvas.SetTop(child1, 20);
            Canvas.SetTop(child2, 20);
            Canvas.SetLeft(child1, 20);
            Canvas.SetLeft(child2, 80);

            Canvas canvas = new Canvas { Background = Brushes.Transparent, Width = 140, Height = 80, Margin = new Thickness(20), IsRootElement = true };
            canvas.Children.Add(child1);
            canvas.Children.Add(child2);
            canvas.Measure(Size.Infinity);
            canvas.Arrange(new Rect(canvas.DesiredSize));

            TestPresentationSource presentationSource = new TestPresentationSource();
            presentationSource.RootElement = canvas;

            MouseDevice mouseDevice = new MouseDevice(presentationSource);

            List<object> enterList = new List<object>();
            List<object> leaveList = new List<object>();

            canvas.MouseEnter += (sender, e) => enterList.Add(sender);
            canvas.MouseLeave += (sender, e) => leaveList.Add(sender);
            child1.MouseEnter += (sender, e) => enterList.Add(sender);
            child1.MouseLeave += (sender, e) => leaveList.Add(sender);
            child2.MouseEnter += (sender, e) => enterList.Add(sender);
            child2.MouseLeave += (sender, e) => leaveList.Add(sender);

            mouseDevice.Activate();

            mouseDevice.ProcessRawEvent(new RawMouseEventArgs(new Point(10, 10), 0));
            Assert.AreEqual(0, enterList.Count());
            Assert.AreEqual(0, leaveList.Count());
            Assert.IsFalse(canvas.IsMouseOver);
            Assert.IsFalse(child1.IsMouseOver);
            Assert.IsFalse(child2.IsMouseOver);

            mouseDevice.ProcessRawEvent(new RawMouseEventArgs(new Point(30, 30), 0));
            CollectionAssert.AreEqual(enterList, new object[] { canvas });
            Assert.AreEqual(0, leaveList.Count());
            Assert.IsTrue(canvas.IsMouseOver);
            Assert.IsFalse(child1.IsMouseOver);
            Assert.IsFalse(child2.IsMouseOver);

            mouseDevice.ProcessRawEvent(new RawMouseEventArgs(new Point(60, 60), 0));
            CollectionAssert.AreEqual(enterList, new object[] { canvas, child1 });
            Assert.AreEqual(0, leaveList.Count());
            Assert.IsTrue(canvas.IsMouseOver);
            Assert.IsTrue(child1.IsMouseOver);
            Assert.IsFalse(child2.IsMouseOver);

            mouseDevice.ProcessRawEvent(new RawMouseEventArgs(new Point(120, 60), 0));
            CollectionAssert.AreEqual(enterList, new object[] { canvas, child1, child2 });
            CollectionAssert.AreEqual(leaveList, new object[] { child1 });
            Assert.IsTrue(canvas.IsMouseOver);
            Assert.IsFalse(child1.IsMouseOver);
            Assert.IsTrue(child2.IsMouseOver);

            mouseDevice.Deactivate();
            CollectionAssert.AreEqual(enterList, new object[] { canvas, child1, child2 });
            CollectionAssert.AreEqual(leaveList, new object[] { child1, child2, canvas });
            Assert.IsFalse(canvas.IsMouseOver);
            Assert.IsFalse(child1.IsMouseOver);
            Assert.IsFalse(child2.IsMouseOver);

            mouseDevice.Activate();
            CollectionAssert.AreEqual(enterList, new object[] { canvas, child1, child2, canvas, child2 });
            CollectionAssert.AreEqual(leaveList, new object[] { child1, child2, canvas });
            Assert.IsTrue(canvas.IsMouseOver);
            Assert.IsFalse(child1.IsMouseOver);
            Assert.IsTrue(child2.IsMouseOver);
        }

        [TestMethod]
        public void MouseDeviceButtonTest()
        {
            Border element = new Border { Background = Brushes.Transparent, Width = 100, Height = 100, IsRootElement = true };
            element.Measure(Size.Infinity);
            element.Arrange(new Rect(element.DesiredSize));

            int eventIndex = 0;
            int previewMouseDownIndex = 0;
            int previewMouseUpIndex = 0;
            int mouseDownIndex = 0;
            int mouseUpIndex = 0;

            element.PreviewMouseDown += (sender, e) => previewMouseDownIndex = ++eventIndex;
            element.PreviewMouseUp += (sender, e) => previewMouseUpIndex = ++eventIndex;
            element.MouseDown += (sender, e) => mouseDownIndex = ++eventIndex;
            element.MouseUp += (sender, e) => mouseUpIndex = ++eventIndex;

            TestPresentationSource presentationSource = new TestPresentationSource();
            presentationSource.RootElement = element;

            MouseDevice mouseDevice = new MouseDevice(presentationSource);

            mouseDevice.Activate();

            mouseDevice.ProcessRawEvent(new RawMouseButtonEventArgs(MouseButton.Left, MouseButtonState.Pressed, new Point(10, 10), 0));
            Assert.AreEqual(MouseButtonState.Pressed, mouseDevice.GetButtonState(MouseButton.Left));
            Assert.AreEqual(1, previewMouseDownIndex);
            Assert.AreEqual(2, mouseDownIndex);

            mouseDevice.ProcessRawEvent(new RawMouseButtonEventArgs(MouseButton.Left, MouseButtonState.Released, new Point(10, 10), 0));
            Assert.AreEqual(MouseButtonState.Released, mouseDevice.GetButtonState(MouseButton.Left));
            Assert.AreEqual(3, previewMouseUpIndex);
            Assert.AreEqual(4, mouseUpIndex);

            mouseDevice.ProcessRawEvent(new RawMouseButtonEventArgs(MouseButton.Left, MouseButtonState.Pressed, new Point(10, 10), 0));
            Assert.AreEqual(MouseButtonState.Pressed, mouseDevice.GetButtonState(MouseButton.Left));
            Assert.AreEqual(5, previewMouseDownIndex);
            Assert.AreEqual(6, mouseDownIndex);

            mouseDevice.Deactivate();
            Assert.AreEqual(MouseButtonState.Released, mouseDevice.GetButtonState(MouseButton.Left));
            Assert.AreEqual(7, previewMouseUpIndex);
            Assert.AreEqual(8, mouseUpIndex);
        }

        [TestMethod]
        public void MouseDeviceCaptureTest()
        {
            Border child1 = new Border { Background = Brushes.Transparent, Width = 100, Height = 100 };
            Border child2 = new Border { Background = Brushes.Transparent, Width = 100, Height = 100 };
            Border child3 = new Border { Background = Brushes.Transparent, Width = 100, Height = 100 };

            StackPanel panel = new StackPanel { IsRootElement = true };
            panel.Children.Add(child1);
            panel.Children.Add(child2);

            MouseEventArgs lastArgs = null;
            panel.MouseMove += (sender, e) => lastArgs = e;

            TestPresentationSource presentationSource = new TestPresentationSource();
            presentationSource.RootElement = panel;

            panel.Measure(Size.Infinity);
            panel.Arrange(new Rect(panel.DesiredSize));

            MouseDevice mouseDevice = new MouseDevice(presentationSource);

            mouseDevice.Activate();
            mouseDevice.ProcessRawEvent(new RawMouseEventArgs(new Point(50, 50), 0));
            Assert.AreEqual(child1, lastArgs.Source);
            Assert.AreEqual(child1, mouseDevice.HitTarget);
            Assert.AreEqual(null, mouseDevice.CaptureTarget);
            Assert.AreEqual(child1, mouseDevice.Target);

            mouseDevice.ProcessRawEvent(new RawMouseEventArgs(new Point(50, 150), 0));
            Assert.AreEqual(child2, lastArgs.Source);
            Assert.AreEqual(child2, mouseDevice.HitTarget);
            Assert.AreEqual(null, mouseDevice.CaptureTarget);
            Assert.AreEqual(child2, mouseDevice.Target);

            mouseDevice.Capture(child2);
            mouseDevice.ProcessRawEvent(new RawMouseEventArgs(new Point(50, 50), 0));
            Assert.AreEqual(child2, lastArgs.Source);
            Assert.AreEqual(child1, mouseDevice.HitTarget);
            Assert.AreEqual(child2, mouseDevice.CaptureTarget);
            Assert.AreEqual(child2, mouseDevice.Target);

            mouseDevice.ReleaseCapture();
            mouseDevice.ProcessRawEvent(new RawMouseEventArgs(new Point(50, 50), 0));
            Assert.AreEqual(child1, lastArgs.Source);
            Assert.AreEqual(child1, mouseDevice.HitTarget);
            Assert.AreEqual(null, mouseDevice.CaptureTarget);
            Assert.AreEqual(child1, mouseDevice.Target);
        }

        [TestMethod]
        public void MouseDeviceQueryCursorTest()
        {
            Border child1 = new Border { Background = Brushes.Transparent, Width = 100, Height = 100 };
            Border child2 = new Border { Background = Brushes.Transparent, Width = 100, Height = 100 };

            StackPanel panel = new StackPanel { IsRootElement = true };
            panel.Children.Add(child1);
            panel.Children.Add(child2);

            TestPresentationSource presentationSource = new TestPresentationSource();
            presentationSource.RootElement = panel;

            ((TestApplicationHost)ApplicationHost.Current).PresentationSourceFactory = new TestPresentationSourceFactory(presentationSource);

            panel.Measure(Size.Infinity);
            panel.Arrange(new Rect(panel.DesiredSize));

            child1.Cursor = Cursors.Help;
            child2.Cursor = Cursors.Pen;

            presentationSource.MouseDevice.Activate();
            Assert.AreEqual(Cursors.Arrow, presentationSource.MouseDevice.Cursor);

            presentationSource.MouseDevice.ProcessRawEvent(new RawMouseEventArgs(new Point(50, 50), 0));
            Assert.AreEqual(Cursors.Help, presentationSource.MouseDevice.Cursor);

            presentationSource.MouseDevice.ProcessRawEvent(new RawMouseEventArgs(new Point(50, 150), 0));
            Assert.AreEqual(Cursors.Pen, presentationSource.MouseDevice.Cursor);

            child2.Cursor = Cursors.Hand;
            Assert.AreEqual(Cursors.Hand, presentationSource.MouseDevice.Cursor);

            panel.Cursor = Cursors.Arrow;
            Assert.AreEqual(Cursors.Hand, presentationSource.MouseDevice.Cursor);

            panel.ForceCursor = true;
            Assert.AreEqual(Cursors.Arrow, presentationSource.MouseDevice.Cursor);

            ((TestApplicationHost)ApplicationHost.Current).PresentationSourceFactory = null;
        }
    }
}
