-- Tables that do not have Class definition. If OM tables missing, check DB separation is on.
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
WHERE
	TABLE_TYPE = 'BASE TABLE' AND
	TABLE_NAME NOT IN (SELECT ClassTableName FROM CMS_Class WHERE ClassTableName IS NOT NULL)
ORDER BY
	TABLE_NAME