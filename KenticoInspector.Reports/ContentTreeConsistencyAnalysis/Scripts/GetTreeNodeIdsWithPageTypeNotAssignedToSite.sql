﻿SELECT NodeID
	FROM CMS_Tree
	WHERE NOT EXISTS (
		SELECT * 
			FROM  CMS_ClassSite
			WHERE ClassID = NodeClassID and SiteID = NodeSiteID)