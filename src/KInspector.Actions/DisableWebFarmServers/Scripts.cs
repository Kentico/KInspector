namespace KInspector.Actions.DisableWebFarmServers
{
    public static class Scripts
    {
        public static string BaseDirectory => $"{nameof(DisableWebFarmServers)}/Scripts";

        public static string GetWebFarmServerSummary => $"{BaseDirectory}/{nameof(GetWebFarmServerSummary)}.sql";

        public static string DisableServer => $"{BaseDirectory}/{nameof(DisableServer)}.sql";
    }
}
