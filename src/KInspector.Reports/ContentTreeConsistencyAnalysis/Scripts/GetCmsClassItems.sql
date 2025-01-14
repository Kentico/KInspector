SELECT 
	ClassDisplayName,
	CAST (ClassFormDefinition as XML) ClassFormDefinitionXml,
	ClassID,
	ClassName,
	ClassTableName
FROM CMS_Class
WHERE ClassID in @IDs