using KInspector.Core.Models;

namespace KInspector.Reports.DatabaseTableSizeAnalysis.Models
{
    public class Terms
    {
        public Term? CheckResultsTableForAnyIssues { get; set; }

        public Term? Top25Results { get; set; }
    }
}