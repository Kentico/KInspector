namespace KInspector.Reports.WebPartPerformanceAnalysis
{
    public static class Scripts
    {
        public static string BaseDirectory => $"{nameof(WebPartPerformanceAnalysis)}/Scripts/";

        public static string GetAffectedTemplates => $"{BaseDirectory}{nameof(GetAffectedTemplates)}.sql";

        public static string GetDocumentsByPageTemplateIds => $"{BaseDirectory}{nameof(GetDocumentsByPageTemplateIds)}.sql";
    }
}