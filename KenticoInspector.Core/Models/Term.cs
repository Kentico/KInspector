using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace KenticoInspector.Core.Models
{
    public class Term
    {
        private Term(string value)
        {
            Markdown = value;
        }

        public string Markdown { get; set; }

        public static implicit operator Term(string value)
        {
            return new Term(value);
        }

        public static implicit operator string(Term term)
        {
            return term.Markdown;
        }

        public override string ToString()
        {
            return Markdown;
        }

        /// <summary>
        /// Replaces tokens in strings based on the <paramref name="tokenValues"/> object.
        /// </summary>
        /// <param name="tokenValues">Object with property names that map to token names and property values that map to the token value.</param>
        /// <returns>Phrase with  string.</returns>
        /// <remarks>
        /// Replacement is done using simple <see cref="string.Replace"/> and <see cref="object.ToString()"/>.
        /// </remarks>
        public Term With(object tokenValues)
        {
            var term = this;

            if (tokenValues == null)
            {
                return term;
            }

            var properties = tokenValues
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(PropertyIsNotIndexableAndHasGetter);

            foreach (var property in properties)
            {
                var name = property.Name;
                var value = property.GetValue(tokenValues);

                term = ResolvePluralizationExpressions(term.Markdown, name, value);
            }

            return term;
        }

        private static bool PropertyIsNotIndexableAndHasGetter(PropertyInfo prop)
        {
            return prop.GetIndexParameters().Length == 0
                    && prop.GetMethod != null;
        }

        private string ResolvePluralizationExpressions(string markdown, string tokenName, object tokenValue)
        {
            var pluralizationRegex = new Regex($"<({tokenName}):?([^\\|]*?)?\\|?([^\\|]*?)?>");
            var resolvedMarkdown = pluralizationRegex.Replace(markdown, match => ResolvePluralizationMatches(match, tokenValue));

            return resolvedMarkdown;
        }

        private string ResolvePluralizationMatches(Match match, object tokenValue)
        {
            var singular = match.Groups[2].Value;
            var plural = match.Groups[3].Value;

            if (string.IsNullOrEmpty(singular))
            {
                return tokenValue.ToString();
            }

            if (tokenValue is int intValue)
            {
                if (intValue == 1)
                {
                    return singular;
                }

                return plural;
            }

            return match.Value;
        }
    }
}