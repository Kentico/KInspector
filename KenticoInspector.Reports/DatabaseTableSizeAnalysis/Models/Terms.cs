using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.DatabaseTableSizeAnalysis.Models
{
    public class Terms
    {
        public Term CheckResultsTableForAnyIssues { get; set; }

        public Term Top25Results { get; set; }
    }
}