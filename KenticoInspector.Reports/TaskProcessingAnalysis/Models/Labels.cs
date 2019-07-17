using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.TaskProcessingAnalysis.Models
{
    public class Labels
    {
        public Label CountIntegrationBusTask { get; set; }

        public Label CountScheduledTask { get; set; }

        public Label CountSearchTask { get; set; }

        public Label CountStagingTask { get; set; }

        public Label CountWebFarmTask { get; set; }

        public Label CountUnprocessedTask { get; set; }
    }
}