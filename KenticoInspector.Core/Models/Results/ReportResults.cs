using KenticoInspector.Core.Constants;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KenticoInspector.Core.Models.Results
{
    public class ReportResults
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ReportResultsStatus Status { get; set; }

        public string Summary { get; set; }

        public ReportResultsData Data { get; set; }

        public ReportResults(ReportResultsStatus reportResultsStatus = ReportResultsStatus.Information)
        {
            Status = reportResultsStatus;
            Data = new ReportResultsData();
        }
    }
}