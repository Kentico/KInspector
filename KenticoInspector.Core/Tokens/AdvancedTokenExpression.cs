using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Core.Tokens
{
    /// <summary>
    /// Represents a token expression using multiple tokens and having N cases and an optional default.
    /// </summary>
    [TokenExpression("(\\/.*?\\/)")]
    internal class AdvancedTokenExpression : ITokenExpression
    {
        private static readonly char[] expressionBoundary = new[] { '/', '/' };
        private const char simpleDelimiter = '|';
        private const char advancedDelimiter = '=';
        private const char caseDelimiter = ':';

        public string Resolve(string tokenExpression, IDictionary<string, object> tokenDictionary)
        {
            var trimmedTokenExpression = tokenExpression.Trim(expressionBoundary);

            var expression = GetExpression(trimmedTokenExpression);

            var resolved = false;

            var resolvedValue = string.Empty;

            foreach (var (advancedCaseKey, caseValue) in expression.expressionCases)
            {
                resolved = TryResolveToken(tokenDictionary, advancedCaseKey, caseValue, out resolvedValue);

                if (resolved) break;
            }

            if (resolved)
            {
                return resolvedValue;
            }

            return expression.defaultValue ?? string.Empty;
        }

        private (IEnumerable<((string token, string value) advancedCaseKey, string caseValue)> expressionCases, string defaultValue) GetExpression(string tokenExpression)
        {
            if (string.IsNullOrEmpty(tokenExpression))
            {
                throw new ArgumentException($"'{tokenExpression}' looks like an advanced token expression but does not contain a key.");
            }

            string defaultValue = null;

            var advancedCasePairs = tokenExpression
                .Split(new[] { simpleDelimiter }, StringSplitOptions.RemoveEmptyEntries)
                .Select(casePair => GetCasePair(casePair, ref defaultValue))
                .Where(advancedCasePair => advancedCasePair.caseValue != null)
                .ToList();

            return (advancedCasePairs, defaultValue);
        }

        private static ((string, string), string caseValue) GetCasePair(string casePair, ref string defaultValue)
        {
            var pair = casePair.Split(new[] { caseDelimiter }, StringSplitOptions.RemoveEmptyEntries);

            if (pair.Length < 2)
            {
                defaultValue = pair[0];
                return ((null, null), null);
            }

            return (GetAdvancedCaseKey(pair[0]), pair[1]);
        }

        private static (string, string) GetAdvancedCaseKey(string caseKey)
        {
            var key = caseKey.Split(new[] { advancedDelimiter }, StringSplitOptions.RemoveEmptyEntries);

            switch (key.Length)
            {
                case 1:
                    return (key[0], null);
                case 2:
                    return (key[0], key[1]);
            }

            throw new ArgumentException($"Case key '{caseKey}' looks like an advanced case key but does not contain zero or one {advancedDelimiter}.");
        }


        private bool TryResolveToken(IDictionary<string, object> tokenDictionary, (string token, string value) advancedCaseKey, string caseValue, out string resolvedValue)
        {
            var valueExists = tokenDictionary.TryGetValue(advancedCaseKey.token, out object token);
            var valueMatches = token?.ToString() == advancedCaseKey.value;

            if (valueExists && valueMatches)
            {
                resolvedValue = caseValue;

                return true;
            }

            resolvedValue = null;

            return false;
        }
    }
}