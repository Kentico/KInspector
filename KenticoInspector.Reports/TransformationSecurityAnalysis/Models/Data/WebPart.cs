using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using KenticoInspector.Reports.TransformationSecurityAnalysis.Constants;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data
{
    public class WebPart
    {
        public string ControlId { get; }

        public IEnumerable<WebPartProperty> Properties { get; private set; }

        public WebPart(XElement webPartXml)
        {
            ControlId = webPartXml.Attribute(XmlConstants.WebPartControlId).Value;
            Properties = webPartXml.Elements(XmlConstants.Property)
                        .Where(WebPartProperty.PropertyXmlContainsTransformation)
                        .Select(xmlElement => new WebPartProperty(xmlElement))
                        .ToList();
        }

        public void RemovePropertiesWithoutTransformations()
        {
            Properties = Properties
                        .Where(WebPartProperty.HasIssues);
        }

        public static IEnumerable<WebPartProperty> WebPartProperties(WebPart webPart)
        {
            return webPart.Properties;
        }

        public static bool HasProperties(WebPart webPart)
        {
            return WebPartProperties(webPart)
                .Any();
        }
    }
}