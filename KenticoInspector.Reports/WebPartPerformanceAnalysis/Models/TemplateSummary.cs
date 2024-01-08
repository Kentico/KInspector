using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.WebPartPerformanceAnalysis.Models
{
    public class TemplateSummary
    {
        [JsonIgnore]
        public IEnumerable<Document> AffectedDocuments { get; set; }

        [JsonIgnore]
        public IEnumerable<WebPartSummary> AffectedWebParts { get; set; }

        public string TemplateCodename { get; set; }

        public string TemplateName { get; set; }

        public int TemplateID { get; set; }

        public int TotalAffectedDocuments => AffectedDocuments.Count();

        public int TotalAffectedWebParts => AffectedWebParts.Count();
    }
}