using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Core.Tokens
{
    /// <summary>
    /// Represents a token expression using multiple tokens and having N cases and an optional default.
    /// </summary>
    [TokenExpression("<\\?.*?>")]
    internal class AdvancedTokenExpression : ITokenExpression
    {
        private static readonly char[] expressionBoundary = new[] { '<', '>' };

        public string Resolve(string tokenExpression, IDictionary<string, object> tokenDictionary)
        {
            var trimmedTokenExpression = tokenExpression.Trim(expressionBoundary).TrimStart(Constants.Question);

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

        private (IEnumerable<((string token, string value, char operation) caseKey, string caseValue)> expressionCases, string defaultValue) GetExpression(string tokenExpression)
        {
            if (string.IsNullOrEmpty(tokenExpression))
            {
                throw new ArgumentException($"'{tokenExpression}' looks like an advanced token expression but does not contain a key.");
            }

            string defaultValue = null;

            var casePairs = tokenExpression
                .Split(new[] { Constants.Pipe }, StringSplitOptions.RemoveEmptyEntries)
                .Select(casePair => GetCasePair(casePair, ref defaultValue))
                .Where(casePair => casePair.caseValue != null)
                .ToList();

            return (casePairs, defaultValue);
        }

        private static ((string, string, char), string caseValue) GetCasePair(string casePair, ref string defaultValue)
        {
            var pair = casePair.Split(new[] { Constants.Colon }, StringSplitOptions.RemoveEmptyEntries);

            switch (pair.Length)
            {
                case 1:
                    defaultValue = pair[0];

                    return ((null, null, char.MinValue), null);

                case 2:
                    return (GetCaseKey(pair[0]), pair[1]);
            }

            if (pair.Skip(2).Any(segment => segment[0] != Constants.Space))
            {
                throw new ArgumentException($"Case pair '{casePair}' looks like an advanced case pair but does not contain zero or one {Constants.Colon}.");
            }

            return (GetCaseKey(pair[0]), pair.Skip(1).Aggregate((l, r) => $"{l}{Constants.Colon}{r}"));
        }

        private static (string, string, char) GetCaseKey(string caseKey)
        {
            char operation = Constants.Equals;

            char[] operationChars = new[] { Constants.Equals, Constants.LessThan, Constants.MoreThan };

            var key = caseKey.Split(operationChars, StringSplitOptions.RemoveEmptyEntries);

            switch (key.Length)
            {
                case 1:
                    return (key[0], null, operation);

                case 2:
                    if (caseKey.Contains(Constants.MoreThan)) operation = Constants.MoreThan;
                    if (caseKey.Contains(Constants.LessThan)) operation = Constants.LessThan;

                    return (key[0], key[1], operation);
            }

            throw new ArgumentException($"Case key '{caseKey}' looks like an advanced case key but does not contain zero or one {string.Join(',', operationChars)}.");
        }

        private bool TryResolveToken(IDictionary<string, object> tokenDictionary, (string token, string value, char operation) caseKey, string caseValue, out string resolvedValue)
        {
            var valueExists = tokenDictionary.TryGetValue(caseKey.token, out object token);

            if (valueExists)
            {
                switch (token)
                {
                    case int intValue when token is int && intValue == 1:
                    case int lessThanValue when token is int && caseKey.operation == Constants.LessThan && lessThanValue < int.Parse(caseKey.value.ToString()):
                    case int moreThanValue when token is int && caseKey.operation == Constants.MoreThan && moreThanValue > int.Parse(caseKey.value.ToString()):
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