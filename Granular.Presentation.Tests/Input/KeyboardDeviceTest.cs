using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Input
{
    [TestClass]
    public class KeyboardDeviceTest
    {
        [TestMethod]
        public void KeyboardDeviceBasicTest()
        {
            FrameworkElement element = new FrameworkElement();

            int eventIndex = 0;

            int previewKeyDownIndex = 0;
            int previewKeyUpIndex = 0;
            int previewGotKeyboardFocusIndex = 0;
            int previewLostKeyboardFocusIndex = 0;
            int keyDownIndex = 0;
            int keyUpIndex = 0;
            int gotKeyboardFocusIndex = 0;
            int lostKeyboardFocusIndex = 0;

            element.PreviewKeyDown += (sender, e) => previewKeyDownIndex = ++eventIndex;
            element.PreviewKeyUp += (sender, e) => previewKeyUpIndex = ++eventIndex;
            element.PreviewGotKeyboardFocus += (sender, e) => previewGotKeyboardFocusIndex = ++eventIndex;
            element.PreviewLostKeyboardFocus += (sender, e) => previewLostKeyboardFocusIndex = ++eventIndex;
            element.KeyDown += (sender, e) => keyDownIndex = ++eventIndex;
            element.KeyUp += (sender, e) => keyUpIndex = ++eventIndex;
            element.GotKeyboardFocus += (sender, e) => gotKeyboardFocusIndex = ++eventIndex;
            element.LostKeyboardFocus += (sender, e) => lostKeyboardFocusIndex = ++eventIndex;

            TestPresentationSource presentationSource = new TestPresentationSource();
            KeyboardDevice keyboardDevice = new KeyboardDevice(presentationSource);

            keyboardDevice.Activate();

            IDisposable focus = keyboardDevice.Focus(element);
            Assert.AreEqual(1, previewGotKeyboardFocusIndex);
            Assert.AreEqual(2, gotKeyboardFocusIndex);
            Assert.IsTrue(element.IsKeyboardFocused);

            keyboardDevice.ProcessRawEvent(new RawKeyboardEventArgs(Key.Enter, KeyStates.Down, false, 0));
            Assert.AreEqual(KeyStates.Down, keyboardDevice.GetKeyStates(Key.Enter));
            Assert.AreEqual(3, previewKeyDownIndex);
            Assert.AreEqual(4, keyDownIndex);

            keyboardDevice.ProcessRawEvent(new RawKeyboardEventArgs(Key.Enter, KeyStates.None, false, 0));
            Assert.AreEqual(KeyStates.None, keyboardDevice.GetKeyStates(Key.Enter));
            Assert.AreEqual(5, previewKeyUpIndex);
            Assert.AreEqual(6, keyUpIndex);

            focus.Dispose();
            Assert.AreEqual(7, previewLostKeyboardFocusIndex);
            Assert.AreEqual(8, lostKeyboardFocusIndex);
            Assert.IsFalse(element.IsKeyboardFocused);

            focus = keyboardDevice.Focus(element);
            keyboardDevice.ProcessRawEvent(new RawKeyboardEventArgs(Key.Enter, KeyStates.Down, false, 0));
            focus.Dispose();
            Assert.AreEqual(13, previewKeyUpIndex);
            Assert.AreEqual(14, keyUpIndex);
            Assert.AreEqual(15, previewLostKeyboardFocusIndex);
            Assert.AreEqual(16, lostKeyboardFocusIndex);

            keyboardDevice.Focus(element);
            keyboardDevice.ProcessRawEvent(new RawKeyboardEventArgs(Key.Enter, KeyStates.Down, false, 0));
            keyboardDevice.Deactivate();
            Assert.AreEqual(23, previewKeyUpIndex);
            Assert.AreEqual(24, keyUpIndex);
        }
    }
}
