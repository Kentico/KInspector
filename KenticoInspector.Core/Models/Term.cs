using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KenticoInspector.Core.Models
{
    public class Term
    {
        private string RawMarkdown { get; set; }

        private IEnumerable<Token> Tokens
        {
            get
            {
                return Token.ToTokens(RawMarkdown);
            }
        }

        private Term(string value)
        {
            RawMarkdown = value;
        }

        public static implicit operator Term(string value)
        {
            return new Term(value);
        }

        public static implicit operator string(Term term)
        {
            return term.RawMarkdown;
        }

        public override string ToString()
        {
            return RawMarkdown;
        }

        /// <summary>
        /// Replaces tokens in strings based on the <paramref name="tokenValues"/> object.
        /// </summary>
        /// <param name="tokenValues">Object with property names that map to token names and property values that map to the token value.</param>
        /// <returns>Phrase with string.</returns>
        public string With(object tokenValues)
        {
            var term = this;

            if (tokenValues == null || !Tokens.Any())
            {
                return term;
            }

            var valuesDictionary = GetValuesDictionary(tokenValues);

            var filledTerm = Tokens
                .Select(token => token.FillFrom(valuesDictionary))
                .Aggregate(MergeStrings);

            return filledTerm;
        }

        private static IDictionary<string, object> GetValuesDictionary(object tokenValues)
        {
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

        private static string MergeStrings(string left, string right)
        {
            return $"{left}{right}";
        }
    }
}