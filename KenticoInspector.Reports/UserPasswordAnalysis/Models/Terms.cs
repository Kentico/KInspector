using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.UserPasswordAnalysis.Models
{
    public class Terms
    {
        public Term ErrorSummary { get; set; }

        public Term GoodSummary { get; set; }

        public TableTitlesTerms TableTitles { get; set; }
    }

    public class TableTitlesTerms
    {
        public Term EmptyPasswords { get; set; }

        public Term PlaintextPasswords { get; set; }
    }
}