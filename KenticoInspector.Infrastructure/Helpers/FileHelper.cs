using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KenticoInspector.Infrastructure.Helpers
{
    static class FileHelper
    {
        public static string GetSqlQueryText(string relativeFilePath)
        {
            var executingDirectory = DirectoryHelper.GetExecutingDirectory();
            var fullPathToScript = $"{executingDirectory}/{relativeFilePath}";
            return File.ReadAllText(fullPathToScript);
        }
    }
}
