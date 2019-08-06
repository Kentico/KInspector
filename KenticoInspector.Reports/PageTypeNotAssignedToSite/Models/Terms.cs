using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.PageTypeNotAssignedToSite.Models
{
    public class Terms
    {
        public Term WarningSummary { get; set; }
        public Term UnassignedPageTypesTableHeader { get; set; }
        public Term NoIssuesFound { get; set; }
    }
}