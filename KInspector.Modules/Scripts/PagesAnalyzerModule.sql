SELECT COALESCE(NULLIF(DocumentUrlPath, ''), NodeAliasPath) AS AliasPath, 
	CASE
			WHEN DocumentMenuRedirectToFirstChild = '1' THEN 'YES'
			ELSE 'NO'
	END AS 'Redirected',
	ClassName, pt.PageTemplateCodeName,  '' AS 'Response', 
	'' AS 'HTML Size [KB]', '' AS 'ViewState Size [KB]', 
	'' AS 'Response Time [ms]', '' AS 'Link count', 
	'' AS 'Response type', '' AS 'Favicon', '' AS 'Apple Touch Icon', 
	'' AS 'Apple Touch Icon Precomposed',
	'' AS 'Images without alt'
FROM View_CMS_Tree_Joined v 
JOIN CMS_PageTemplate pt ON v.DocumentPageTemplateID = pt.PageTemplateID 
WHERE NodeSiteID = @SiteId AND Published = 1