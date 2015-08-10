SELECT distinct SiteName, ClassName, NodeSiteID, NodeClassID
FROM VIEW_CMS_Tree_Joined 
LEFT JOIN CMS_ClassSite ON SiteID = NodeSiteID AND ClassID = NodeClassID
WHERE SiteID IS NULL OR ClassID IS NULL
