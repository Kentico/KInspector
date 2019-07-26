namespace KenticoInspector.Reports.TransformationSecurityAnalysis
{
    public class Scripts
    {
        public static string BaseDirectory = $"{nameof(TransformationSecurityAnalysis)}/Scripts";
        public static string GetTransformations = $"{BaseDirectory}/{nameof(GetTransformations)}.sql";
        public static string GetPageTemplates = $"{BaseDirectory}/{nameof(GetPageTemplates)}.sql";
        public static string GetPages = $"{BaseDirectory}/{nameof(GetPages)}.sql";
    }
}