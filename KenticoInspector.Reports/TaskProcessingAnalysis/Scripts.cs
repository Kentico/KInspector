namespace KenticoInspector.Reports.TaskProcessingAnalysis
{
    public static class Scripts
    {
        public static string BaseDirectory => $"{nameof(TaskProcessingAnalysis)}/Scripts";

        public static string GetCountOfUnprocessedIntegrationBusTasks => $"{BaseDirectory}/{nameof(GetCountOfUnprocessedIntegrationBusTasks)}.sql";

        public static string GetCountOfUnprocessedScheduledTasks => $"{BaseDirectory}/{nameof(GetCountOfUnprocessedScheduledTasks)}.sql";

        public static string GetCountOfUnprocessedSearchTasks => $"{BaseDirectory}/{nameof(GetCountOfUnprocessedSearchTasks)}.sql";

        public static string GetCountOfUnprocessedStagingTasks => $"{BaseDirectory}/{nameof(GetCountOfUnprocessedStagingTasks)}.sql";

        public static string GetCountOfUnprocessedWebFarmTasks => $"{BaseDirectory}/{nameof(GetCountOfUnprocessedWebFarmTasks)}.sql";
    }
}