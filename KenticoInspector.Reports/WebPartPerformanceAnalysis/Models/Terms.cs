using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.WebPartPerformanceAnalysis.Models
{
    public class HeaderTerms
    {
        public Term DocumentSummary { get; set; }
        public Term TemplateSummary { get; set; }
        public Term WebPartSummary { get; set; }
    }

    public class Terms
    {
        public HeaderTerms Headers { get; set; }
        public Term Summary { get; set; }
    }
}