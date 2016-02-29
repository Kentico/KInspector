;WITH versions AS
(
   SELECT CMS_VersionHistory.DocumentID, DocumentName, ClassName, DocumentForeignKeyValue, WasPublishedFrom, NodeXML, NodeAliasPath, DocumentCulture,
         ROW_NUMBER() OVER (PARTITION BY CMS_VersionHistory.DocumentID ORDER BY VersionHistoryID desc) AS rn
   FROM CMS_VersionHistory join View_CMS_Tree_Joined on CMS_VersionHistory.DocumentID = View_CMS_Tree_Joined.DocumentID
)
SELECT *
FROM versions
WHERE rn = 1 and WasPublishedFrom IS NOT NULL