using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Services.Interfaces;

using System.Collections.Generic;
using System.Xml;

namespace KenticoInspector.Core.Helpers
{
    public class CmsFileService : ICmsFileService
    {
        public Dictionary<string, string> GetResourceStringsFromResx(string instanceRoot, string relativeResxFilePath = DefaultKenticoPaths.PrimaryResxFile)
        {
            var resourceXml = GetXmlDocument(instanceRoot, relativeResxFilePath);
            var resourceStringNodes = resourceXml?.SelectNodes("/root/data");
            var results = new Dictionary<string, string>();
            foreach (XmlNode resourceStringNode in resourceStringNodes)
            {
                var key = resourceStringNode.Attributes["name"].InnerText.ToLowerInvariant();
                var value = resourceStringNode.SelectSingleNode("./value").InnerText;
                results.Add(key, value);
            }

            return results;
        }

        public XmlDocument GetXmlDocument(string instanceRoot, string relativeFilePath)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(instanceRoot + relativeFilePath);

            return xmlDocument;
        }
    }
}