using Newtonsoft.Json;

namespace KenticoInspector.Core.Models
{
    public class ReportDetails
    {
        [JsonConverter(typeof(TermConverter))]
        public Term Name { get; set; }

        [JsonConverter(typeof(TermConverter))]
        public Term ShortDescription { get; set; }

        [JsonConverter(typeof(TermConverter))]
        public Term LongDescription { get; set; }
    }
}