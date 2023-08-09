namespace KenticoInspector.Actions.WebFarmServerSummary
{
    public static class Scripts
    {
        public static string BaseDirectory => $"{nameof(WebFarmServerSummary)}/Scripts";

        public static string GetWebFarmServerSummary => $"{BaseDirectory}/{nameof(GetWebFarmServerSummary)}.sql";

        public static string DisableServer => $"{BaseDirectory}/{nameof(DisableServer)}.sql";
    }
}
