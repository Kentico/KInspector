using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.DatabaseConsistencyCheck.Models
{
    public class Labels
    {
        public Label CheckResultsTableForAnyIssues { get; set; }

        public Label NoIssuesFound { get; set; }
    }
}