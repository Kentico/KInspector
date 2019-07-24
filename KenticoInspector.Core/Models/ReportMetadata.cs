using Newtonsoft.Json;

namespace KenticoInspector.Core.Models
{
    public class ReportMetadata<TTerms> where TTerms : new()
    {
        public ReportDetails Details { get; set; }

        [JsonIgnore]
        public TTerms Terms { get; set; }
    }
}