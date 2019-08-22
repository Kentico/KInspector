using System.Linq;
using System.Xml.Linq;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data
{
    public class WebConfigSetting
    {
        public string KeyName { get; set; }

        public string KeyValue { get; set; }

        public string KeyPath { get; }

        public WebConfigSetting(XElement element, string keyName, XAttribute attributeWithValue)
        {
            KeyName = keyName;
            KeyValue = attributeWithValue?.Value;
            KeyPath = GetPath(element);
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

            return string.Join("/", elementsOnPath);
        }
    }
}