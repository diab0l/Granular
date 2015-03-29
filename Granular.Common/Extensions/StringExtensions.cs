using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string @this)
        {
            return String.IsNullOrEmpty(@this);
        }

        public static bool IsNullOrWhitespace(this string @this)
        {
            return Granular.Compatibility.String.IsNullOrWhitespace(@this);
        }

        public static string DefaultIfNullOrEmpty(this string @this, string defaultValue = null)
        {
            return String.IsNullOrEmpty(@this) ? (defaultValue ?? String.Empty) : @this;
        }

        public static int GetCharacterIndexFromLineIndex(this string @this, int lineIndex)
        {
            int[] linesStartIndex = new[] { 0 }.Concat(@this.IndexOfAll("\n").Select(index => index + 1)).ToArray();

            return lineIndex >= 0 && lineIndex < linesStartIndex.Length ? linesStartIndex[lineIndex] : -1;
        }

        public static int GetLineIndexFromCharacterIndex(this string @this, int charIndex)
        {
            int[] linesIndex = @this.IndexOfAll("\n");
            int[] linesStartIndex = new[] { 0 }.Concat(linesIndex.Select(index => index + 1)).ToArray();
            int[] linesEndIndex = linesIndex.Concat(new[] { @this.Length }).ToArray();

            for (int i = 0; i < linesStartIndex.Length; i++)
            {
                if (linesStartIndex[i] <= charIndex && charIndex <= linesEndIndex[i])
                {
                    return i;
                }
            }

            return -1;
        }

        public static int GetLineLength(this string @this, int lineIndex)
        {
            string[] lines = @this.GetLines();
            return lineIndex >= 0 && lineIndex < lines.Length ? lines[lineIndex].Length : -1;
        }

        public static string GetLineText(this string @this, int lineIndex)
        {
            string[] lines = @this.GetLines();
            return lineIndex >= 0 && lineIndex < lines.Length ? lines[lineIndex] : String.Empty;
        }

        public static string[] GetLines(this string @this)
        {
            int[] linesIndex = @this.IndexOfAll("\n");

            IEnumerable<int> linesStartIndex = new [] { 0 }.Concat(linesIndex.Select(index => index + 1));
            IEnumerable<int> linesEndIndex = linesIndex.Concat(new[] { @this.Length });

            return linesStartIndex.Zip(linesEndIndex, (lineStartIndex, lineEndIndex) => @this.Substring(lineStartIndex, lineEndIndex - lineStartIndex).TrimEnd('\r')).ToArray();
        }

        public static int[] IndexOfAll(this string @this, string allOf)
        {
            List<int> list = new List<int>();

            for (int i = 0; i < @this.Length - allOf.Length; i++)
            {
                bool match = true;

                for (int j = 0; j < allOf.Length; j++)
                {
                    if (@this[i + j] != allOf[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    list.Add(i);
                }
            }

            return list.ToArray();
        }
    }
}
