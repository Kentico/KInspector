using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.WebPartPerformanceAnalysis.Models
{
    public class WebPartSummary
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int TemplateId { get; set; }

        [JsonIgnore]
        public IEnumerable<Document> Documents { get; set; }

        public int DocumentCount => Documents.Count();
    }
}