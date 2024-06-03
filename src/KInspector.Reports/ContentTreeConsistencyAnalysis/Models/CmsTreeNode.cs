namespace KInspector.Reports.ContentTreeConsistencyAnalysis.Models
{
    public class CmsTreeNode
    {
        public string? ClassDisplayName { get; set; }

        public string? ClassName { get; set; }

        public string? NodeAliasPath { get; set; }

        public int NodeClassID { get; set; }

        public int NodeID { get; set; }

        public int NodeLevel { get; set; }

        public string? NodeName { get; set; }

        public int? NodeParentID { get; set; }

        public int NodeSiteID { get; set; }
    }
}