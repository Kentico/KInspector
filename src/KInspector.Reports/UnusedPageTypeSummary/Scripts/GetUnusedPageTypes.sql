SELECT ClassDisplayName, ClassName
  FROM CMS_Class
  WHERE
    ClassIsDocumentType = 1 AND
    ClassID not in (SELECT DISTINCT NodeClassID FROM View_CMS_Tree_Joined)