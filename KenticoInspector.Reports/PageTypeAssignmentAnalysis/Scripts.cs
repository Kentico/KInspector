namespace KenticoInspector.Reports.PageTypeAssignmentAnalysis
{
    public class Scripts
    {
        public static string BaseDirectory = $"{nameof(PageTypeAssignmentAnalysis)}/Scripts";

        public static string GetPageTypesNotAssignedToSite = $"{BaseDirectory}/{nameof(GetPageTypesNotAssignedToSite)}.sql";
    }
}