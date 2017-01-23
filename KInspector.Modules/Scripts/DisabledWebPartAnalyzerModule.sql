SELECT PageTemplateDisplayName, PageTemplateWebParts 
FROM CMS_PageTemplate 
WHERE PageTemplateWebParts LIKE '%<property name="visible">False</property>%'