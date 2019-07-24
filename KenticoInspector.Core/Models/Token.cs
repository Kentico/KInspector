using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace KenticoInspector.Core.Models
{
    internal abstract class Token
    {
        private static string SimpleRegexPattern => $"({Opener}.*?{Closer})";

        private static char Opener => '<';

        private static char Closer => '>';

        protected string TokenExpression { get; set; }

        protected Token(string tokenExpression)
        {
            TokenExpression = tokenExpression;
        }

        internal static IEnumerable<Token> ToTokens(string source)
        {
            return Regex.Split(source, SimpleRegexPattern)
                .Select(AsToken);
        }

        private static Token AsToken(string rawTokenString)
        {
            var trimmedTokenString = rawTokenString.Trim(new[] { Opener, Closer });

            switch (trimmedTokenString)
            {
                case var tokenExpression when Regex.IsMatch(rawTokenString, SimpleRegexPattern):
                    return new SimpleToken(tokenExpression);

                default:
                    return new EmptyToken(rawTokenString);
            }
        }

        internal virtual string FillFrom(IDictionary<string, object> valuesDictionary)
        {
            return TokenExpression;
        }
    }
}