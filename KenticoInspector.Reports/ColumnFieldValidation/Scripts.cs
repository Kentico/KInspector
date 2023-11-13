namespace KenticoInspector.Reports.ColumnFieldValidation
{
    public static class Scripts
    {
        public static string BaseDirectory => $"{nameof(ColumnFieldValidation)}/Scripts";

        public static string GetCmsClasses => $"{BaseDirectory}/{nameof(GetCmsClasses)}.sql";

        public static string GetTableColumns => $"{BaseDirectory}/{nameof(GetTableColumns)}.sql";
    }
}