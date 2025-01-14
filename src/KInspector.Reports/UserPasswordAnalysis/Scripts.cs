namespace KInspector.Reports.UserPasswordAnalysis
{
    public static class Scripts
    {
        public static string BaseDirectory => $"{nameof(UserPasswordAnalysis)}/Scripts";

        public static string GetEnabledAndNotExternalUsers => $"{BaseDirectory}/{nameof(GetEnabledAndNotExternalUsers)}.sql";
    }
}