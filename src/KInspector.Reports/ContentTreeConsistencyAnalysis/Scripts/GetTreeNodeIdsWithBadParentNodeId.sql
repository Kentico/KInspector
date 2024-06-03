SELECT NodeID
	FROM CMS_Tree
	WHERE	NodeParentID NOT IN (Select NodeID from CMS_Tree)
			AND NodeParentID != 0