using Newtonsoft.Json;

namespace KInspector.Reports.WebPartPerformanceAnalysis.Models
{
    public class Document
    {
        public string? DocumentName { get; set; }

        public int DocumentPageTemplateID { get; set; }

        [JsonIgnore]
        public string? DocumentWebParts { get; set; }

        public string? NodeAliasPath { get; set; }

        public int NodeSiteID { get; set; }
    }
}