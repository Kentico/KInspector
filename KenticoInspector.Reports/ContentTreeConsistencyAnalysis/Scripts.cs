namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis
{
    public static class Scripts
    {
        public static string BaseDirectory => $"{nameof(ContentTreeConsistencyAnalysis)}/Scripts";

        public static string GetCmsClassItems => $"{BaseDirectory}/{nameof(GetCmsClassItems)}.sql";

        public static string GetCmsDocumentCoupledDataItems => $"{BaseDirectory}/{nameof(GetCmsDocumentCoupledDataItems)}.sql";

        public static string GetDocumentIdsWithMissingTreeNode => $"{BaseDirectory}/{nameof(GetDocumentIdsWithMissingTreeNode)}.sql";

        public static string GetDocumentNodeDetails => $"{BaseDirectory}/{nameof(GetDocumentNodeDetails)}.sql";

        public static string GetTreeNodeDetails => $"{BaseDirectory}/{nameof(GetTreeNodeDetails)}.sql";

        public static string GetTreeNodeIdsWithBadParentNodeId => $"{BaseDirectory}/{nameof(GetTreeNodeIdsWithBadParentNodeId)}.sql";

        public static string GetTreeNodeIdsWithBadParentSiteId => $"{BaseDirectory}/{nameof(GetTreeNodeIdsWithBadParentSiteId)}.sql";

        public static string GetTreeNodeIdsWithDuplicatedAliasPath => $"{BaseDirectory}/{nameof(GetTreeNodeIdsWithDuplicatedAliasPath)}.sql";

        public static string GetTreeNodeIdsWithLevelMismatchByAliasPathTest => $"{BaseDirectory}/{nameof(GetTreeNodeIdsWithLevelMismatchByAliasPathTest)}.sql";

        public static string GetTreeNodeIdsWithLevelMismatchByNodeLevelTest => $"{BaseDirectory}/{nameof(GetTreeNodeIdsWithLevelMismatchByNodeLevelTest)}.sql";

        public static string GetTreeNodeIdsWithMissingDocument => $"{BaseDirectory}/{nameof(GetTreeNodeIdsWithMissingDocument)}.sql";

        public static string GetTreeNodeIdsWithPageTypeNotAssignedToSite => $"{BaseDirectory}/{nameof(GetTreeNodeIdsWithPageTypeNotAssignedToSite)}.sql";

        public static string GetLatestVersionHistoryIdForAllDocuments => $"{BaseDirectory}/{nameof(GetLatestVersionHistoryIdForAllDocuments)}.sql";

        public static string GetVersionHistoryDetails => $"{BaseDirectory}/{nameof(GetVersionHistoryDetails)}.sql";
    }
}