﻿using KInspector.Core.Models;

namespace KInspector.Reports.TaskProcessingAnalysis.Models
{
    public class Terms
    {
        public Term? CountIntegrationBusTask { get; set; }

        public Term? CountScheduledTask { get; set; }

        public Term? CountSearchTask { get; set; }

        public Term? CountStagingTask { get; set; }

        public Term? CountUnprocessedTask { get; set; }

        public Term? CountWebFarmTask { get; set; }
    }
}