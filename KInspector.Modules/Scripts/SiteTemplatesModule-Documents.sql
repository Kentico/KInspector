SELECT [SiteName], [NodeAliasPath], [DocumentCulture] 
FROM [View_CMS_Tree_Joined] 
WHERE [DocumentPageTemplateID] = @PageTemplateID 
ORDER BY SiteName, DocumentCulture