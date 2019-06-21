using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KenticoInspector.Infrastructure.Helpers
{
    static class FileHelper
    {
        public static string GetSqlQueryText(string relativeFilePath, IDictionary<string,string> literalReplacements = null)
        {
            var executingDirectory = DirectoryHelper.GetExecutingDirectory();
            var fullPathToScript = $"{executingDirectory}/{relativeFilePath}";
            var query = File.ReadAllText(fullPathToScript);

            if(literalReplacements != null)
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
