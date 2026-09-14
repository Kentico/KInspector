SELECT ClassDisplayName, ClassName
  FROM CMS_Class
  WHERE
    ClassContentTypeType IN ('Website', 'Reusable') AND
    ClassID not in (SELECT DISTINCT ContentItemContentTypeID FROM CMS_ContentItem)