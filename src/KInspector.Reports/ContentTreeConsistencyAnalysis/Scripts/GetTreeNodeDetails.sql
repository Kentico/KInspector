SELECT NodeAliasPath, NodeID, NodeParentID, NodeSiteID, NodeLevel, NodeClassID, ClassName, ClassDisplayName
	FROM CMS_Tree
	JOIN CMS_Class on ClassID = NodeClassID
	WHERE NodeID in @IDs