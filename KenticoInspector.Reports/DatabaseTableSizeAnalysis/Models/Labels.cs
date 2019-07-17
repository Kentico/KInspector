using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.DatabaseTableSizeAnalysis.Models
{
    public class Labels
    {
        public Label CheckResultsTableForAnyIssues { get; set; }

        public Label Top25Results { get; set; }
    }
}