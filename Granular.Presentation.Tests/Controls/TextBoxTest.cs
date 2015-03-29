using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Controls
{
    [TestClass]
    public class TextBoxTest
    {
        private static readonly string TestText = "012\r\n3456\n7\r\n\n89";

        [TestMethod]
        public void TextBoxGetCharacterIndexFromLineIndexTest()
        {
            TextBox textBox = new TextBox { Text = TestText };
            Assert.AreEqual(-1, textBox.GetCharacterIndexFromLineIndex(-1));
            Assert.AreEqual(TestText.IndexOf("012"), textBox.GetCharacterIndexFromLineIndex(0));
            Assert.AreEqual(TestText.IndexOf("3456"), textBox.GetCharacterIndexFromLineIndex(1));
            Assert.AreEqual(TestText.IndexOf("7"), textBox.GetCharacterIndexFromLineIndex(2));
            Assert.AreEqual(TestText.IndexOf("89") - 1, textBox.GetCharacterIndexFromLineIndex(3));
            Assert.AreEqual(TestText.IndexOf("89"), textBox.GetCharacterIndexFromLineIndex(4));
            Assert.AreEqual(-1, textBox.GetCharacterIndexFromLineIndex(5));
        }

        [TestMethod]
        public void TextBoxGetLineIndexFromCharacterIndexTest()
        {
            TextBox textBox = new TextBox { Text = TestText };
            Assert.AreEqual(-1, textBox.GetCharacterIndexFromLineIndex(-1));
            Assert.AreEqual(-1, textBox.GetCharacterIndexFromLineIndex(20));
            Assert.AreEqual(0, textBox.GetLineIndexFromCharacterIndex(TestText.IndexOf("0")));
            Assert.AreEqual(0, textBox.GetLineIndexFromCharacterIndex(TestText.IndexOf("1")));
            Assert.AreEqual(0, textBox.GetLineIndexFromCharacterIndex(TestText.IndexOf("2")));
            Assert.AreEqual(0, textBox.GetLineIndexFromCharacterIndex(TestText.IndexOf("2") + 1));
            Assert.AreEqual(0, textBox.GetLineIndexFromCharacterIndex(TestText.IndexOf("2") + 2));
            Assert.AreEqual(1, textBox.GetLineIndexFromCharacterIndex(TestText.IndexOf("3")));
            Assert.AreEqual(1, textBox.GetLineIndexFromCharacterIndex(TestText.IndexOf("4")));
            Assert.AreEqual(1, textBox.GetLineIndexFromCharacterIndex(TestText.IndexOf("5")));
            Assert.AreEqual(1, textBox.GetLineIndexFromCharacterIndex(TestText.IndexOf("6")));
            Assert.AreEqual(2, textBox.GetLineIndexFromCharacterIndex(TestText.IndexOf("7")));
            Assert.AreEqual(4, textBox.GetLineIndexFromCharacterIndex(TestText.IndexOf("8")));
            Assert.AreEqual(4, textBox.GetLineIndexFromCharacterIndex(TestText.IndexOf("9")));
        }

        [TestMethod]
        public void TextBoxGetLineLengthTest()
        {
            TextBox textBox = new TextBox { Text = TestText };
            Assert.AreEqual(-1, textBox.GetLineLength(-1));
            Assert.AreEqual(3, textBox.GetLineLength(0));
            Assert.AreEqual(4, textBox.GetLineLength(1));
            Assert.AreEqual(1, textBox.GetLineLength(2));
            Assert.AreEqual(0, textBox.GetLineLength(3));
            Assert.AreEqual(2, textBox.GetLineLength(4));
            Assert.AreEqual(-1, textBox.GetLineLength(5));
        }

        [TestMethod]
        public void TextBoxGetLineTextTest()
        {
            TextBox textBox = new TextBox { Text = TestText };
            Assert.AreEqual(String.Empty, textBox.GetLineText(-1));
            Assert.AreEqual("012", textBox.GetLineText(0));
            Assert.AreEqual("3456", textBox.GetLineText(1));
            Assert.AreEqual("7", textBox.GetLineText(2));
            Assert.AreEqual("", textBox.GetLineText(3));
            Assert.AreEqual("89", textBox.GetLineText(4));
            Assert.AreEqual(String.Empty, textBox.GetLineText(5));
        }
    }
}
