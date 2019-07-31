using System;
using System.Collections.Generic;
using System.Linq;
using KenticoInspector.Core.Extensions;

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

        private (IEnumerable<((string token, char operation, string value) caseKey, string caseValue)> expressionCases, string defaultValue) GetExpression(string tokenExpression)
        {
            if (string.IsNullOrEmpty(tokenExpression))
            {
                throw new ArgumentException($"'{tokenExpression}' looks like an advanced token expression but does not contain a key.");
            }

            var (cases, defaultValue) = tokenExpression.SplitAtLast(Constants.Pipe);

            if (defaultValue.IndexOf(Constants.Colon) > -1)
            {
                cases += $"|{defaultValue}";
                defaultValue = null;
            }

            if (string.IsNullOrEmpty(cases))
            {
                return (Enumerable.Empty<((string, char, string), string)>(), defaultValue);
            }

            var casePairs = cases
                .Split(new[] { Constants.Pipe })
                .Select(casePair => GetCasePair(casePair))
                .Where(casePair => casePair.caseValue != null)
                .ToList();

            return (casePairs, defaultValue);
        }

        private static ((string, char, string), string caseValue) GetCasePair(string casePair)
        {
            var pair = casePair.SplitAtFirst(Constants.Colon);

            if (string.IsNullOrEmpty(pair.second))
            {
                return ((null, char.MinValue, null), null);
            }

            return (GetCaseKey(pair.first), pair.second);
        }

        private static (string, char, string) GetCaseKey(string caseKey)
        {
            char operation = Constants.Equals;

            char[] operationChars = new[] { Constants.Equals, Constants.LessThan, Constants.MoreThan };

            var key = caseKey.Split(operationChars);

            switch (key.Length)
            {
                case 1:
                    return (key[0], operation, null);

                case 2:
                    if (caseKey.Contains(Constants.MoreThan)) operation = Constants.MoreThan;
                    if (caseKey.Contains(Constants.LessThan)) operation = Constants.LessThan;

                    return (key[0], operation, key[1]);
            }

            throw new ArgumentException($"Case key '{caseKey}' looks like an advanced case key but does not contain zero or one {string.Join(',', operationChars)}.");
        }

        private bool TryResolveToken(IDictionary<string, object> tokenDictionary, (string token, char operation, string value) caseKey, string caseValue, out string resolvedValue)
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