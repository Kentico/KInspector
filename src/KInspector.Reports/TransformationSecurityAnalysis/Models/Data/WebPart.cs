using System.Xml.Linq;

namespace KInspector.Reports.TransformationSecurityAnalysis.Models.Data
{
    public class WebPart
    {
        public string? ControlId { get; }

        public IEnumerable<WebPartProperty> Properties { get; private set; }

        public WebPart(XElement webPartXml)
        {
            ControlId = webPartXml.Attribute("controlid")?.Value;
            Properties = webPartXml.Elements("property")
                .Where(WebPartProperty.PropertyXmlContainsTransformation)
                .Select(xmlElement => new WebPartProperty(xmlElement))
                .ToList();
        }

        public void RemovePropertiesWithoutTransformations()
        {
            Properties = Properties
                        .Where(WebPartProperty.HasIssues);
        }
    }
}