-- Select WebParts in transformations
SELECT N'WebParts in transformations' AS N'#KInspectorNextTableName'
SELECT v.SiteName, v.NodeAliasPath, pt.PageTemplateCodeName, c.ClassName + '.' + t.TransformationName AS FullTransformatioName, t.TransformationCode FROM CMS_Transformation AS t
INNER JOIN CMS_Class AS c
ON c.ClassID = t.TransformationClassID
INNER JOIN CMS_PageTemplate AS pt
ON pt.PageTemplateWebParts LIKE '%' + c.ClassName + '.' + t.TransformationName + '%'
INNER JOIN View_CMS_Tree_Joined AS v
ON pt.PageTemplateID = v.DocumentPageTemplateID
WHERE
t.TransformationCode LIKE '%CMSRepeater%'
OR
t.TransformationCode LIKE '%CMSBreadCrumbs%'
OR
t.TransformationCode LIKE '%CMSListMenu%'
OR
t.TransformationCode LIKE '%CMSDataList%'

-- Select WebParts in page templates
SELECT N'WebParts in page templates' AS N'#KInspectorNextTableName'
SELECT v.SiteName, pt.PageTemplateCodeName, pt.PageTemplateLayout, Count(v.DocumentID) AS UsedbyNPages, pt.PageTemplateShowAsMasterTemplate, pt.PageTemplateIsPortal FROM CMS_PageTemplate AS pt
INNER JOIN View_CMS_Tree_Joined AS v
ON pt.PageTemplateID = v.DocumentPageTemplateID
WHERE
pt.PageTemplateLayout LIKE '%CMSRepeater%'
OR
pt.PageTemplateLayout LIKE '%CMSBreadCrumbs%'
OR
pt.PageTemplateLayout LIKE '%CMSListMenu%'
OR
pt.PageTemplateLayout LIKE '%CMSDataList%'
GROUP BY pt.PageTemplateCodeName, pt.PageTemplateLayout, v.SiteName, pt.PageTemplateShowAsMasterTemplate, pt.PageTemplateIsPortal
