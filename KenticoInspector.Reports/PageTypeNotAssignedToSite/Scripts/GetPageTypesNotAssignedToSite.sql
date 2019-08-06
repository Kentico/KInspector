SELECT DISTINCT NodeSiteID, NodeClassID, ClassName, ClassDisplayName
	FROM View_CMS_Tree_Joined
	LEFT JOIN CMS_ClassSite on 
		ClassID = NodeClassID AND
		SiteID = NodeSiteID
	WHERE SiteID is null