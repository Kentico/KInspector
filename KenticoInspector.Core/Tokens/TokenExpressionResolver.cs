using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace KenticoInspector.Core.Tokens
{
    public class TokenExpressionResolver
    {
        private static IEnumerable<(Type tokenExpressionType, string pattern)> TokenExpressionTypePatterns { get; set; }

        public static void RegisterTokenExpressions(Assembly assembly)
        {
            TokenExpressionTypePatterns = assembly
                .GetTypes()
                .Where(TypeIsMarkedWithTokenExpressionAttribute)
                .Select(AsTokenExpressionTypePattern);

            bool TypeIsMarkedWithTokenExpressionAttribute(Type type)
            {
                return type.IsDefined(typeof(TokenExpressionAttribute), false);
            }

            (Type type, string) AsTokenExpressionTypePattern(Type type)
            {
                var pattern = type
                    .GetCustomAttributes<TokenExpressionAttribute>(false)
                    .First()
                    .Pattern;

                var patternVariants = new[]
                {
                    $"([{Constants.Space}]{pattern}[{Constants.Space}{Constants.Period}{Constants.Colon}])",
                    $"({pattern}[{Constants.Space}{Constants.Period}{Constants.Colon}])",
                    $"([{Constants.Space}]{pattern})",
                    $"(^{pattern}$)"
                };

                var joinedPattern = string.Join(Constants.Pipe, patternVariants);

                return (type, joinedPattern);
            }
        }

        internal static string ResolveTokenExpressions(string term, object tokenValues)
        {
            var allTokenExpressionPatterns = TokenExpressionTypePatterns
                .Select(tokenExpressionTypePattern => tokenExpressionTypePattern.pattern)
                .Where(pattern => !string.IsNullOrEmpty(pattern));

            var joinedTokenExpressionPatterns = string.Join(Constants.Pipe, allTokenExpressionPatterns);

            var tokenDictionary = GetValuesDictionary(tokenValues);

            var resolvedExpressions = Regex.Split(term, joinedTokenExpressionPatterns)
                .Select(tokenExpression => ResolveTokenExpression(tokenExpression, tokenDictionary));

            return string.Join(string.Empty, resolvedExpressions);
        }

        private static IDictionary<string, object> GetValuesDictionary(object tokenValues)
        {
            if (tokenValues is IDictionary<string, object> dictionary)
            {
                return dictionary;
            }

            return tokenValues
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(PropertyIsNotIndexableAndHasGetter)
                .ToDictionary(property => property.Name, property => property.GetValue(tokenValues));
        }

        private static bool PropertyIsNotIndexableAndHasGetter(PropertyInfo prop)
        {
            return prop.GetIndexParameters().Length == 0
                && prop.GetMethod != null;
        }

        private static string ResolveTokenExpression(string tokenExpression, IDictionary<string, object> tokenDictionary)
        {
            var (leadingChar, innerTokenExpression, trailingChar) = GetSplitExpression(tokenExpression);

            string resolvedExpression = null;

            foreach (var (tokenExpressionType, pattern) in TokenExpressionTypePatterns)
            {
                if (Regex.IsMatch(innerTokenExpression, pattern))
                {
                    var expressionObject = FormatterServices.GetUninitializedObject(tokenExpressionType) as ITokenExpression;

                    resolvedExpression = expressionObject.Resolve(innerTokenExpression, tokenDictionary);

                    break;
                }
            }

            if (string.IsNullOrEmpty(resolvedExpression) && leadingChar != null && trailingChar != null)
            {
                return Constants.Space.ToString();
            }
            else
            {
                return $"{leadingChar}{resolvedExpression ?? innerTokenExpression}{trailingChar}";
            }
        }

        private static (char?, string, char?) GetSplitExpression(string tokenExpression)
        {
            char? leadingChar = null;
            char? trailingChar = null;

            var leadingChars = new[] { Constants.Space };
            var trailingChars = new[] { Constants.Space, Constants.Period, Constants.Colon };

            if (tokenExpression.Any())
            {
                var firstChar = tokenExpression.First();
                var lastChar = tokenExpression.Last();

                foreach (var item in leadingChars)
                {
                    if (firstChar == item) leadingChar = item;
                }

                foreach (var item in trailingChars)
                {
                    if (lastChar == item) trailingChar = item;
                }
            }

            return (leadingChar, tokenExpression.TrimStart(leadingChars).TrimEnd(trailingChars), trailingChar);
        }
    }
}