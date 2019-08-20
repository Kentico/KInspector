using System.Xml.Linq;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data
{
    public class AppSetting : WebConfigSetting
    {
        public AppSetting(XElement element)
            : base(element, element.Attribute("key").Value, element.Attribute("value").Value)
        {
        }
    }
}