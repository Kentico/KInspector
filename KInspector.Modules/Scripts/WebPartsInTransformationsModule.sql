-- Select WebParts in transformations
SELECT N'WebParts in transformations' AS N'#KInspectorNextTableName'
SELECT 
	s.SiteName, 
	v.NodeAliasPath, 
	pt.PageTemplateCodeName, 
	c.ClassName + '.' + t.TransformationName AS FullTransformatioName, 
	t.TransformationCode 
FROM CMS_Transformation AS t
INNER JOIN CMS_Class AS c
ON c.ClassID = t.TransformationClassID
INNER JOIN CMS_PageTemplate AS pt ON pt.PageTemplateWebParts LIKE '%' + c.ClassName + '.' + t.TransformationName + '%'
INNER JOIN View_CMS_Tree_Joined AS v ON pt.PageTemplateID = v.DocumentPageTemplateID
INNER JOIN CMS_Site as s ON s.SiteID = v.NodeSiteID
WHERE
t.TransformationCode LIKE '%CMSRepeater%'
OR
t.TransformationCode LIKE '%CMSBreadCrumbs%'
OR
t.TransformationCode LIKE '%CMSListMenu%'
OR
t.TransformationCode LIKE '%CMSDataList%'
