using System.IO;
using System.Reflection;

namespace Kentico.KInspector.Core
{
    public static class PathHelper
    {
        public static string GetExecutingPath()
        {
            var assemblyPath = Assembly.GetExecutingAssembly().CodeBase;
            var assemblyDirectory = Path.GetDirectoryName(assemblyPath);
            var filePrefix = "file:\\";
            return assemblyDirectory.Substring(filePrefix.Length);
        }
    }
}
