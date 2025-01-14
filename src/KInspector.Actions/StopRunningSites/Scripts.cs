namespace KInspector.Actions.StopRunningSites
{
    public static class Scripts
    {
        public static string BaseDirectory => $"{nameof(StopRunningSites)}/Scripts";

        public static string GetSiteSummary => $"{BaseDirectory}/{nameof(GetSiteSummary)}.sql";

        public static string StopSite => $"{BaseDirectory}/{nameof(StopSite)}.sql";
    }
}
