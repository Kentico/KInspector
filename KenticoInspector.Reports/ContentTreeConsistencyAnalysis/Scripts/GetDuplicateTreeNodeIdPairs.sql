SELECT 
	Original.NodeID,
	Duplicate.NodeID as 'DuplicateNodeID'
	
	FROM CMS_Tree as Original
	LEFT JOIN CMS_Tree as Duplicate on Original.NodeAliasPath = Duplicate.NodeAliasPath 

	WHERE
		Original.NodeID != Duplicate.NodeID
		AND Original.NodeSiteID = Duplicate.NodeSiteID

	ORDER BY Original.NodeID, Original.NodeAliasPath