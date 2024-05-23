namespace KInspector.Actions.DisableStagingServers
{
    public static class Scripts
    {
        public static string BaseDirectory => $"{nameof(DisableStagingServers)}/Scripts";

        public static string GetStagingServerSummary => $"{BaseDirectory}/{nameof(GetStagingServerSummary)}.sql";

        public static string DisableServer => $"{BaseDirectory}/{nameof(DisableServer)}.sql";
    }
}
