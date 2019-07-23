namespace KenticoInspector.Reports.DatabaseTableSizeAnalysis
{
    public static class Scripts
    {
        public static string BaseDirectory = $"{nameof(DatabaseTableSizeAnalysis)}/Scripts";

        public static string GetTop25LargestTables = $"{BaseDirectory}/{nameof(GetTop25LargestTables)}.sql";
    }
}