
SELECT NodeAliasPath, NodeID, NodeParentID, NodeSiteID, NodeLevel
	FROM CMS_Tree 
	WHERE NodeID in (@IDs)