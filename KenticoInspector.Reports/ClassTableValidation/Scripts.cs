namespace KenticoInspector.Reports.ClassTableValidation
{
    public static class Scripts
    {
        public static string BaseDirectory = $"{nameof(ClassTableValidation)}/Scripts";

        public static string GetCmsClassesWithMissingTable = $"{BaseDirectory}/{nameof(GetCmsClassesWithMissingTable)}.sql";
        public static string GetTablesWithMissingClass = $"{BaseDirectory}/{nameof(GetTablesWithMissingClass)}.sql";
    }
}