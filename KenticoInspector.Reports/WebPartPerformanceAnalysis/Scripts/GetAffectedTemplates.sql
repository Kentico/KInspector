SELECT PageTemplateID, PageTemplateCodeName, PageTemplateDisplayName, PageTemplateWebParts
	FROM CMS_PageTemplate
	WHERE PageTemplateWebParts LIKE '%<property name="columns"></property>%'