SELECT PageTemplateDisplayName,PageTemplateCodeName, PageTemplateType, PageTemplateDescription 
  FROM CMS_PageTemplate
  WHERE PageTemplateID not in (SELECT DISTINCT NodeTemplateID FROM View_CMS_Tree_Joined WHERE NodeTemplateID is not NULL)
	AND PageTemplateType not in ('dashboard','ui')
  ORDER BY PageTemplateDisplayName