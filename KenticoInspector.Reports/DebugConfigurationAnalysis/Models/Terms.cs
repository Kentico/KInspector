using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.DebugConfigurationAnalysis.Models
{
    public class DatabaseTerms
    {
        public Term ExplicitlyEnabledSettingsTableHeader { get; set; }

        public Term OverviewTableHeader { get; set; }

        public Term Summary { get; set; }
    }

    public class Terms
    {
        public DatabaseTerms Database { get; set; }

        public WebConfigTerms WebConfig { get; set; }
    }

    public class WebConfigTerms
    {
        public Term DebugKeyDisplayName { get; set; }

        public Term OverviewTableHeader { get; set; }

        public Term Summary { get; set; }

        public Term TraceKeyDisplayName { get; set; }
    }
}