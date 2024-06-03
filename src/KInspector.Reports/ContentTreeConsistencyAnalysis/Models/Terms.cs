using KInspector.Core.Models;

namespace KInspector.Reports.ContentTreeConsistencyAnalysis.Models
{
    public class Terms
    {
        public Term? DocumentNodesWithNoTreeNode { get; set; }
        public Term? NameFound { get; set; }
        public Term? NoContentTreeConsistencyIssuesFound { get; set; }
        public Term? TreeNodesWithABadParentNode { get; set; }
        public Term? TreeNodesWithABadParentSite { get; set; }
        public Term? TreeNodesWithDuplicatedAliasPath { get; set; }
        public Term? TreeNodesWithLevelInconsistencyAliasPath { get; set; }
        public Term? TreeNodesWithLevelInconsistencyParent { get; set; }
        public Term? TreeNodesWithNoDocumentNode { get; set; }
        public Term? TreeNodesWithPageTypeNotAssignedToSite { get; set; }
        public Term? WorkflowInconsistencies { get; set; }
    }
}