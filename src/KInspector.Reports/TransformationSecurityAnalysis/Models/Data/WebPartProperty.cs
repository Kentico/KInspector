using System.Xml.Linq;

namespace KInspector.Reports.TransformationSecurityAnalysis.Models.Data
{
    public class WebPartProperty
    {
        public string Name { get; }

        public string TransformationFullName { get; }

        public Transformation? Transformation { get; set; }

        public WebPartProperty(XElement propertyXml)
        {
            Name = GetNameFromPropertyXml(propertyXml);
            TransformationFullName = propertyXml.Value;
        }

        private static string? GetNameFromPropertyXml(XElement propertyXml)
        {
            return propertyXml.Attribute("name")?.Value;
        }

        public static bool PropertyXmlContainsTransformation(XElement propertyXml)
        {
            var propertyXmlContainsTransformation = GetNameFromPropertyXml(propertyXml)?
                .Contains("transformation", StringComparison.InvariantCultureIgnoreCase) ?? false;

            var propertyXmlIsNotEmpty = !string.IsNullOrEmpty(propertyXml.Value);

            return propertyXmlContainsTransformation
                && propertyXmlIsNotEmpty;
        }

        public static bool HasIssues(WebPartProperty property)
        {
            if (property.Transformation is null)
            {
                return false;
            }

            return property
                .Transformation
                .Issues
                .Any();
        }
    }
}