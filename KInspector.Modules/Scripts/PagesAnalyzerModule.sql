SELECT COALESCE(NULLIF(DocumentUrlPath, ''), NodeAliasPath) AS AliasPath, 
	ClassName, pt.PageTemplateCodeName,  '' AS 'Response', 
	'' AS 'HTML Size [KB]', '' AS 'ViewState Size [KB]', 
	'' AS 'Response Time [ms]', '' AS 'Link count', 
	'' AS 'Response type', '' AS 'Favicon', '' AS 'Apple Touch Icon', 
	'' AS 'Apple Touch Icon Precomposed' 
FROM View_CMS_Tree_Joined v 
JOIN CMS_PageTemplate pt ON v.DocumentPageTemplateID = pt.PageTemplateID 
WHERE NodeSiteID = @SiteId AND Published = 1