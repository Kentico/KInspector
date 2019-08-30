﻿using KenticoInspector.Core.Tokens;

namespace KenticoInspector.Core.Models
{
    public class Term
    {
        private string Markdown { get; set; }

        private object TokenValues { get; set; }

        private Term(string value)
        {
            Markdown = value;
        }

        public static implicit operator Term(string value)
        {
            return new Term(value);
        }

        public static implicit operator string(Term term)
        {
            return term.ToString();
        }

        public override string ToString()
        {
            if (TokenValues != null)
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