using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.TaskProcessingAnalysis.Models
{
    public class Terms
    {
        public Term GoodSummary { get; set; }

        public Term CountIntegrationBusTask { get; set; }

        public Term CountScheduledTask { get; set; }

        public Term CountSearchTask { get; set; }

        public Term CountStagingTask { get; set; }

        public Term CountUnprocessedTask { get; set; }

        public Term CountWebFarmTask { get; set; }
    }
}