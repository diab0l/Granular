using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Markup
{
    public class Token
    {
        public object Id { get; private set; }
        public string Value { get; private set; }
        public int Start { get; private set; }

        public Token(object id, string value, int start)
        {
            this.Id = id;
            this.Value = value;
            this.Start = start;
        }

        public override string ToString()
        {
            return String.Format("{0} \"{1}\" ({2})", Id, Value, Start);
        }
    }

    public interface ITokenDefinition
    {
        Token Match(string stream, int start);
    }

    public class RegexTokenDefinition : ITokenDefinition
    {
        private object id;
        private Granular.Compatibility.Regex regex;

        public RegexTokenDefinition(object id, Granular.Compatibility.Regex regex)
        {
            this.id = id;
            this.regex = regex;
        }

        public Token Match(string stream, int start)
        {
            string[] matches = regex.Match(stream.Substring(start));

            if (matches != null)
            {
                return new Token(id, matches[0], start);
            }

            return null;
        }
    }

    public class Lexer
    {
        private static readonly Granular.Compatibility.Regex WhiteSpaceRegex = new Granular.Compatibility.Regex("^[ \t]+");
        private ITokenDefinition[] tokensDefinition;

        public Lexer(params ITokenDefinition[] tokensDefinition)
        {
            this.tokensDefinition = tokensDefinition;
        }

        public IEnumerable<Token> GetTokens(string stream)
        {
            int start = 0;

            while (start < stream.Length)
            {
                string[] matches = WhiteSpaceRegex.Match(stream.Substring(start));

                if (matches != null)
                {
                    start += matches[0].Length;
                }

                Token selectedToken = null;

                foreach (ITokenDefinition tokenDefinition in tokensDefinition)
                {
                    Token matchedToken = tokenDefinition.Match(stream, start);
                    if (matchedToken != null && (selectedToken == null || matchedToken.Value.Length > selectedToken.Value.Length))
                    {
                        selectedToken = matchedToken;
                    }
                }

                if (selectedToken == null)
                {
                    throw new Granular.Exception("Can't parse \"{0}\" at index {1} ('{2}' is unexpected)", stream, start, stream[start].ToString());
                }

                start += selectedToken.Value.Length;

                yield return selectedToken;
            }
        }
    }
}
