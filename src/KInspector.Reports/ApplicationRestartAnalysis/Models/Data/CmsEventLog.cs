namespace KInspector.Reports.ApplicationRestartAnalysis.Models.Data
{
    public class CmsEventLog
    {
        public int EventID { get; set; }

        public string? EventCode { get; set; }

        public DateTime EventTime { get; set; }

        public string? EventMachineName { get; set; }
    }
}