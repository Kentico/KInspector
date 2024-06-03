SELECT NodeID
	FROM CMS_Tree
	WHERE NodeLevel != LEN(NodeAliasPath) - LEN(REPLACE(NodeAliasPath,'/','')) -- NodeLevel = number of '/' in it
	and NodeLevel != 0 and NodeParentID != 0 -- Root nodes