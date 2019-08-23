SELECT 
    ClassID,
	ClassName,
	ClassDisplayName,
    ClassTableName, 
    ClassXmlSchema
    
FROM 
    CMS_Class

WHERE
    ClassXmlSchema != ''