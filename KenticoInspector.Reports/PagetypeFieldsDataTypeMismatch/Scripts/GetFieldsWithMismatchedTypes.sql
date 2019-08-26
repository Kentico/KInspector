-- Get field duplicate column names that have mismatched data types.
SELECT ClassName as PageType, COLUMN_NAME as FieldName, DATA_TYPE as DataType
	FROM INFORMATION_SCHEMA.COLUMNS
	LEFT JOIN CMS_Class ON ClassTableName = TABLE_NAME
	WHERE COLUMN_NAME IN (
		SELECT COLUMN_NAME
			FROM INFORMATION_SCHEMA.COLUMNS
		WHERE TABLE_NAME IN (
			SELECT ClassTableName 
				FROM cms_class
			WHERE ClassIsDocumentType = 1
		) 
		GROUP BY COLUMN_NAME
			HAVING COUNT(DISTINCT DATA_TYPE) > 1
	) 
	ORDER BY COLUMN_NAME
