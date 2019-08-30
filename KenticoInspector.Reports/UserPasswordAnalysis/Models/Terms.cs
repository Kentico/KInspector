using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.UserPasswordAnalysis.Models
{
    public class Terms
    {
        public Term ErrorSummary { get; set; }

        public Term GoodSummary { get; set; }

        public TableLabels TableLabels { get; set; }
    }

    public class TableLabels
    {
        public Term EmptyPasswords { get; set; }

        public Term PlaintextPasswords { get; set; }
    }
}