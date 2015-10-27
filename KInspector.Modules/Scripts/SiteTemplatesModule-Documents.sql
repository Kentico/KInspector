SELECT S.SiteName, NodeAliasPath, DocumentCulture 
FROM View_CMS_Tree_Joined AS V
INNER JOIN CMS_Site AS S on S.SiteID = V.NodeSiteID
WHERE DocumentPageTemplateID = @PageTemplateID 
ORDER BY S.SiteName, DocumentCulture