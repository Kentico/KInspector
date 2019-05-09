using System.Dynamic;

namespace KenticoInspector.Core.Models
{
    public class ReportResults
    {
        public ReportResults()
        {
            Data = new ExpandoObject();
        }

        public dynamic Data { get; set; }
        public string Status { get; set; }
        public string Summary { get; set; }
        public string Type { get; set; }
    }
}