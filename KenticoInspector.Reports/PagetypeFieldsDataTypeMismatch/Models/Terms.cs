using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.PagetypeFieldsDataTypeMisMatch.Models
{
    public class Terms
    {
        public Summaries Summaries { get; set; }
        
        public TableLabels TableLabels { get; set; }
    }
    public class Summaries
    {
        public Term Information { get; set; }

        public Term Good { get; set; }
    }
    public class TableLabels
    {
        public Term FieldsWithMismatchedTypes { get; set; }
    }
}
