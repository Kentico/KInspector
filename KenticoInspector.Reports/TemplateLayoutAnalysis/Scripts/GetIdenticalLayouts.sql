SELECT
  CodeNames = STUFF(
    (SELECT ', ' + PageTemplateCodeName
      FROM CMS_PageTemplate b 
        WHERE b.PageTemplateLayout = a.PageTemplateLayout
        FOR XML PATH('')), 1, 2, ''),
  a.PageTemplateLayout
  FROM CMS_PageTemplate a
  WHERE a.PageTemplateLayout is not NULL AND a.PageTemplateLayoutID is NULL
  GROUP BY a.PageTemplateLayout
  HAVING COUNT(*) > 1