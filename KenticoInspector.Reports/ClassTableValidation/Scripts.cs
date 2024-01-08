namespace KenticoInspector.Reports.ClassTableValidation
{
    public static class Scripts
    {
        public static string BaseDirectory => $"{nameof(ClassTableValidation)}/Scripts";

        public static string ClassesWithNoTable => $"{BaseDirectory}/{nameof(ClassesWithNoTable)}.sql";

        public static string TablesWithNoClass => $"{BaseDirectory}/{nameof(TablesWithNoClass)}.sql";
    }
}