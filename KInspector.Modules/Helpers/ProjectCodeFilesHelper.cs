using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kentico.KInspector.Modules
{
    /// <summary>
    /// Provides methods for listing of project code files.
    /// </summary>
    public class ProjectCodeFilesHelper
    {
        #region "Constants"

        /// <summary>
        /// Path to directory where metafiles with information about default installation files are located.
        /// </summary>
        private const string DEFAULT_INSTALLATION_FILES_DIR_PATH = "Data\\DefaultInstallationFiles";


        public static readonly ProjectCodeFilesHelper Current = new ProjectCodeFilesHelper();

        #endregion


        #region "Public methods"

        /// <summary>
        /// Gets enumeration of .cs, .aspx and .ascx files (.designer.cs files are excluded)
        /// residing in <paramref name="pathToKenticoInstance"/> which are not shipped with default Kentico installation.
        /// The files are relative paths within <paramref name="pathToKenticoInstance"/>.
        /// </summary>
        /// <param name="pathToKenticoInstance">Path to Kentico instance (e.g. <c>C:\inetpub\wwwroot\myKenticoInstance\CMS</c>).</param>
        /// <param name="version">Kentico version.</param>
        /// <param name="isWebSiteProject">Whether to return default files of web site or web application project.</param>
        /// <returns>Enumeration of customer code files.</returns>
        public IEnumerable<string> GetCustomerProjectCodeFiles(DirectoryInfo pathToKenticoInstance, Version version, bool isWebSiteProject, bool onlyCsFiles = false)
        {
            IEnumerable<string> projectFiles = GetProjectCodeFiles(pathToKenticoInstance.FullName, onlyCsFiles);
            ISet<string> defaultProjectFiles = new HashSet<string>(GetDefaultProjectCodeFiles(version, isWebSiteProject));

            return projectFiles.Where(it => !defaultProjectFiles.Contains(it, StringComparer.InvariantCultureIgnoreCase));
        }


        /// <summary>
        /// Gets enumeration of .cs, .aspx and .ascx files (.designer.cs files are excluded)
        /// residing in <paramref name="pathToKenticoInstance"/>.
        /// The files are relative paths within <paramref name="pathToKenticoInstance"/>.
        /// </summary>
        /// <param name="pathToKenticoInstance">Path to Kentico instance (e.g. <c>C:\inetpub\wwwroot\myKenticoInstance\CMS</c>).</param>
        /// <param name="onlyCsFiles">Whether to select only .cs files</param>
        /// <returns>Enumeration of code files.</returns>
        public IEnumerable<string> GetProjectCodeFiles(string pathToKenticoInstance, bool onlyCsFiles)
        {
            if (!pathToKenticoInstance.EndsWith("\\"))
            {
                pathToKenticoInstance += "\\";
            }

            var csFiles = Directory.EnumerateFiles(pathToKenticoInstance, "*.cs", SearchOption.AllDirectories);
            csFiles = csFiles.Where(it => !it.EndsWith(".designer.cs"));

            var aspxFiles = Directory.EnumerateFiles(pathToKenticoInstance, "*.aspx", SearchOption.AllDirectories);
            var ascxFiles = Directory.EnumerateFiles(pathToKenticoInstance, "*.ascx", SearchOption.AllDirectories);

            if (onlyCsFiles)
            {
                return csFiles.Select(it => it.Substring(pathToKenticoInstance.Length));
            }

            return csFiles.Concat(aspxFiles).Concat(ascxFiles).Select(it => it.Substring(pathToKenticoInstance.Length));
        }


        /// <summary>
        /// Gets enumeration of .cs, .aspx and .ascx files (.designer.cs files are excluded)
        /// shipped with default Kentico installation.
        /// The files are relative paths within default installation.
        /// </summary>
        /// <param name="version">Kentico version.</param>
        /// <param name="isWebSiteProject">Whether to return default files of web site or web application project.</param>
        /// <returns>Enumeration of default code files.</returns>
        /// <exception cref="ArgumentException">Thrown when version is not supported.</exception>
        public IEnumerable<string> GetDefaultProjectCodeFiles(Version version, bool isWebSiteProject)
        {
            try
            {
                var contentLines = File.ReadAllLines(GetMetaFilePath(version, isWebSiteProject));

                return contentLines;
            }
            catch (FileNotFoundException ex)
            {
                // Thrown when metafile for given version is not available
                throw new ArgumentException($"Default project code files listing for version {version.ToString(2)} is not supported.", ex);
            }
        }


        /// <summary>
        /// Tells you whether instance is web site or web application
        /// (based on presence of some well-known .designer.cs file).
        /// </summary>
        /// <param name="pathToKenticoInstance">Path to Kentico instance (e.g. <c>C:\inetpub\wwwroot\myKenticoInstance\CMS</c>).</param>
        /// <returns>True if instance is web site, false otherwise.</returns>
        public bool IsWebSiteProject(DirectoryInfo pathToKenticoInstance)
        {
            const string testedFileName = "default.aspx.designer.cs";
            string testedFilePath = Path.Combine(pathToKenticoInstance.FullName, testedFileName);

            // Web site does not contain designer files
            return !File.Exists(testedFilePath);
        }

        #endregion


        #region "Private methods"

        /// <summary>
        /// Gets path to metafile with information about default installation files.
        /// </summary>
        /// <param name="version">Kentico version.</param>
        /// <param name="isWebSiteProject">Whether to return metafile for web site or web application project.</param>
        /// <returns></returns>
        private string GetMetaFilePath(Version version, bool isWebSiteProject)
        {
            string metaFileName = $"K{version.Major}{(version.Minor > 0 ? version.Minor.ToString() : string.Empty)}web{(isWebSiteProject ? "site" : "app")}.txt";
            
            return Path.Combine(DEFAULT_INSTALLATION_FILES_DIR_PATH, metaFileName);
        }

        #endregion
    }
}
