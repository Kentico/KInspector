using System.Text;

namespace KInspector.Core.Helpers
{
    public static class VersionHelper
    {
        public static IList<Version> GetVersionList(params string[] versions)
        {
            return versions
                .Select(GetVersionFromShortString)
                .ToList();
        }

        public static Version GetVersionFromShortString(string version)
        {
            var expandedVersionString = ExpandVersionString(version);

            return new Version(expandedVersionString);
        }

        public static string ExpandVersionString(string version)
        {
            var builder = new StringBuilder(version);
            for (int i = 0; i < 2; i++)
            {
                builder.Append(".0");
            }

            return builder.ToString();
        }
    }
}