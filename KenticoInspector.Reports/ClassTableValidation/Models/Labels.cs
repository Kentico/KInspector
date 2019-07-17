using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.ClassTableValidation.Models
{
    public class Labels
    {
        public Label DatabaseTablesWithMissingKenticoClasses { get; set; }

        public Label KenticoClassesWithMissingDatabaseTables { get; set; }

        public Label NoIssuesFound { get; set; }

        public Label CountIssueFound { get; set; }
    }
}