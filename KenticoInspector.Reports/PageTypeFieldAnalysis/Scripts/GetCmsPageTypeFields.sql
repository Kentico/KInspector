SELECT 
    ClassName as PageTypeCodeName,
    COLUMN_NAME as FieldName,
    DATA_TYPE as FieldDataType
FROM
    INFORMATION_SCHEMA.COLUMNS
INNER JOIN CMS_Class ON
    ClassTableName = TABLE_NAME
WHERE 
    ClassIsDocumentType = 1