SELECT NodeID, NodeAliasPath, NodeSiteID, NodeLinkedNodeID 
	FROM CMS_Tree
	WHERE	NodeID not in (SELECT DocumentNodeID from CMS_Document)
	and NodeLinkedNodeID is null