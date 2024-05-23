namespace KInspector.Reports.DatabaseTableSizeAnalysis
{
    public class DatabaseTableSizeResult
    {
        public string? TableName { get; set; }

        public int Rows { get; set; }

        public int SizeInMB { get; set; }

        public int BytesPerRow { get; set; }
    }
}