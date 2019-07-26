using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using KenticoInspector.Reports.TransformationSecurityAnalysis.Constants;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Analysis;

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
            return propertyXml.Attribute(XmlConstants.Name).Value;
        }

        public static bool PropertyXmlContainsTransformation(XElement propertyXml)
        {
            var propertyXmlContainsTransformation = GetNameFromPropertyXml(propertyXml)
                .Contains(XmlConstants.Transformation, StringComparison.InvariantCultureIgnoreCase);

            var propertyXmlIsNotEmpty = !string.IsNullOrEmpty(propertyXml.Value);

            return propertyXmlContainsTransformation
                && propertyXmlIsNotEmpty;
        }

        public static Transformation PropertyTransformation(WebPartProperty property)
        {
            return property.Transformation;
        }

        public static bool HasIssues(WebPartProperty property)
        {
            if (property.Transformation == null)
            {
                return false;
            }

            return TransformationIssues(property)
                .Any();
        }

        public static IEnumerable<TransformationIssue> TransformationIssues(WebPartProperty property)
        {
            return property.Transformation.Issues;
        }
    }
}