using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Tokens
{
    public class TokenProcessor
    {
        private static IDictionary<Type, string> TokenTypePatterns { get; set; }

        public static void RegisterTokens(Assembly assemblies)
        {
            TokenTypePatterns = assemblies.GetTypes()
                                .Where(TypeIsMarkedToken)
                                .ToDictionary(Type, TokenPattern);
        }

        private static bool TypeIsMarkedToken(Type type)
        {
            return type.IsDefined(typeof(TokenAttribute), false);
        }

        private static Type Type(Type type)
        {
            return type;
        }

        private static string TokenPattern(Type type)
        {
            return type
                .GetCustomAttributes<TokenAttribute>(false)
                .First()
                .Pattern;
        }

        internal static string ParseTokens(Term term, object tokenValues)
        {
            var valuesDictionary = GetValuesDictionary(tokenValues);

            var mergedPattern = TokenTypePatterns
                                .Values
                                .Where(NotEmpty)
                                .Aggregate(AsRegexOr);

            return Regex.Split(term, mergedPattern)
                        .Select(token => AsFilledToken(token, valuesDictionary))
                        .Aggregate(MergeStrings);
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
                .ToDictionary(PropertyName, p => PropertyValue(p, tokenValues));
        }


        private static bool PropertyIsNotIndexableAndHasGetter(PropertyInfo prop)
        {
            return prop.GetIndexParameters().Length == 0
                    && prop.GetMethod != null;
        }

        private static string PropertyName(PropertyInfo property)
        {
            return property.Name;
        }

        private static object PropertyValue(PropertyInfo property, object tokenValues)
        {
            return property.GetValue(tokenValues);
        }

        private static bool NotEmpty(string pattern)
        {
            return !string.IsNullOrEmpty(pattern);
        }

        private static string AsRegexOr(string left, string right)
        {
            return $"{left}|{right}";
        }

        private static string AsFilledToken(string rawToken, IDictionary<string, object> valuesDictionary)
        {
            foreach (var tokenType in TokenTypePatterns)
            {
                if (Regex.IsMatch(rawToken, tokenType.Value))
                {
                    var token = FormatterServices.GetUninitializedObject(tokenType.Key) as IToken;

                    return token.FillFrom(rawToken, valuesDictionary);
                }
            }

            return rawToken;
        }

        private static string MergeStrings(string left, string right)
        {
            return $"{left}{right}";
        }
    }
}