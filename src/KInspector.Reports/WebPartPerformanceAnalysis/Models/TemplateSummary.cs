using Newtonsoft.Json;

namespace KInspector.Reports.WebPartPerformanceAnalysis.Models
{
    public class TemplateSummary
    {
        [JsonIgnore]
        public IEnumerable<Document> AffectedDocuments { get; set; } = Enumerable.Empty<Document>();

        [JsonIgnore]
        public IEnumerable<WebPartSummary> AffectedWebParts { get; set; } = Enumerable.Empty<WebPartSummary>();

        public string? TemplateCodename { get; set; }

        public string? TemplateName { get; set; }

        public int TemplateID { get; set; }

        public int TotalAffectedDocuments => AffectedDocuments.Count();

        public int TotalAffectedWebParts => AffectedWebParts.Count();
    }
}