SELECT Child.NodeID
	FROM CMS_Tree as Child
	LEFT JOIN CMS_Tree as Parent on Child.NodeParentID = Parent.NodeID
	WHERE (Child.NodeLevel - 1) != Parent.NodeLevel