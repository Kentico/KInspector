using KenticoInspector.Core.Constants;
using System.Collections.Generic;
using System.Xml;

namespace KenticoInspector.Core.Services.Interfaces
{
    public interface ICmsFileService : IService
    {
        Dictionary<string, string> GetResourceStringsFromResx(string instanceRoot, string relativeResxFilePath = DefaultKenticoPaths.PrimaryResxFile);

        XmlDocument GetXmlDocument(string instanceRoot, string relativeFilePath);
    }
}