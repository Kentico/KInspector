using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.ApplicationRestartAnalysis.Models
{
    public class Terms
    {
        public Summaries Summaries { get; set; }

        public TableTitles TableTitles { get; set; }
    }

    public class Summaries
    {
        public Term Good { get; set; }

        public Term Information { get; set; }
    }

    public class TableTitles
    {
        public Term ApplicationRestartEvents { get; set; }
    }
}