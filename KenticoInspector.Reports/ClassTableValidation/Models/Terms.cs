using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.ClassTableValidation.Models
{
    public class Terms
    {
        public Summaries Summaries { get; set; }

        public TableTitles TableTitles { get; set; }
    }

    public class Summaries
    {
        public Term Error { get; set; }

        public Term Good { get; set; }
    }

    public class TableTitles
    {
        public Term DatabaseTablesWithMissingKenticoClasses { get; set; }

        public Term KenticoClassesWithMissingDatabaseTables { get; set; }
    }
}