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

            if (tokenDictionary.TryGetValue(expression.token, out object token))
            {
                foreach (var (value, operation, result) in expression.cases)
                {
                    var resolved = TryResolveToken(token, value, operation, result, out string resolvedToken);

                    if (resolved) return resolvedToken;
                }
            }

            if (token == null)
            {
                return string.Empty;
            }

            if (expression.token == expression.defaultValue)
            {
                return token.ToString();
            }

            return expression.defaultValue ?? string.Empty;
        }

        private (string token, IEnumerable<(string value, char operation, string result)> cases, string defaultValue) GetExpression(string tokenExpression)
        {
            if (string.IsNullOrEmpty(tokenExpression)) throw new ArgumentException($"'{tokenExpression}' looks like a simple token expression but does not contain a token.");

            var segments = tokenExpression.Split(Constants.Pipe);

            if (segments[0].Contains(Constants.Colon)) throw new FormatException($"Simple token expression token '{segments[0]}' must not contain a {Constants.Colon}.");

            var cases = new List<(string, char, string)>();

            string defaultValue = null;

            switch (segments.Length)
            {
                case 1:
                    defaultValue = segments[0];

                    break;

                default:
                    if (!segments[segments.Length - 1].Contains(Constants.Colon))
                    {
                        defaultValue = segments[segments.Length - 1];
                    }

                    foreach (var segment in segments.Skip(1).Take(segments.Length - 1))
                    {
                        cases.Add(GetCase(segment));
                    }

                    break;
            }

            return (segments[0], cases, defaultValue);
        }

        private static (string, char, string) GetCase(string expressionCase)
        {
            var operation = Constants.Equals;

            var pair = expressionCase.SplitAtFirst(Constants.Colon);

            if (!expressionCase.Contains(Constants.Colon))
            {
                return (null, operation, pair.first);
            }

            var firstChar = pair.first[0];

            var operationChars = new[] { Constants.MoreThan, Constants.LessThan };

            if (!operationChars.Contains(firstChar))
            {
                return (pair.first, operation, pair.second);
            }

            foreach (var item in operationChars)
            {
                if (firstChar == item) operation = item;
            }

            return (pair.first.Substring(1), operation, pair.second);
        }

        public bool TryResolveToken(object token, string expressionCaseValue, char operation, string expressionCaseResult, out string resolvedValue)
        {
            var resolved = false;
            resolvedValue = null;

            if (token == null)
            {
                return resolved;
            }

            switch (token)
            {
                case int intToken:
                    resolved = TryResolveIntToken(intToken, expressionCaseValue, operation);
                    break;

                case double doubleToken:
                    resolved = TryResolveDoubleToken(doubleToken, expressionCaseValue, operation);
                    break;

                case bool boolToken:
                    resolved = TryResolveBoolToken(boolToken, expressionCaseValue);
                    break;

                default:
                    resolved = TryResolveStringToken(token.ToString(), expressionCaseValue);
                    break;
            }

            if (resolved)
            {
                resolvedValue = expressionCaseResult;
            }

            return resolved;
        }

        private bool TryResolveIntToken(int token, string expressionCaseValue, char operation)
        {
            return TryResolveDoubleToken(token, expressionCaseValue, operation);
        }

        private static bool TryResolveDoubleToken(double token, string expressionCaseValue, char operation)
        {
            var expressionCaseValueIsDouble = double.TryParse(expressionCaseValue, out double doubleExpressionCaseValue);

            if (expressionCaseValueIsDouble)
            {
                if (operation == Constants.Equals && token == doubleExpressionCaseValue
                    || operation == Constants.LessThan && token < doubleExpressionCaseValue
                    || operation == Constants.MoreThan && token > doubleExpressionCaseValue)
                {
                    return true;
                }
            }
            else if (string.IsNullOrEmpty(expressionCaseValue) && token == 1)
            {
                return true;
            }

            return false;
        }

        private static bool TryResolveBoolToken(bool token, string expressionCaseValue)
        {
            var expressionCaseValueIsBool = bool.TryParse(expressionCaseValue, out bool boolExpressionCaseValue);

            if (expressionCaseValueIsBool)
            {
                if (token == boolExpressionCaseValue)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryResolveStringToken(string token, string expressionCaseValue)
        {
            if (token == expressionCaseValue)
            {
                return true;
            }

            return false;
        }
    }
}