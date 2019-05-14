namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis
{
    public static class Scripts
    {
        public const string BaseDirectory = "ContentTreeConsistencyAnalysis/Scripts/";

        public const string GetDocumentIdsWithMissingTreeNode = BaseDirectory + "GetDocumentIdsWithMissingTreeNode.sql";
        public const string GetPageTypeAssignmentResults = BaseDirectory + "GetPageTypeAssignmentResults.sql";
        public const string GetTreeNodeDetails = BaseDirectory + "GetTreeNodeDetails.sql";
        public const string GetTreeNodeIdsWithBadParentNodeId = BaseDirectory + "GetTreeNodeIdsWithBadParentNodeId.sql";
        public const string GetTreeNodeIdsWithBadParentSiteId = BaseDirectory + "GetTreeNodeIdsWithBadParentSiteId.sql";
        public const string GetTreeNodeIdsWithLevelMismatchByAliasPathTest = BaseDirectory + "GetTreeNodeIdsWithLevelMismatchByAliasPathTest.sql";
        public const string GetTreeNodeIdsWithLevelMismatchByNodeLevelTest = BaseDirectory + "GetTreeNodeIdsWithLevelMismatchByNodeLevelTest.sql";
        public const string GetTreeNodeIdsWithMissingDocument = BaseDirectory + "GetTreeNodeIdsWithMissingDocument.sql";
        public const string GetTreeNodeIdsWithDuplicatedAliasPath = BaseDirectory + "GetTreeNodeIdsWithDuplicatedAliasPath.sql";
    }
}
