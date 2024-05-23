namespace KInspector.Reports.DebugConfigurationAnalysis
{
    public static class Scripts
    {
        public static string BaseDirectory => $"{nameof(DebugConfigurationAnalysis)}/Scripts";

        public static string GetDebugSettingsValues => $"{BaseDirectory}/{nameof(GetDebugSettingsValues)}.sql";
    }
}