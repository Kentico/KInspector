using System.Collections.Generic;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis
{
    public class CoupledDataScriptReplacements
    {
        public Dictionary<string, string> Dictionary { get; set; }

        public CoupledDataScriptReplacements(string tableName, string idColumnName)
        {
            Dictionary = new Dictionary<string, string>();
            Dictionary.Add("TableName", tableName);
            Dictionary.Add("IdColumnName", idColumnName);
        }
    }
}