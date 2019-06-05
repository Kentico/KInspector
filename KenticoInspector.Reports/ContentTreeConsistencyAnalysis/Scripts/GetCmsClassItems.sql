SELECT 
	ClassID,
	ClassName,
	ClassDisplayName,
	CAST (ClassFormDefinition as XML) ClassFormDefinitionXml
	FROM CMS_Class
	WHERE ClassID in @IDs