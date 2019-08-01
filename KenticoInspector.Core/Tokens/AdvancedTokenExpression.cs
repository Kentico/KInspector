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

            foreach (var (caseValue, caseResult) in expression.expressionCases)
            {
                var resolved = TryResolveToken(tokenDictionary, caseValue, caseResult, out string resolvedToken);

                if (resolved) return resolvedToken;
            }

            if (!string.IsNullOrEmpty(expression.defaultValue) && tokenDictionary.TryGetValue(expression.defaultValue, out object token))
            {
                return token?.ToString();
            }

            return expression.defaultValue ?? string.Empty;
        }

        private (IEnumerable<((string token, char operation, string value) caseKey, string caseValue)> expressionCases, string defaultValue) GetExpression(string tokenExpression)
        {
            if (string.IsNullOrEmpty(tokenExpression)) throw new ArgumentException($"'{tokenExpression}' looks like an advanced token expression but does not contain a token or case.");

            var segments = tokenExpression.Split(Constants.Pipe);

            var cases = new List<((string, char, string), string)>();

            string defaultValue = null;

            switch (segments.Length)
            {
                case 1:
                    if (segments[0].Contains(Constants.Colon) && segments[0].Contains(Constants.Equals))
                    {
                        cases.Add(GetCase(segments[0]));
                    }
                    else
                    {
                        defaultValue = segments[0];
                    }

                    break;

                default:
                    if (segments[segments.Length - 1].Contains(Constants.Colon))
                    {
                        cases.Add(GetCase(segments[segments.Length - 1]));
                    }
                    else
                    {
                        defaultValue = segments[segments.Length - 1];
                    }

                    foreach (var segment in segments.Take(segments.Length - 1))
                    {
                        cases.Add(GetCase(segment));
                    }

                    break;
            }

            return (cases, defaultValue);
        }

        private static ((string, char, string), string caseValue) GetCase(string casePair)
        {
            var pair = casePair.SplitAtFirst(Constants.Colon);

            if (string.IsNullOrEmpty(pair.second))
            {
                return ((null, char.MinValue, null), null);
            }

            return (GetCaseValue(pair.first), pair.second);
        }

        private static (string, char, string) GetCaseValue(string caseValue)
        {
            char operation = Constants.Equals;

            char[] operationChars = new[] { Constants.Equals, Constants.LessThan, Constants.MoreThan };

            var key = caseValue.Split(operationChars);

            switch (key.Length)
            {
                case 1:
                    return (key[0], operation, null);

                case 2:
                    if (caseValue.Contains(Constants.MoreThan)) operation = Constants.MoreThan;
                    if (caseValue.Contains(Constants.LessThan)) operation = Constants.LessThan;

                    return (key[0], operation, key[1]);
            }

            throw new ArgumentException($"Case key '{caseValue}' looks like an advanced case key but does not contain zero or one {string.Join(',', operationChars)}.");
        }

        private bool TryResolveToken(IDictionary<string, object> tokenDictionary, (string token, char operation, string value) caseValue, string result, out string resolvedValue)
        {
            var valueExists = tokenDictionary.TryGetValue(caseValue.token, out object token);

            if (valueExists)
            {
                switch (token)
                {
                    case int intValue when token is int && intValue == 1:
                    case int lessThanValue when token is int && caseValue.operation == Constants.LessThan && lessThanValue < int.Parse(caseValue.value.ToString()):
                    case int moreThanValue when token is int && caseValue.operation == Constants.MoreThan && moreThanValue > int.Parse(caseValue.value.ToString()):
                    case var _ when token?.ToString() == caseValue.value:
                        resolvedValue = result;

                        return true;
                }
            }

            resolvedValue = null;

            return false;
        }
    }
}