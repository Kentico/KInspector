using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Tokens
{
    public class TokenExpressionResolver
    {
        private const char space = ' ';

        private static IEnumerable<(Type tokenExpressionType, string pattern)> TokenExpressionTypePatterns { get; set; }

        private static string AllTokenExpressionPatterns => TokenExpressionTypePatterns
                                .Select(tokenExpressionTypePattern => tokenExpressionTypePattern.pattern)
                                .Where(ValueIsNotEmpty)
                                .Aggregate(AsRegexOr);

        public static void RegisterTokenExpressions(Assembly assemblies)
        {
            TokenExpressionTypePatterns = assemblies.GetTypes()
                                .Where(TypeIsMarkedWithTokenExpressionAttribute)
                                .Select(type => (type, GetTokenPatternFromAttribute(type)));
        }

        private static bool TypeIsMarkedWithTokenExpressionAttribute(Type type)
        {
            return type.IsDefined(typeof(TokenExpressionAttribute), false);
        }

        private static string GetTokenPatternFromAttribute(Type type)
        {
            var pattern = type
                .GetCustomAttributes<TokenExpressionAttribute>(false)
                .First()
                .Pattern;

            return $"( {pattern} )|({pattern} )|( {pattern}$)|(^{pattern}$)";
        }

        internal static string ResolveTokenExpressions(Term term, object tokenValues)
        {
            var tokenDictionary = GetValuesDictionary(tokenValues);

            return Regex.Split(term, AllTokenExpressionPatterns)
                        .Select(tokenExpression => ResolveTokenExpression(tokenExpression, tokenDictionary))
                        .Aggregate(AggregateStrings);
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

        private static bool ValueIsNotEmpty(string pattern)
        {
            return !string.IsNullOrEmpty(pattern);
        }

        private static string AsRegexOr(string left, string right)
        {
            return $"{left}|{right}";
        }

        private static string ResolveTokenExpression(string tokenExpression, IDictionary<string, object> tokenDictionary)
        {
            var (leadingSpace, innerTokenExpression, trailingSpace) = GetSplitExpression(tokenExpression);

            string resolvedExpression = null;

            foreach (var (tokenExpressionType, pattern) in TokenExpressionTypePatterns)
            {
                if (Regex.IsMatch(innerTokenExpression, pattern))
                {
                    var expressionObject = FormatterServices.GetUninitializedObject(tokenExpressionType) as ITokenExpression;

                    resolvedExpression = expressionObject.Resolve(innerTokenExpression, tokenDictionary);
                }
            }

            resolvedExpression = EnsureEmptyResolvedExpressionContainsOnlyOneSpace(ref leadingSpace, resolvedExpression, ref trailingSpace, innerTokenExpression);

            return $"{leadingSpace}{resolvedExpression}{trailingSpace}";
        }

        private static string EnsureEmptyResolvedExpressionContainsOnlyOneSpace(ref char? leadingSpace, string resolvedExpression, ref char? trailingSpace, string innerTokenExpression)
        {
            if (string.IsNullOrEmpty(resolvedExpression) && leadingSpace != null && trailingSpace != null)
            {
                leadingSpace = null;
                trailingSpace = null;

                return space.ToString();
            }
            else
            {
                return resolvedExpression ?? innerTokenExpression;
            }
        }

        private static (char? leadingSpace, string innerTokenExpression, char? trailingSpace) GetSplitExpression(string tokenExpression)
        {
            char? leadingSpace = null;
            char? trailingSpace = null;

            if (tokenExpression.Any())
            {
                if (tokenExpression.First() == space) leadingSpace = space;
                if (tokenExpression.Last() == space) trailingSpace = space;
            }

            return (leadingSpace, tokenExpression.Trim(), trailingSpace);
        }

        private static string AggregateStrings(string left, string right)
        {
            return $"{left}{right}";
        }
    }
}