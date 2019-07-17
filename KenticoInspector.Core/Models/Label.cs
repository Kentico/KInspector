using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace KenticoInspector.Core.Models
{
    public class Label
    {
        /// <summary>
        /// Contains Markdown string.
        /// </summary>
        private readonly string text;

        private Label(string value)
        {
            text = value;
        }

        public static implicit operator string(Label comment)
        {
            return comment.text;
        }

        public static implicit operator Label(string value)
        {
            return new Label(value);
        }

        public override string ToString()
        {
            return text;
        }

        public Label Replace(string macroName, object newValue)
        {
            var macroRegex = new Regex($"<({macroName}):?(\\w*)\\|?(\\w*)>");

            return macroRegex.Replace(text, match => ReplaceMacro(match, newValue));
        }

        private string ReplaceMacro(Match match, object newValue)
        {
            var singular = match.Groups[2].Value;
            var plural = match.Groups[3].Value;

            if (string.IsNullOrEmpty(singular))
            {
                return newValue.ToString();
            }

            if (newValue is int intValue)
            {
                if (intValue == 1)
                {
                    return singular;
                }

                return plural;
            }

            return match.Value;
        }

        /// <summary>
        /// Replaces macro strings based on the <paramref name="replacements"/> object.
        /// </summary>
        /// <param name="commentName">Case-sensitive comment name.</param>
        /// <param name="replacements">Object containing replacements.</param>
        /// <returns>Markdown string.</returns>
        /// <remarks>
        /// Replacement is done using simple <see cref="string.Replace"/> and <see cref="object.ToString()"/>.
        /// </remarks>
        public Label With(object replacements)
        {
            var rawLabel = this;

            if (replacements == null)
            {
                return rawLabel;
            }

            var properties = replacements.GetType()
                                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                        .Where(PropertyIsNotIndexableAndHasGetter);

            foreach (var property in properties)
            {
                var value = property.GetValue(replacements);

                rawLabel = rawLabel.Replace(property.Name, value);
            }

            return rawLabel;
        }

        private static bool PropertyIsNotIndexableAndHasGetter(PropertyInfo prop)
        {
            return prop.GetIndexParameters().Length == 0
                    && prop.GetMethod != null;
        }
    }
}