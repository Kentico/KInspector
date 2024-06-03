namespace KInspector.Reports.SecuritySettingsAnalysis
{
    public static class Scripts
    {
        public static string BaseDirectory => $"{nameof(SecuritySettingsAnalysis)}/Scripts";

        public static string GetSecurityCmsSettings => $"{BaseDirectory}/{nameof(GetSecurityCmsSettings)}.sql";

        public static string GetCmsSettingsCategories => $"{BaseDirectory}/{nameof(GetCmsSettingsCategories)}.sql";
    }
}