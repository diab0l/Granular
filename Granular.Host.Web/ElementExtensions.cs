using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using System.Text;

namespace Granular.Host
{
    public static class ElementExtensions
    {
        [InlineCode("{element}.setSelectionRange({index}, {index})")]
        public static void SetCaretIndex(this Element element, int index)
        {
            //
        }

        [InlineCode("{element}.selectionStart")]
        public static int GetSelectionStart(this Element element)
        {
            return 0;
        }

        [InlineCode("{element}.selectionStart = {value}")]
        public static void SetSelectionStart(this Element element, int value)
        {
            //
        }

        [InlineCode("{element}.selectionEnd")]
        public static int GetSelectionEnd(this Element element)
        {
            return 0;
        }

        [InlineCode("{element}.selectionEnd = {value}")]
        public static void SetSelectionEnd(this Element element, int value)
        {
            //
        }

        [InlineCode("{element}.value")]
        public static string GetValue(this Element element)
        {
            return null;
        }

        [InlineCode("{element}.value = {value}")]
        public static void SetValue(this Element element, string value)
        {
            //
        }
    }
}
