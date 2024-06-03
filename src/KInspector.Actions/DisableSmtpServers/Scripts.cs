namespace KInspector.Actions.DisableSmtpServers
{
    public static class Scripts
    {
        public static string BaseDirectory => $"{nameof(DisableSmtpServers)}/Scripts";

        public static string GetSmtpFromSettingsKeys => $"{BaseDirectory}/{nameof(GetSmtpFromSettingsKeys)}.sql";

        public static string GetSmtpFromSmtpServers => $"{BaseDirectory}/{nameof(GetSmtpFromSmtpServers)}.sql";

        public static string DisableSiteSmtpServer => $"{BaseDirectory}/{nameof(DisableSiteSmtpServer)}.sql";

        public static string DisableSmtpServer => $"{BaseDirectory}/{nameof(DisableSmtpServer)}.sql";
    }
}
