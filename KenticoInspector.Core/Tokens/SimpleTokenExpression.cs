using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Core.Tokens
{
    /// <summary>
    /// Represents tokens using a single token and having N cases and an optional default.
    /// </summary>
    [TokenExpression("(<.*?>)")]
    internal class SimpleTokenExpression : ITokenExpression
    {
        private static readonly char[] expressionBoundary = new[] { '<', '>' };
        private const char simpleDelimiter = '|';
        private const char caseDelimiter = ':';

        public string Resolve(string tokenExpression, IDictionary<string, object> tokenDictionary)
        {
            var trimmedTokenExpression = tokenExpression.Trim(expressionBoundary);

            var expression = GetExpression(trimmedTokenExpression);

            var resolved = false;

            var resolvedValue = string.Empty;

            if (tokenDictionary.TryGetValue(expression.token, out object token))
            {
                foreach (var (value, caseValue) in expression.expressionCases)
                {
                    resolved = TryResolveToken(token, value, caseValue, out resolvedValue);

                    if (resolved) break;
                }

                if (resolved)
                {
                    return resolvedValue;
                }

                if (expression.token == expression.defaultValue)
                {
                    return token.ToString();
                }
            }
            else
            {
                return string.Empty;
            }

            return expression.defaultValue ?? string.Empty;
        }

        private (string token, IEnumerable<(string value, string caseValue)> expressionCases, string defaultValue) GetExpression(string tokenExpression)
        {
            if (string.IsNullOrEmpty(tokenExpression))
            {
                throw new ArgumentException($"'{tokenExpression}' looks like a simple token expression but does not contain a key.");
            }

            var token = tokenExpression;

            var indexOfFirstPipe = tokenExpression.IndexOf(simpleDelimiter);

            if (indexOfFirstPipe > -1)
            {
                token = tokenExpression.Substring(0, indexOfFirstPipe);
            }

            string defaultValue = null;

            var casePairs = tokenExpression
                .Split(new[] { simpleDelimiter }, StringSplitOptions.RemoveEmptyEntries)
                .AsEnumerable();

            if (casePairs.Count() > 1) casePairs = casePairs.Skip(1);

            var simpleCasePairs = casePairs
                .Select(casePair => GetCasePair(casePair, ref defaultValue))
                .ToList();

            return (token, simpleCasePairs, defaultValue);
        }

        private static (string, string) GetCasePair(string casePair, ref string defaultValue)
        {
            var pair = casePair.Split(new[] { caseDelimiter }, StringSplitOptions.RemoveEmptyEntries);

            switch (pair.Length)
            {
                case 1:
                    defaultValue = pair[0];

                    return (null, pair[0]);

                case 2:
                    return (pair[0], pair[1]);
            }

            throw new ArgumentException($"Case pair '{casePair}' looks like a simple case key but does not contain zero or one {caseDelimiter}.");
        }

        private bool TryResolveToken(object token, string value, string caseValue, out string resolvedValue)
        {
            switch (token)
            {
                case int intValue when token is int && intValue == 1:
                    resolvedValue = caseValue;

                    return true;
            }

            var valueMatches = token?.ToString() == value;

            if (valueMatches)
            {
                resolvedValue = caseValue;

                return true;
            }

            resolvedValue = null;

            return false;
        }
    }
}