using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xaml;
using Granular.Collections;

namespace System.Windows.Markup
{
    public class PropertyPathParser
    {
        private enum TokenType { Terminal, Value }

        private static readonly Lexer lexer = new Lexer
        (
            new RegexTokenDefinition(TokenType.Terminal, new Regex(@"[\(\)\[\]\.,]")),
            new RegexTokenDefinition(TokenType.Value, new Regex(@"[^\(\)\[\]\.\,]*"))
        );

        private string text;
        private XamlNamespaces namespaces;
        private ReadOnlyStack<Token> tokens;

        public PropertyPathParser(string text, XamlNamespaces namespaces)
        {
            this.text = text;
            this.namespaces = namespaces;
        }

        public IEnumerable<IPropertyPathElement> Parse()
        {
            this.tokens = new ReadOnlyStack<Token>(lexer.GetTokens(text));

            List<IPropertyPathElement> elements = new List<IPropertyPathElement>();

            elements.Add(MatchElement(namespaces));

            while (!tokens.IsEmpty)
            {
                MatchTerminal(".");
                elements.Add(MatchElement(namespaces));
            }

            return elements;
        }

        private IPropertyPathElement MatchElement(XamlNamespaces namespaces)
        {
            VerifyTokensExists();

            XamlName propertyName = TryMatchPropertyName(namespaces);
            IEnumerable<string> indexRawValues = TryMatchIndexRawValues();

            if (propertyName.IsEmpty && !indexRawValues.Any())
            {
                throw new Granular.Exception("Can't parse \"{0}\", Property name or Index parameters were expected, \"{1}\" was found at index {2}", text, tokens.Peek().Value, tokens.Peek().Start);
            }

            return indexRawValues.Any() ? (IPropertyPathElement) new IndexPropertyPathElement(propertyName, indexRawValues, namespaces) : new PropertyPathElement(propertyName);
        }

        private XamlName TryMatchPropertyName(XamlNamespaces namespaces)
        {
            if (!tokens.IsEmpty && tokens.Peek().Value == "(")
            {
                MatchTerminal("(");

                string propertyName = MatchValue();

                if (!tokens.IsEmpty && tokens.Peek().Value == ".")
                {
                    MatchTerminal(".");
                    propertyName = String.Format("{0}.{1}", propertyName, MatchValue());
                }

                MatchTerminal(")");

                XamlName xamlName = XamlName.FromPrefixedName(propertyName, namespaces);

                if (xamlName.IsEmpty)
                {
                    throw new Granular.Exception("Can't parse \"{0}\", Can't parse property name \"{1}\" at index {2}, is namespace missing?", text, propertyName, tokens.Peek().Start - propertyName.Length - 1);
                }

                return xamlName;
            }

            if (!tokens.IsEmpty && (TokenType)tokens.Peek().Id == TokenType.Value)
            {
                return new XamlName(MatchValue());
            }

            return XamlName.Empty;
        }

        private IEnumerable<string> TryMatchIndexRawValues()
        {
            List<string> values = new List<string>();

            if (!tokens.IsEmpty && tokens.Peek().Value == "[")
            {
                MatchTerminal("[");
                values.Add(MatchValue());

                while (!tokens.IsEmpty && tokens.Peek().Value == ",")
                {
                    MatchTerminal(",");
                    values.Add(MatchValue());
                }

                MatchTerminal("]");
            }

            return values;
        }

        private string MatchValue()
        {
            VerifyTokensExists();

            Token token = tokens.Pop();

            if ((TokenType)token.Id != TokenType.Value)
            {
                throw new Granular.Exception("Can't parse \"{0}\", \"{1}\" was not expected at index {2}", text, token.Value, token.Start);
            }

            return token.Value;
        }

        private Token MatchTerminal(string terminal)
        {
            VerifyTokensExists();

            Token token = tokens.Pop();

            if ((TokenType)token.Id != TokenType.Terminal || token.Value != terminal)
            {
                throw new Granular.Exception("Can't parse \"{0}\", \"{1}\" is expected, \"{2}\" was found at index {3}", text, terminal, token.Value, token.Start);
            }

            return token;
        }

        private void VerifyTokensExists()
        {
            if (tokens.IsEmpty)
            {
                throw new Granular.Exception("Can't parse \"{0}\", stream was terminated unexpectedly", text);
            }
        }
    }
}
