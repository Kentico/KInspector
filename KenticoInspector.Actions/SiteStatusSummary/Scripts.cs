namespace KenticoInspector.Actions.SiteStatusSummary
{
    public static class Scripts
    {
        public static string BaseDirectory => $"{nameof(SiteStatusSummary)}/Scripts";

        public static string GetSiteSummary => $"{BaseDirectory}/{nameof(GetSiteSummary)}.sql";

        public static string StopSite => $"{BaseDirectory}/{nameof(StopSite)}.sql";
    }
}
