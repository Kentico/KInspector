using KInspector.Core.Models;

namespace KInspector.Reports.ClassTableValidation.Models
{
    public class Terms
    {
        public Term? CountIssueFound { get; set; }
        public Term? DatabaseTablesWithMissingKenticoClasses { get; set; }
        public Term? KenticoClassesWithMissingDatabaseTables { get; set; }
        public Term? NoIssuesFound { get; set; }
    }
}