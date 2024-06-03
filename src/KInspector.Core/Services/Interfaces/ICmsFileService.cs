using KInspector.Core.Constants;

using System.Xml;

namespace KInspector.Core.Services.Interfaces
{
    /// <summary>
    /// Contains methods for reading files from a Kentico instance's filesystem.
    /// </summary>
    public interface ICmsFileService : IService
    {
        /// <summary>
        /// Gets a key/vair pair of resource strings from the provided .resx file.
        /// </summary>
        /// <param name="instanceRoot">The root of the Kentico administration website.</param>
        /// <param name="relativeResxFilePath">The relative path of the .resx file within the <paramref name="instanceRoot"/>.</param>
        Dictionary<string, string> GetResourceStringsFromResx(string? instanceRoot, string relativeResxFilePath = DefaultKenticoPaths.PrimaryResxFile);

        /// <summary>
        /// Gets the CMSConnectionString from the provided web.config file.
        /// </summary>
        /// <param name="instanceRoot">The root of the Kentico administration website.</param>
        /// <param name="relativeWebConfigFilePath">The relative path of the web.config file within the <paramref name="instanceRoot"/>.</param>
        string? GetCMSConnectionString(string? instanceRoot, string relativeWebConfigFilePath = DefaultKenticoPaths.WebConfigFile);

        /// <summary>
        /// Gets the XML contents of the provided .xml file.
        /// </summary>
        /// <param name="instanceRoot">The root of the Kentico administration website.</param>
        /// <param name="relativeFilePath">The relative path of the .xml file within the <paramref name="instanceRoot"/>.</param>
        /// <returns></returns>
        XmlDocument? GetXmlDocument(string? instanceRoot, string relativeFilePath);
    }
}