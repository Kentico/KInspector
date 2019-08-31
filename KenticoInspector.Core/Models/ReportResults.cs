using System.Dynamic;

using KenticoInspector.Core.Constants;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KenticoInspector.Core.Models
{
    public class ReportResults
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ResultsStatus Status { get; set; }

        public string Summary { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ReportResultsType Type { get; set; }

        public dynamic Data { get; set; }

        public ReportResults()
        {
            Data = new ExpandoObject();
        }
    }
}