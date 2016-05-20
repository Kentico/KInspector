using System;
using System.Collections.Generic;
using System.Linq;

namespace Kentico.KInspector.Modules
{
    /// <summary>
    /// Provides methods for validating macros.
    /// </summary>
    public class MacroValidator
    {
        private static readonly Version CustomDeprecatedOn = new Version("8.0");

        /// <summary>
        /// Enumerator for macro types to validate
        /// </summary>
        [Flags]
        public enum MacroType
        {
            Context = 1,
            Query = 2,
            Custom = 4
        }

        /// <summary>
        /// Checks whether <paramref name="version"/> should be checked for deprecated custom macros.
        /// </summary>
        /// <param name="version">Kentico instance version.</param>
        /// <returns>True if version is greater than 8.0, false otherwise.</returns>
        public bool CheckForCustomMacros(Version version)
        {
            return (version >= CustomDeprecatedOn);
        }
        /// <summary>
        /// Checks whether <paramref name="text"/> contains macros.
        /// </summary>
        /// <param name="text">Text with or without macros.</param>
        /// <returns>True if text contains macros, false otherwise.</returns>
        public bool ContainsMacros(string text)
        {
            return ContainsMacros(text, (MacroType.Context | MacroType.Query | MacroType.Custom));
        }

        /// <summary>
        /// Checks whether <paramref name="text"/> contains macros of defined <paramref name="type"/>.
        /// </summary>
        /// <param name="text">Text with or without macros.</param>
        /// <param name="type">Type of macros to check.</param>
        /// <returns>True if text contains macros, false otherwise.</returns>
        public bool ContainsMacros(string text, MacroType type)
        {
            if (type.HasFlag(MacroType.Context) && text.Contains("{%"))
                return true;
            if (type.HasFlag(MacroType.Query) && text.Contains("{?"))
                return true;
            if (type.HasFlag(MacroType.Custom) && text.Contains("{#"))
                return true;
            return false;
        }

        /// <summary>
        /// Highlights context, query and custom macros in <paramref name="code"/>
        /// using HTML syntax.
        /// </summary>
        /// <param name="code">Code with macros.</param>
        /// <returns>Code with HTML highlighted macros.</returns>
        public string HighlightMacros(string code)
        {
            return HighlightMacros(code, (MacroType.Context | MacroType.Query | MacroType.Custom));
        }

        /// <summary>
        /// Highlights macros of defined <paramref name="type"/> in <paramref name="code"/>
        /// using HTML syntax.
        /// </summary>
        /// <param name="code">Code with macros.</param>
        /// <param name="type">Type of macros to highlight.</param>
        /// <returns>Code with HTML highlighted macros.</returns>
        public string HighlightMacros(string code, MacroType type)
        {
            List<string> macros = new List<string>();
            if (type.HasFlag(MacroType.Context))
                macros.AddRange(GetMacros(code, "%"));

            if (type.HasFlag(MacroType.Query))
                macros.AddRange(GetMacros(code, "?"));

            if (type.HasFlag(MacroType.Custom))
                macros.AddRange(GetMacros(code, "#"));

            foreach (string macro in macros)
            {
                code = code.Replace(macro, "<span style=\"color: red;\">" + macro + "</span>");
            }

            return code;
        }


        /// <summary>
        /// Gets macro expressions from <paramref name="code"/>.
        /// </summary>
        /// <param name="code">Code to be searched for macro expressions.</param>
        /// <param name="macroType">Macro type to be searched (e.g. "%", "?").</param>
        /// <param name="startIndex">Position from which to start the search.</param>
        /// <param name="matches">Set of matches to which the macros are added.</param>
        /// <returns></returns>
        private ISet<string> GetMacros(string code, string macroType, int startIndex = 0, ISet<string> matches = null)
        {
            if (matches == null)
            {
                matches = new HashSet<string>();
            }

            int start = code.IndexOf("{" + macroType, startIndex);
            if (start >= 0)
            {
                int end = code.IndexOf(macroType + "}", start + 2);

                if (end >= 0)
                {
                    string macroExpression = code.Substring(start, end - start + 2);
                    matches.Add(macroExpression);

                    return GetMacros(code, macroType, end + 2, matches);
                }
            }

            return matches;
        }
    }
}
