using KInspector.Core.Models;

namespace KInspector.Reports.DatabaseConsistencyCheck.Models
{
    public class Terms
    {
        public Term? CheckResultsTableForAnyIssues { get; set; }

        public Term? NoIssuesFound { get; set; }
    }
}