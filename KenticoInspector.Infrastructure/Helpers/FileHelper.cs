using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KenticoInspector.Infrastructure.Helpers
{
    static class FileHelper
    {
        public static string GetSqlScriptText(string relativeFilePath)
        {
            var executingDirectory = DirectoryHelper.GetExecutingDirectory();
            var fullPathToScript = $"{executingDirectory}/Scripts/{relativeFilePath}";
            return File.ReadAllText(fullPathToScript);
        }
    }
}
