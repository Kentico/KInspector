using System.IO;
using System.Reflection;

namespace KenticoInspector.Infrastructure.Helpers
{
    public class DirectoryHelper
    {
        public static string GetExecutingDirectory()
        {
            var assemblyPath = Assembly.GetExecutingAssembly().CodeBase;
            var assemblyDirectory = Path.GetDirectoryName(assemblyPath);
            var filePrefix = "file:\\";
            return assemblyDirectory.Substring(filePrefix.Length);
        }
    }
}