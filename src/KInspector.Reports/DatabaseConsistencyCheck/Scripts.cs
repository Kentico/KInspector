namespace KInspector.Reports.DatabaseConsistencyCheck
{
    public static class Scripts
    {
        public static string BaseDirectory => $"{nameof(DatabaseConsistencyCheck)}/Scripts";

        public static string GetCheckDbResults => $"{BaseDirectory}/{nameof(GetCheckDbResults)}.sql";
    }
}