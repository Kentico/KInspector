namespace KenticoInspector.Actions.GlobalAdminSummary
{
    public static class Scripts
    {
        public static string BaseDirectory = $"{nameof(GlobalAdminSummary)}/Scripts";

        public static string GetAdministrators => $"{BaseDirectory}/{nameof(GetAdministrators)}.sql";

        public static string ResetAndEnableUser => $"{BaseDirectory}/{nameof(ResetAndEnableUser)}.sql";
    }
}
