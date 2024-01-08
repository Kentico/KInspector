namespace KenticoInspector.Actions.ResetCmsUserLogin
{
    public static class Scripts
    {
        public static string BaseDirectory = $"{nameof(ResetCmsUserLogin)}/Scripts";

        public static string GetAdministrators => $"{BaseDirectory}/{nameof(GetAdministrators)}.sql";

        public static string ResetAndEnableUser => $"{BaseDirectory}/{nameof(ResetAndEnableUser)}.sql";
    }
}
