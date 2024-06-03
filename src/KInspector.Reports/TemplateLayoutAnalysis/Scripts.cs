namespace KInspector.Reports.TemplateLayoutAnalysis
{
    public static class Scripts
    {
        public static string BaseDirectory => $"{nameof(TemplateLayoutAnalysis)}/Scripts";

        public static string GetIdenticalLayouts => $"{BaseDirectory}/{nameof(GetIdenticalLayouts)}.sql";
    }
}