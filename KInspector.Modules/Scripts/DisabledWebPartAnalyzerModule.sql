SELECT PageTemplateDisplayName, PageTemplateCodeName 
FROM CMS_PageTemplate 
WHERE PageTemplateWebParts LIKE '%<property name="visible">False</property>%'