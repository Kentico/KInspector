using System.Collections.Generic;
using System.IO;

namespace KenticoInspector.Core.Helpers
{
    public class FileHelper
    {
        /// <summary>
        /// Reads file located at <paramref name="relativeFilePath"/> using <see cref="DirectoryHelper.GetExecutingDirectory"/> as the root location.
        /// </summary>
        /// <param name="relativeFilePath">Relative file path not starting with a slash (/).</param>
        /// <param name="literalReplacements">Dictionary of string replacements.</param>
        /// <returns></returns>
        public static string GetSqlQueryText(string relativeFilePath, IDictionary<string, string> literalReplacements = null)
        {
            var executingDirectory = DirectoryHelper.GetExecutingDirectory();

            var fullPathToScript = $"{executingDirectory}/{relativeFilePath}";

            var query = File.ReadAllText(fullPathToScript);

            if (literalReplacements != null)
            {
                foreach (var replacement in literalReplacements)
                {
                    query = query.Replace(replacement.Key, replacement.Value);
                }
            }

            return query;
        }
    }
}