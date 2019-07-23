namespace KenticoInspector.Reports.UnusedPageTypeSummary
{
    public static class Scripts
    {
        public static string BaseDirectory = $"{nameof(UnusedPageTypeSummary)}/Scripts";

        public static string GetUnusedPageTypes = $"{BaseDirectory}/{nameof(GetUnusedPageTypes)}.sql";
    }
}