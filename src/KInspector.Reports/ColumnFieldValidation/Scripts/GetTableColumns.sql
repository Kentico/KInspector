SELECT 
	Table_Name,
	Column_Name,
	Data_Type

FROM
	INFORMATION_SCHEMA.COLUMNS

WHERE
    Table_Name IN @classTableNames