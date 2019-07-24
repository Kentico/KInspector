namespace KenticoInspector.Reports.WebPartPerformanceAnalysis
{
    public static class Scripts
    {
        public readonly static string BaseDirectory = $"{nameof(WebPartPerformanceAnalysis)}/Scripts/";
        public readonly static string GetAffectedTemplates = $"{BaseDirectory}{nameof(GetAffectedTemplates)}.sql";
        public readonly static string GetDocumentsByPageTemplateIds = $"{BaseDirectory}{nameof(GetDocumentsByPageTemplateIds)}.sql";
    }
}