using System.Xml.Linq;

namespace KenticoInspector.Reports.WebPartPerformanceAnalysis.Models
{
    public class PageTemplate
    {
        public string PageTemplateCodeName { get; set; }

        public string PageTemplateDisplayName { get; set; }

        public int PageTemplateID { get; set; }

        public XDocument PageTemplateWebParts { get; set; }
    }
}