using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.DatabaseConsistencyCheck.Models
{
    public class Terms
    {
        public Term CheckResultsTableForAnyIssues { get; set; }

        public Term NoIssuesFound { get; set; }
    }
}