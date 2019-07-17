using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models
{
    public class Labels
    {
        public Label TreeNodesWithABadParentSite { get; set; }

        public Label TreeNodesWithABadParentNode { get; set; }

        public Label TreeNodesWithLevelInconsistencyAliasPath { get; set; }

        public Label TreeNodesWithLevelInconsistencyParent { get; set; }

        public Label TreeNodesWithNoDocumentNode { get; set; }

        public Label TreeNodesWithDuplicatedAliasPath { get; set; }

        public Label TreeNodesWithPageTypeNotAssignedToSite { get; set; }

        public Label DocumentNodesWithNoTreeNode { get; set; }

        public Label NameFound { get; set; }

        public Label NoContentTreeConsistencyIssuesFound { get; set; }

        public Label WorkflowInconsistencies { get; set; }
    }
}