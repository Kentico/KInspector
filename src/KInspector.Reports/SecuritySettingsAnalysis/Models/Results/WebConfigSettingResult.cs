using System.Web;
using System.Xml.Linq;

using KInspector.Core.Models;

namespace KInspector.Reports.SecuritySettingsAnalysis.Models.Results
{
    public class WebConfigSettingResult
    {
        public string KeyPath { get; set; }

        public string KeyName { get; set; }

        public string KeyValue { get; set; }

        public string RecommendedValue { get; set; }

        public string RecommendationReason { get; set; }

        public WebConfigSettingResult(
            XElement element,
            string keyName,
            string keyValue,
            string recommendedValue,
            Term recommendationReason
            )
        {
            KeyPath = GetPath(element);
            KeyName = keyName;
            KeyValue = keyValue;
            RecommendedValue = recommendedValue;
            RecommendationReason = recommendationReason;
        }

        private string GetPath(XElement element)
        {
            var elementsOnPath = element.AncestorsAndSelf()
                .Reverse()
                .Select(elementOnPath =>
                {
                    var trimmedElement = new XElement(elementOnPath);
                    trimmedElement.RemoveNodes();

                    return trimmedElement.ToString().Replace(" />", ">");
                });

            var path = string.Join("/", elementsOnPath);

            return HttpUtility.HtmlEncode(path);
        }
    }
}