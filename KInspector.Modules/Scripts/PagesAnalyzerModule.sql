DECLARE @columnVariable nvarchar(32);
DECLARE @sqlString nvarchar(MAX);

SELECT @columnVariable =
	CASE
		WHEN EXISTS(
			SELECT 1 FROM Sys.columns c
			WHERE c.object_id = OBJECT_ID('CMS_Document')
			AND c.name = 'DocumentMenuRedirectToFirstChild')
			THEN
				'DocumentMenuRedirectToFirstChild'
			ELSE
				'''DOESNOTEXIST'''
			END

SET @sqlString = N'SELECT COALESCE(NULLIF(DocumentUrlPath, ''''), NodeAliasPath) AS AliasPath, 
	' + @columnVariable + ' AS Redirected,
	ClassName, pt.PageTemplateCodeName,  '''' AS ''Response'', 
	'''' AS ''HTML Size [KB]'', '''' AS ''ViewState Size [KB]'', 
	'''' AS ''Response Time [ms]'', '''' AS ''Link count'', 
	'''' AS ''Response type'', '''' AS ''Favicon'', '''' AS ''Apple Touch Icon'', 
	'''' AS ''Apple Touch Icon Precomposed'',
	'''' AS ''Images without alt''
FROM View_CMS_Tree_Joined v 
JOIN CMS_PageTemplate pt ON v.DocumentPageTemplateID = pt.PageTemplateID 
WHERE NodeSiteID =' + @SiteId + ' AND Published = 1';

EXECUTE sp_executesql @sqlString