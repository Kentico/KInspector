using KInspector.Core.Tokens;

namespace KInspector.Core.Models
{
    /// <summary>
    /// A string containing tokens which can be replaced with provided values.
    /// </summary>
    public class Term
    {
        private string Markdown { get; set; }

        private object? TokenValues { get; set; }

        private Term(string? value)
        {
            Markdown = value ?? string.Empty;
        }

        public static implicit operator Term(string? value)
        {
            return new Term(value);
        }

        public static implicit operator string(Term? term)
        {
            return term?.ToString() ?? string.Empty;
        }

        public override string ToString()
        {
            if (TokenValues is not null)
            {
                return TokenExpressionResolver.ResolveTokenExpressions(Markdown, TokenValues);
            }

            return Markdown;
        }

        /// <summary>
        /// Prepares for token replacement based on the <paramref name="tokenValues"/> object.
        /// </summary>
        /// <param name="tokenValues">Object with property names that map to token names and property values that map to token values.</param>
        /// <returns>Phrase with string.</returns>
        public Term With(object tokenValues)
        {
            TokenValues = tokenValues;

            return this;
        }
    }
}