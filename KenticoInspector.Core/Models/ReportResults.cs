namespace KenticoInspector.Core.Models
{
    public class ReportResults
    {
        public string Status { get; set; }
        public string Summary { get; set; }
        public string Type { get; set; }
        public dynamic Data { get; set; }
    }
}