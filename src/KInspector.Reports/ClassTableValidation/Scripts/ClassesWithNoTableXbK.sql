-- Classes with missing database tables.
SELECT ClassDisplayName, ClassName, ClassTableName FROM CMS_Class
	WHERE
		ClassContentTypeType IS NULL AND
		ClassTableName NOT IN (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES)
	ORDER BY
		ClassDisplayName, ClassName, ClassTableName
