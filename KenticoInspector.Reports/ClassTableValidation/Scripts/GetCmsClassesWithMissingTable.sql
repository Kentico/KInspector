﻿SELECT 
	ClassID, 
	ClassDisplayName, 
	ClassName, 
	ClassTableName 
	
FROM 
	CMS_Class
	
WHERE
	ClassIsDocumentType = 0 
	AND ClassTableName NOT IN (
		SELECT 
			TABLE_NAME 
			
		FROM 
			INFORMATION_SCHEMA.TABLES
	)

ORDER BY
	ClassDisplayName, 
	ClassName, 
	ClassTableName
