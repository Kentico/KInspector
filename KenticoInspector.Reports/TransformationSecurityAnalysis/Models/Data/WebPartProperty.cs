using System;
using System.Linq;
using System.Xml.Linq;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data
{
    public class WebPartProperty
    {
        public string Name { get; }

        public string TransformationFullName { get; }

        public Transformation Transformation { get; set; }

        public WebPartProperty(XElement propertyXml)
        {
            Name = GetNameFromPropertyXml(propertyXml);
            TransformationFullName = propertyXml.Value;
        }

        private static string GetNameFromPropertyXml(XElement propertyXml)
        {
            return propertyXml.Attribute("name").Value;
        }

        public static bool PropertyXmlContainsTransformation(XElement propertyXml)
        {
            var propertyXmlContainsTransformation = GetNameFromPropertyXml(propertyXml)
                .Contains("transformation", StringComparison.InvariantCultureIgnoreCase);

            var propertyXmlIsNotEmpty = !string.IsNullOrEmpty(propertyXml.Value);

            return propertyXmlContainsTransformation
                && propertyXmlIsNotEmpty;
        }

        public static bool HasIssues(WebPartProperty property)
        {
            if (property.Transformation == null)
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