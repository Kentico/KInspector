using System.IO;
using System.Reflection;

namespace KenticoInspector.Core.Helpers
{
    public static class DirectoryHelper
    {
        /// <summary>
        /// Gets the executing directory of the application.
        /// </summary>
        /// <returns>A string that contains the path of the executing directory, and does not end with a backslash (\).</returns>
        public static string GetExecutingDirectory()
        {
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(assemblyPath);
        }
    }
}