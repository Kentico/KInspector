SELECT
	(SELECT NodeSiteID FROM CMS_Tree WHERE NodeID = T.NodeParentID) AS [NodeSiteID], 
	(SELECT NodeAliasPath FROM CMS_Tree WHERE NodeID = T.NodeParentID) AS [NodeAliasPath], 
	COUNT(*) AS [NodeNumberOfChildren]
FROM 
	CMS_Tree T
WHERE NodeParentID > 0
GROUP BY NodeParentID
ORDER BY NodeSiteID, [NodeNumberOfChildren] DESC, NodeAliasPath

SELECT SiteID, SiteDisplayName FROM CMS_Site ORDER BY SiteID
