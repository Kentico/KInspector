using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Core.Models
{
    /// <summary>
    /// Represents tokens using a single key and having N cases and an optional default.
    /// </summary>
    internal class SimpleToken : Token
    {
        private static readonly char SimpleDelimiter = '|';
        private static readonly char CaseSeparator = ':';

        private string Key { get; set; }

        internal SimpleToken(string tokenExpression) : base(tokenExpression)
        {
            Key = ExtractKey(tokenExpression);
        }

        private string ExtractKey(string tokenExpression)
        {
            if (string.IsNullOrEmpty(tokenExpression))
            {
                throw new ArgumentException($"'{tokenExpression}' looks like a simple token but does not contain a key.");
            }

            var indexOfFirstPipe = tokenExpression.IndexOf(SimpleDelimiter);

            if (indexOfFirstPipe > -1)
            {
                return tokenExpression.Substring(0, indexOfFirstPipe);
            }

            return tokenExpression;
        }

        internal override string FillFrom(IDictionary<string, object> valuesDictionary)
        {
            if (string.IsNullOrEmpty(Key))
            {
                return TokenExpression;
            }

            if (valuesDictionary.TryGetValue(Key, out object value))
            {
                return ResolveToken(value);
            }

            return TokenExpression;
        }

        private string ResolveToken(object value)
        {
            var splitExpression = TokenExpression.Split(new[] { SimpleDelimiter }, StringSplitOptions.RemoveEmptyEntries);

            if (splitExpression.Length == 1)
            {
                return value.ToString();
            }

            var expressionCases = splitExpression.Skip(1).ToArray();

            var totalCases = expressionCases.Length;

            for (int i = 0; i < totalCases; i++)
            {
                var expressionCase = expressionCases[i];

                var isDefault = i + 1 == totalCases;

                (string caseKey, string caseValue) = GetCaseKeyAndValue(expressionCase);

                if (string.IsNullOrEmpty(caseKey))
                {
                    switch (value)
                    {
                        case int intValue when value is int && intValue == 1:
                            return caseValue;

                        case var _ when value is int && !isDefault:
                            continue;
                    }

                    if (isDefault)
                    {
                        return caseValue;
                    }

                    throw new FormatException($"'{expressionCase}' inside '{TokenExpression}' looks like a default but does not come last.");
                }

                if (value.ToString().Equals(caseKey, StringComparison.InvariantCultureIgnoreCase))
                {
                    return caseValue;
                }

                continue;
            }

            return string.Empty;
        }

        private (string key, string value) GetCaseKeyAndValue(string expressionCase)
        {
            var splitExpressionCase = expressionCase.Split(new[] { CaseSeparator }, StringSplitOptions.RemoveEmptyEntries);

            switch (splitExpressionCase.Length)
            {
                case 2:
                    return (splitExpressionCase[0], splitExpressionCase[1]);

                case 1:
                    return (null, splitExpressionCase[0]);

                default:
                    throw new FormatException($"'{expressionCase}' inside '{TokenExpression}' looks like a case but contains more than one {CaseSeparator}.");
            }
        }
    }
}