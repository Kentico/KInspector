using System.Collections.Generic;
using System.IO;

namespace Kentico.KInspector.Modules
{
    /// <summary>
    /// Provides methods useful for Kentico instance probe installation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All the probe files must reside within <c>ProbeData</c> directory.
    /// The directory content is copied to the Kentico instance folder.
    /// </para>
    /// <para>
    /// Make sure the content folder items have their <c>Build Action</c> set to "Content"
    /// and <c>Copy to Output Directory</c> set to "Copy Always" (because you want
    /// the target instance to compile those files, not this tool).
    /// Otherwise errors may be encountered when compiling this tool.
    /// </para>
    /// <para>
    /// For ease of deployment to both web site and web application the probe file is
    /// a web form with <c>CodeFile</c> set in its markup and with <c>.designer.cs</c> file generated.
    /// But the probe helper should be able to deploy anything (e.g. handler + DLL).
    /// </para>
    /// </remarks>
    public static class ProbeHelper
    {
        /// <summary>
        /// Path to directory where all the files the probe needs reside (with trailing slash).
        /// The content of the directory is copied to the Kentico instance upon install.
        /// </summary>
        private const string PROBE_DATA_FOLDER_PATH = ".\\ProbeData\\";


        /// <summary>
        /// Gets enumeration of probe files as an enumeration of relative paths.
        /// </summary>
        private static IEnumerable<string> ProbeFilesRelativePath => Directory.EnumerateFiles(PROBE_DATA_FOLDER_PATH, "*", SearchOption.AllDirectories);

        /// <summary>
        /// Installs the probe for Kentico instance residing in <paramref name="pathToKenticoFiles"/>
        /// (e.g. <c>C:\inetpub\wwwroot\myKenticoInstance\CMS</c>).
        /// </summary>
        /// <param name="pathToKenticoFiles">Path to Kentico instance.</param>
        public static void InstallProbe(DirectoryInfo pathToKenticoFiles)
        {
            foreach (var probeFile in ProbeFilesRelativePath)
            {
                string relativePathWithinInstance = probeFile.Substring(PROBE_DATA_FOLDER_PATH.Length);
                string targetPath = Path.Combine(pathToKenticoFiles.FullName, relativePathWithinInstance);
                File.Copy(probeFile, targetPath, true);
            }
        }


        /// <summary>
        /// Uninstalls the probe for Kentico instance residing in <paramref name="pathToKenticoFiles"/>
        /// (e.g. <c>C:\inetpub\wwwroot\myKenticoInstance\CMS</c>).
        /// </summary>
        /// <param name="pathToKenticoFiles">Path to Kentico instance.</param>
        /// <returns>True if the probe was uninstalled, false if there was nothing to uninstall.</returns>
        public static void UninstallProbe(DirectoryInfo pathToKenticoFiles)
        {
            foreach (var probeFile in ProbeFilesRelativePath)
            {
                string relativePathWithinInstance = probeFile.Substring(PROBE_DATA_FOLDER_PATH.Length);
                string targetPath = Path.Combine(pathToKenticoFiles.FullName, relativePathWithinInstance);
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
            }
        }
    }
}
