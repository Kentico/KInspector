using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Core.Tokens
{
    /// <summary>
    /// Represents tokens using a single token and having N cases and an optional default.
    /// </summary>
    [TokenExpression("<[^?].*?>")]
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

            var token = tokenExpression;

            var indexOfFirstPipe = tokenExpression.IndexOf(Constants.Pipe);

            if (indexOfFirstPipe > -1)
            {
                token = tokenExpression.Substring(0, indexOfFirstPipe);
            }

            string defaultValue = null;

            var cases = tokenExpression
                .Split(new[] { Constants.Pipe }, StringSplitOptions.RemoveEmptyEntries)
                .AsEnumerable();

            if (cases.Count() > 1) cases = cases.Skip(1);

            var casePairs = cases
                .Select(casePair => GetCasePair(casePair, ref defaultValue))
                .ToList();

            return (token, casePairs, defaultValue);
        }

        private static (string, char, string) GetCasePair(string casePair, ref string defaultValue)
        {
            var pair = casePair.Split(new[] { Constants.Colon }, StringSplitOptions.RemoveEmptyEntries);

            switch (pair.Length)
            {
                case 1:
                    defaultValue = pair[0];

                    return (null, Constants.Equals, pair[0]);

                case 2:
                    var (key, operation) = GetCaseKey(pair[0]);

                    return (key, operation, pair[1]);

                default:
                    if (pair.Skip(2).Any(segment => segment[0] != Constants.Space))
                    {
                        break;
                    }

                    var (keyD, operationD) = GetCaseKey(pair[0]);

                    return (keyD, operationD, pair.Skip(1).Aggregate((l, r) => $"{l}{Constants.Colon}{r}"));
            }

            throw new ArgumentException($"Case pair '{casePair}' looks like a simple case pair but does not contain zero or one {Constants.Colon}.");
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