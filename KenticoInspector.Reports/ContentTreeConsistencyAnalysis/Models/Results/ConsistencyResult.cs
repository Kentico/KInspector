using KenticoInspector.Core.Constants;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Results
{
    public class ConsistencyResult
    {
        public ReportResultsStatus Status { get; set; }

        public string TableName { get; set; }

        public dynamic Data { get; set; }

        public ConsistencyResult(ReportResultsStatus status, string tableName, dynamic data)
        {
            Status = status;
            TableName = tableName;
            Data = data;
        }
    }
}