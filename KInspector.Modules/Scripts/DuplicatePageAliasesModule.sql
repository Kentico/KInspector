SELECT 
		AliasSiteID,
		AliasURLPath,
		AffectedNodeIDs,
		OriginalNodeID
FROM (
	SELECT
		AliasSiteID,
		AliasURLPath,
		STUFF((
			SELECT CAST(AliasNodeID AS VARCHAR(20)) + ' '
			FROM (
					SELECT AliasSiteID, AliasURLPath, AliasNodeID  FROM CMS_DocumentAlias  
					UNION ALL SELECT NodeSiteID, NodeAliasPath, NodeID FROM CMS_Tree
			) AN
			WHERE (AliasURLPath = C.AliasURLPath AND AliasSiteID = C.AliasSiteID)
			FOR XML PATH(''),TYPE).value('(./text())[1]','VARCHAR(MAX)'),1,0,'') [AffectedNodeIDs],
		CASE 
			WHEN (SELECT COUNT(*) FROM CMS_Tree WHERE NodeAliasPath = AliasURLPath AND NodeSiteID = AliasSiteID) > 0 THEN (SELECT TOP 1 NodeID FROM CMS_Tree WHERE NodeAliasPath = AliasURLPath AND NodeSiteID = AliasSiteID) 
			ELSE NULL
		END AS OriginalNodeID
	FROM CMS_DocumentAlias C 
	GROUP BY AliasURLPath, AliasSiteID
) DuplicateAliases
-- This where condition excludes single affected node IDs
WHERE AffectedNodeIDs LIKE '% % '
ORDER BY AliasSiteID, AliasURLPath

SELECT SiteID, SiteDisplayName FROM CMS_Site ORDER BY SiteID
