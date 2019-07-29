using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.ApplicationRestartAnalysis.Models
{
    public class Terms
    {
        public Term ApplicationRestartEvents { get; set; }
        public Term CountEndEvent { get; set; }
        public Term CountStartEvent { get; set; }
        public Term CountTotalEvent { get; set; }
        public Term SpanningEarliestLatest { get; set; }
    }
}