using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Core.Tokens
{
    /// <summary>
    /// Represents a token expression using multiple tokens and having N cases and an optional default.
    /// </summary>
    [TokenExpression("\\/.*?\\/")]
    internal class AdvancedTokenExpression : ITokenExpression
    {
        private static readonly char[] expressionBoundary = new[] { '/', '/' };
        private const char simpleDelimiter = '|';
        private const char caseDelimiter = ':';
        private const char equals = '=';
        private const char lessThan = '<';
        private const char moreThan = '>';

        public string Resolve(string tokenExpression, IDictionary<string, object> tokenDictionary)
        {
            var trimmedTokenExpression = tokenExpression.Trim(expressionBoundary);

            var expression = GetExpression(trimmedTokenExpression);

            var resolved = false;

            var resolvedValue = string.Empty;

            foreach (var (caseKey, caseValue) in expression.expressionCases)
            {
                resolved = TryResolveToken(tokenDictionary, caseKey, caseValue, out resolvedValue);

                if (resolved) break;
            }

            if (resolved)
            {
                return resolvedValue;
            }

            if (!string.IsNullOrEmpty(expression.defaultValue) && tokenDictionary.TryGetValue(expression.defaultValue, out object token))
            {
                return token.ToString();
            }

            return expression.defaultValue ?? string.Empty;
        }

        private (IEnumerable<((string token, string value, char operation) aaseKey, string caseValue)> expressionCases, string defaultValue) GetExpression(string tokenExpression)
        {
            if (string.IsNullOrEmpty(tokenExpression))
            {
                throw new ArgumentException($"'{tokenExpression}' looks like an advanced token expression but does not contain a key.");
            }

            string defaultValue = null;

            var casePairs = tokenExpression
                .Split(new[] { simpleDelimiter }, StringSplitOptions.RemoveEmptyEntries)
                .Select(casePair => GetCasePair(casePair, ref defaultValue))
                .Where(casePair => casePair.caseValue != null)
                .ToList();

            return (casePairs, defaultValue);
        }

        private static ((string, string, char), string caseValue) GetCasePair(string casePair, ref string defaultValue)
        {
            var pair = casePair.Split(new[] { caseDelimiter }, StringSplitOptions.RemoveEmptyEntries);

            if (pair.Length < 2)
            {
                defaultValue = pair[0];
                return ((null, null, char.MinValue), null);
            }

            return (GetCaseKey(pair[0]), pair[1]);
        }

        private static (string, string, char) GetCaseKey(string caseKey)
        {
            var key = caseKey.Split(new[] { equals, lessThan, moreThan }, StringSplitOptions.RemoveEmptyEntries);
            char operation = equals;

            switch (key.Length)
            {
                case 1:
                    return (key[0], null, operation);

                case 2:
                    if (caseKey.Contains(moreThan)) operation = moreThan;
                    if (caseKey.Contains(lessThan)) operation = lessThan;

                    return (key[0], key[1], operation);
            }

            throw new ArgumentException($"Case key '{caseKey}' looks like an advanced case key but does not contain zero or one {equals}.");
        }

        private bool TryResolveToken(IDictionary<string, object> tokenDictionary, (string token, string value, char operation) caseKey, string caseValue, out string resolvedValue)
        {
            var valueExists = tokenDictionary.TryGetValue(caseKey.token, out object token);

            if (valueExists)
            {
                switch (token)
                {
                    case int intValue when token is int && intValue == 1:
                    case int lessThanValue when token is int && caseKey.operation == lessThan && lessThanValue < int.Parse(caseKey.value.ToString()):
                    case int moreThanValue when token is int && caseKey.operation == moreThan && moreThanValue > int.Parse(caseKey.value.ToString()):
                    case var _ when token?.ToString() == caseKey.value:
                        resolvedValue = caseValue;

                        return true;
                }
            }

            resolvedValue = null;

            return false;
        }
    }
}