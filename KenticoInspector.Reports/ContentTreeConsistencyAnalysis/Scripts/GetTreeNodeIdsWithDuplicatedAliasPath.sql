SELECT NodeID 
	FROM CMS_Tree
	WHERE NodeAliasPath in (
		SELECT NodeAliasPath
			FROM CMS_Tree
			GROUP BY NodeAliasPath
			HAVING COUNT(*) > 1)