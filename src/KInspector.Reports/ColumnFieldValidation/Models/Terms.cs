using KInspector.Core.Models;

namespace KInspector.Reports.ColumnFieldValidation.Models
{
    public class Terms
    {
        public Summaries? Summaries { get; set; }

        public TableTitles? TableTitles { get; set; }
    }

    public class Summaries
    {
        public Term? Error { get; set; }

        public Term? Good { get; set; }
    }

    public class TableTitles
    {
        public Term? ClassesWithAddedFields { get; set; }

        public Term? TablesWithAddedColumns { get; set; }
    }
}