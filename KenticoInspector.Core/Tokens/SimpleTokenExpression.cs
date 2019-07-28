using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Core.Tokens
{
    /// <summary>
    /// Represents tokens using a single token and having N cases and an optional default.
    /// </summary>
    [TokenExpression("<.*?>")]
    internal class SimpleTokenExpression : ITokenExpression
    {
        private static readonly char[] expressionBoundary = new[] { '<', '>' };
        private const char simpleDelimiter = '|';
        private const char caseDelimiter = ':';
        private const char equals = '=';
        private const char lessThan = '<';
        private const char moreThan = '>';

        public string Resolve(string tokenExpression, IDictionary<string, object> tokenDictionary)
        {
            var expression = GetExpression(tokenExpression);

            var resolved = false;

            var resolvedValue = string.Empty;

            if (tokenDictionary.TryGetValue(expression.token, out object token))
            {
                foreach (var (value, operation, caseValue) in expression.expressionCases)
                {
                    resolved = TryResolveToken(token, value, operation, caseValue, out resolvedValue);

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

        private (string token, IEnumerable<(string value, char operation, string caseValue)> expressionCases, string defaultValue) GetExpression(string tokenExpression)
        {
            var trimmedTokenExpression = tokenExpression.Trim(expressionBoundary);

            if (string.IsNullOrEmpty(trimmedTokenExpression))
            {
                throw new ArgumentException($"'{tokenExpression}' looks like a simple token expression but does not contain a key.");
            }

            var token = trimmedTokenExpression;

            var indexOfFirstPipe = trimmedTokenExpression.IndexOf(simpleDelimiter);

            if (indexOfFirstPipe > -1)
            {
                token = trimmedTokenExpression.Substring(0, indexOfFirstPipe);
            }

            string defaultValue = null;

            var cases = trimmedTokenExpression
                .Split(new[] { simpleDelimiter }, StringSplitOptions.RemoveEmptyEntries)
                .AsEnumerable();

            if (cases.Count() > 1) cases = cases.Skip(1);

            var casePairs = cases
                .Select(casePair => GetCasePair(casePair, ref defaultValue))
                .ToList();

            return (token, casePairs, defaultValue);
        }

        private static (string, char, string) GetCasePair(string casePair, ref string defaultValue)
        {
            var pair = casePair.Split(new[] { caseDelimiter }, StringSplitOptions.RemoveEmptyEntries);
            char operation = equals;

            switch (pair.Length)
            {
                case 1:
                    defaultValue = pair[0];

                    return (null, operation, pair[0]);

                case 2:
                    if (pair[0].Contains(moreThan)) operation = moreThan;
                    if (pair[0].Contains(lessThan)) operation = lessThan;

                    return (pair[0].Trim(new[] { lessThan, moreThan }), operation, pair[1]);
            }

            throw new ArgumentException($"Case pair '{casePair}' looks like a simple case key but does not contain zero or one {caseDelimiter}.");
        }

        private bool TryResolveToken(object token, string value, char operation, string caseValue, out string resolvedValue)
        {
            switch (token)
            {
                case int intValue when token is int && intValue == 1:
                case int lessThanValue when token is int && operation == lessThan && lessThanValue < int.Parse(value.ToString()):
                case int moreThanValue when token is int && operation == moreThan && moreThanValue > int.Parse(value.ToString()):
                case var _ when token?.ToString() == value:
                    resolvedValue = caseValue;

                    return true;
            }

            resolvedValue = null;

            return false;
        }
    }
}