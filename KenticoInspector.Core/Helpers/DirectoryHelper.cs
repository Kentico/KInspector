using System.IO;
using System.Reflection;

namespace KenticoInspector.Core.Helpers
{
    public class DirectoryHelper
    {
        private const string FilePrefix = "file:\\";

        /// <summary>
        /// Gets the executing directory of the application.
        /// </summary>
        /// <returns>A string that contains the path of the executing directory, and does not end with a backslash (\).</returns>
        public static string GetExecutingDirectory()
        {
            var assemblyPath = Assembly.GetExecutingAssembly().CodeBase;

            var assemblyDirectory = Path.GetDirectoryName(assemblyPath);

            return assemblyDirectory.Substring(FilePrefix.Length);
        }
    }
}