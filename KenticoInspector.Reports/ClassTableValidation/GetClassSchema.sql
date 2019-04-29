-- Classes with their xml schema
SELECT ClassTableName, ClassXmlSchema FROM CMS_Class
where ClassTableName IS NOT NULL
and ClassTableName != ''
	ORDER BY ClassTableName
