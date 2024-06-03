using KInspector.Core.Models;

namespace KInspector.Reports.PageTypeFieldAnalysis.Models
{
    public class Terms
    {
        public Summaries? Summaries { get; set; }

        public TableTitles? TableTitles { get; set; }
    }

    public class Summaries
    {
        public Term? Information { get; set; }

        public Term? Good { get; set; }
    }

    public class TableTitles
    {
        public Term? MatchingPageTypeFieldsWithDifferentDataTypes { get; set; }
    }
}