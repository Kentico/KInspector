namespace KenticoInspector.Actions.StagingServerSummary
{
    public static class Scripts
    {
        public static string BaseDirectory => $"{nameof(StagingServerSummary)}/Scripts";

        public static string GetStagingServerSummary => $"{BaseDirectory}/{nameof(GetStagingServerSummary)}.sql";

        public static string DisableServer => $"{BaseDirectory}/{nameof(DisableServer)}.sql";
    }
}
