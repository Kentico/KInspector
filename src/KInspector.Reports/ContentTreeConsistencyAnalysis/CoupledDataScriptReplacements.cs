namespace KInspector.Reports.ContentTreeConsistencyAnalysis
{
    public class CoupledDataScriptReplacements
    {
        public Dictionary<string, string> Dictionary { get; set; }

        public CoupledDataScriptReplacements(string tableName, string idColumnName)
        {
            Dictionary = new Dictionary<string, string>
            {
                { "TableName", tableName },
                { "IdColumnName", idColumnName }
            };
        }
    }
}