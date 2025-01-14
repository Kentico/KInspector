namespace KInspector.Reports.ContentTreeConsistencyAnalysis.Models
{
    public class CmsDocumentNode
    {
        public int DocumentForeignKeyValue { get; set; }

        public int DocumentID { get; set; }

        public string? DocumentName { get; set; }

        public int DocumentNodeID { get; set; }
    }
}