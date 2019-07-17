using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.ApplicationRestartAnalysis.Models
{
    public class Labels
    {
        public Label ApplicationRestartEvents { get; set; }

        public Label CountTotalEvent { get; set; }

        public Label CountStartEvent { get; set; }

        public Label CountEndEvent { get; set; }

        public Label SpanningEarliestLatest { get; set; }
    }
}