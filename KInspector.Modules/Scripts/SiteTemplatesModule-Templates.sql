SELECT PageTemplateID, PageTemplateDisplayName, PageTemplateCodeName, PageTemplateWebParts 
FROM [CMS_PageTemplate] 
WHERE PageTemplateID IN (
	SELECT DISTINCT DocumentPageTemplateID 
	FROM [View_CMS_Tree_Joined])