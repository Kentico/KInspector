SELECT DISTINCT S.SiteName, T.ClassName, T.NodeSiteID, T.NodeClassID
FROM VIEW_CMS_Tree_Joined AS T
LEFT JOIN CMS_ClassSite AS CS ON SiteID = NodeSiteID AND ClassID = NodeClassID
LEFT JOIN CMS_Site AS S ON S.SiteID = T.NodeSiteID 
WHERE CS.SiteID IS NULL OR CS.ClassID IS NULL