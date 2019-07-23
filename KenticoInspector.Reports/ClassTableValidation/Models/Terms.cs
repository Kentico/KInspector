using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.ClassTableValidation.Models
{
    public class Terms
    {
        public Term CountIssueFound { get; set; }
        public Term DatabaseTablesWithMissingKenticoClasses { get; set; }
        public Term KenticoClassesWithMissingDatabaseTables { get; set; }
        public Term NoIssuesFound { get; set; }
    }
}