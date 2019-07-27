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
            return type
                .GetCustomAttributes<TokenExpressionAttribute>(false)
                .First()
                .Pattern;
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
            foreach (var (tokenExpressionType, pattern) in TokenExpressionTypePatterns)
            {
                if (Regex.IsMatch(tokenExpression, pattern))
                {
                    var expressionObject = FormatterServices.GetUninitializedObject(tokenExpressionType) as ITokenExpression;

                    return expressionObject.Resolve(tokenExpression, tokenDictionary);
                }
            }

            return tokenExpression;
        }

        private static string AggregateStrings(string left, string right)
        {
            return $"{left}{right}";
        }
    }
}