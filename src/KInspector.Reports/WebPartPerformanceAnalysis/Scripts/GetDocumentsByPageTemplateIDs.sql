SELECT DocumentName, DocumentPageTemplateID, DocumentWebParts, NodeAliasPath, NodeSiteID
	FROM View_CMS_Tree_Joined
	WHERE DocumentPageTemplateID in @IDs

