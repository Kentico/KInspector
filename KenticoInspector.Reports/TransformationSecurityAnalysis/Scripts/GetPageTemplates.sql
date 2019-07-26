SELECT 
    PageTemplateID,
    PageTemplateCodeName,
    PageTemplateDisplayName,
    PageTemplateWebParts
    
    FROM CMS_PageTemplate
    
    WHERE PageTemplateID IN @DocumentPageTemplateIDs