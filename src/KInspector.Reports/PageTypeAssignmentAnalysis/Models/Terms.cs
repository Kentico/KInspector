using KInspector.Core.Models;

namespace KInspector.Reports.PageTypeAssignmentAnalysis.Models
{
    public class Terms
    {
        public Term? WarningSummary { get; set; }

        public Term? UnassignedPageTypesTableHeader { get; set; }

        public Term? NoIssuesFound { get; set; }
    }
}