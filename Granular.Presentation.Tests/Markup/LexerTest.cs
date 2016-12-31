using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Markup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Markup
{
    [TestClass]
    public class LexerTest
    {
        [TestMethod]
        public void LexerBasicTest()
        {
            Lexer lexer = new Lexer(
                new RegexTokenDefinition("A", new Granular.Compatibility.Regex("^a+")),
                new RegexTokenDefinition("B", new Granular.Compatibility.Regex("^b+")));

            IEnumerator<Token> tokenEnumerator = lexer.GetTokens("aaa b   a \t bbb").GetEnumerator();

            Assert.IsTrue(tokenEnumerator.MoveNext());
            Assert.AreEqual(0, tokenEnumerator.Current.Start);
            Assert.AreEqual("A", tokenEnumerator.Current.Id);
            Assert.AreEqual("aaa", tokenEnumerator.Current.Value);

            Assert.IsTrue(tokenEnumerator.MoveNext());
            Assert.AreEqual(4, tokenEnumerator.Current.Start);
            Assert.AreEqual("B", tokenEnumerator.Current.Id);
            Assert.AreEqual("b", tokenEnumerator.Current.Value);

            Assert.IsTrue(tokenEnumerator.MoveNext());
            Assert.AreEqual(8, tokenEnumerator.Current.Start);
            Assert.AreEqual("A", tokenEnumerator.Current.Id);
            Assert.AreEqual("a", tokenEnumerator.Current.Value);

            Assert.IsTrue(tokenEnumerator.MoveNext());
            Assert.AreEqual(12, tokenEnumerator.Current.Start);
            Assert.AreEqual("B", tokenEnumerator.Current.Id);
            Assert.AreEqual("bbb", tokenEnumerator.Current.Value);

            Assert.IsFalse(tokenEnumerator.MoveNext());
        }
    }
}
