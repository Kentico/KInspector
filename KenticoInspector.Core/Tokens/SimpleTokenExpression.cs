using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Extensions;

namespace KenticoInspector.Core.Tokens
{
    /// <summary>
    /// Represents tokens using a single token and having N cases and an optional default.
    /// </summary>
    [TokenExpression("<(?!\\?).*?>")]
    internal class SimpleTokenExpression : ITokenExpression
    {
        private static readonly char[] expressionBoundary = new[] { '<', '>' };

        public string Resolve(string tokenExpression, IDictionary<string, object> tokenDictionary)
        {
            var trimmedTokenExpression = tokenExpression.Trim(expressionBoundary);

            var expression = GetExpression(trimmedTokenExpression);

            var resolved = false;

            var resolvedValue = string.Empty;

            if (tokenDictionary.TryGetValue(expression.token, out object token))
            {
                foreach (var (value, operation, caseValue) in expression.expressionCases)
                {
                    resolved = TryResolveToken(token, value, operation, caseValue, out resolvedValue);

                    if (resolved) break;
                }

                if (resolved && expression.token != resolvedValue)
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

        private (string token, IEnumerable<(string value, char operation, string caseValue)> expressionCases, string defaultValue) GetExpression(string tokenExpression)
        {
            if (string.IsNullOrEmpty(tokenExpression))
            {
                throw new ArgumentException($"'{tokenExpression}' looks like a simple token expression but does not contain a key.");
            }

            var (token, casesAndDefault) = tokenExpression.SplitAtFirst(Constants.Pipe);

            if (string.IsNullOrEmpty(casesAndDefault))
            {
                return (token, Enumerable.Empty<(string, char, string)>(), token);
            }

            var (cases, defaultValue) = casesAndDefault.SplitAtLast(Constants.Pipe);

            if (defaultValue.IndexOf(Constants.Colon) > -1)
            {
                cases = defaultValue;
                defaultValue = null;
            }

            var casePairs = cases
                .Split(new[] { Constants.Pipe })
                .Select(casePair => GetCasePair(casePair))
                .ToList();

            return (token, casePairs, defaultValue);
        }

        private static (string, char, string) GetCasePair(string casePair)
        {
            var pair = casePair.SplitAtFirst(Constants.Colon);

            if (string.IsNullOrEmpty(pair.second))
            {
                return (null, Constants.Equals, pair.first);
            }

            var (key, operation) = GetCaseKey(pair.first);

            return (key, operation, pair.second);
        }

        private static (string, char) GetCaseKey(string caseKey)
        {
            char operation = Constants.Equals;

            var operationChars = new[] { Constants.MoreThan, Constants.LessThan };

            if (caseKey.Any())
            {
                var firstChar = caseKey.First();

                foreach (var item in operationChars)
                {
                    if (firstChar == item) operation = item;
                }
            }

            return (caseKey.TrimStart(operationChars), operation);
        }

        private bool TryResolveToken(object token, string value, char operation, string caseValue, out string resolvedValue)
        {
            switch (token)
            {
                case int intValue when token is int && intValue == 1:
                case int lessThanValue when token is int && operation == Constants.LessThan && lessThanValue < int.Parse(value.ToString()):
                case int moreThanValue when token is int && operation == Constants.MoreThan && moreThanValue > int.Parse(value.ToString()):
                case var _ when token?.ToString() == value:
                    resolvedValue = caseValue;

                    return true;
            }

            resolvedValue = null;

            return false;
        }
    }
}