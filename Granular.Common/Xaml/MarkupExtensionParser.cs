using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Collections;

namespace System.Windows.Markup
{
    public class MarkupExtensionParser
    {
        private enum TokenType { Terminal, Identifier, String, Boolean, Integer, Decimal }

        private static readonly Lexer lexer = new Lexer
        (
            new RegexTokenDefinition(TokenType.Terminal, new Granular.Compatibility.Regex("^[{}=,]")),
            new RegexTokenDefinition(TokenType.Boolean, new Granular.Compatibility.Regex("^(true|True|false|False)")),
            new RegexTokenDefinition(TokenType.String, new Granular.Compatibility.Regex("^'([^']|'')*'")),
            new RegexTokenDefinition(TokenType.Integer, new Granular.Compatibility.Regex("^[0-9]+")),
            new RegexTokenDefinition(TokenType.Decimal, new Granular.Compatibility.Regex("^[0-9]*\\.[0-9]+")),
            new RegexTokenDefinition(TokenType.Identifier, new Granular.Compatibility.Regex(@"^[A-Za-z0-9_:\(\)\.]*"))
        );

        private string text;
        private XamlNamespaces namespaces;
        private Uri sourceUri;
        private ReadOnlyStack<Token> tokens;

        private MarkupExtensionParser(string text, XamlNamespaces namespaces, Uri sourceUri)
        {
            this.text = text;
            this.namespaces = namespaces;
            this.sourceUri = sourceUri;
        }

        private XamlElement Parse()
        {
            this.tokens = new ReadOnlyStack<Token>(lexer.GetTokens(text));

            XamlElement root = MatchElement();

            if (!tokens.IsEmpty)
            {
                throw new Granular.Exception("Can't parse \"{0}\", end of stream is expected at index {1}", text, tokens.Peek().Start);
            }

            return root;
        }

        // S -> { Identifier PL }
        private XamlElement MatchElement()
        {
            VerifyTokensExists();

            MatchTerminal("{");
            string typeFullName = MatchIdentifier();
            IEnumerable<XamlMember> membersList = MatchMembersList();
            MatchTerminal("}");

            return new XamlElement(new XamlName(GetTypeName(typeFullName), GetTypeNamespace(typeFullName)), namespaces, sourceUri, members: membersList);
        }

        // PL -> P PL' | epsilon
        private IEnumerable<XamlMember> MatchMembersList()
        {
            VerifyTokensExists();

            List<XamlMember> list = new List<XamlMember>();

            if (tokens.Peek().Value != "}")
            {
                list.Add(MatchMember());
                list.AddRange(MatchMembersListEnd());
            }

            return list;
        }

        // PL' -> , P PL' | epsilon
        private IEnumerable<XamlMember> MatchMembersListEnd()
        {
            VerifyTokensExists();

            List<XamlMember> list = new List<XamlMember>();

            if (tokens.Peek().Value != "}")
            {
                MatchTerminal(",");

                list.Add(MatchMember());
                list.AddRange(MatchMembersListEnd());
            }

            return list;
        }

        // P -> Identifier NV | V
        private XamlMember MatchMember()
        {
            VerifyTokensExists();

            string name = String.Empty;
            object value;

            if ((TokenType)tokens.Peek().Id == TokenType.Identifier)
            {
                string identifier = MatchIdentifier();
                value = MatchNamedValue();

                if (value == null)
                {
                    value = identifier;
                }
                else
                {
                    name = identifier;
                }
            }
            else
            {
                value = MatchValue();
            }

            return new XamlMember(new XamlName(name), namespaces, sourceUri, value);
        }

        // NV -> = V | epsilon
        private object MatchNamedValue()
        {
            VerifyTokensExists();

            Token token = tokens.Peek();
            if ((TokenType)token.Id != TokenType.Terminal || token.Value != "=")
            {
                return null;
            }

            MatchTerminal("=");

            return MatchValue();
        }

        // V -> S | Identifier | String | Boolean | Integer | Decimal
        private object MatchValue()
        {
            VerifyTokensExists();

            if ((TokenType)tokens.Peek().Id == TokenType.Terminal)
            {
                return MatchElement();
            }

            Token token = tokens.Pop();
            object constValue;

            switch ((TokenType)token.Id)
            {
                case TokenType.Identifier: constValue = token.Value; break;
                case TokenType.String: constValue = token.Value.Substring(1, token.Value.Length - 2).Replace("''", "'"); break;
                case TokenType.Boolean: constValue = BooleanParse(token.Value); break;
                case TokenType.Integer: constValue = Int32.Parse(token.Value); break;
                case TokenType.Decimal: constValue = Double.Parse(token.Value); break;
                default: throw new Granular.Exception("Can't parse \"{0}\", value is expected, \"{1}\" was found at index {2}", text, token.Value, token.Start);
            }

            return constValue;
        }

        private static bool BooleanParse(string value)
        {
            value = value.ToLower();
            if (value.CompareTo("true") == 0 || value.CompareTo("false") == 0)
            {
                return value.CompareTo("true") == 0;
            }

            throw new Granular.Exception("Can't parse boolean value \"{0}\"", value);
        }

        private string MatchIdentifier()
        {
            VerifyTokensExists();

            Token token = tokens.Pop();

            if ((TokenType)token.Id != TokenType.Identifier)
            {
                throw new Granular.Exception("Can't parse \"{0}\", identifier is expected, \"{1}\" was found at index {2}", text, token.Value, token.Start);
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

        private string GetTypeName(string typeFullName)
        {
            int namespaceSeparatorIndex = typeFullName.IndexOf(":");
            return namespaceSeparatorIndex != -1 ? typeFullName.Substring(namespaceSeparatorIndex + 1) : typeFullName;
        }

        private string GetTypeNamespace(string typeFullName)
        {
            int namespaceSeparatorIndex = typeFullName.IndexOf(":");
            return namespaces.GetNamespace(namespaceSeparatorIndex != -1 ? typeFullName.Substring(0, namespaceSeparatorIndex) : String.Empty);
        }

        public static object Parse(string text, XamlNamespaces namespaces, Uri sourceUri = null)
        {
            if (IsEscaped(text))
            {
                return GetEscapedText(text);
            }

            if (IsMarkupExtension(text))
            {
                return new MarkupExtensionParser(text, namespaces, sourceUri).Parse();
            }

            return text;
        }

        private static bool IsMarkupExtension(string text)
        {
            text = text.Trim();
            return text.StartsWith("{") && text.EndsWith("}");
        }

        private static bool IsEscaped(string text)
        {
            return text.StartsWith("{}");
        }

        private static string GetEscapedText(string text)
        {
            return text.Substring(2);
        }
    }
}
