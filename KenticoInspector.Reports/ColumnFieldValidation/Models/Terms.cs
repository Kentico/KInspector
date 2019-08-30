using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.ColumnFieldValidation.Models
{
    public class Terms
    {
        public Summaries Summaries { get; set; }

        public TableLabels TableLabels { get; set; }
    }

    public class Summaries
    {
        public Term Error { get; set; }

        public Term Good { get; set; }
    }

    public class TableLabels
    {
        public Term ClassesWithAddedFields { get; set; }

        public Term TablesWithAddedColumns { get; set; }
    }
}