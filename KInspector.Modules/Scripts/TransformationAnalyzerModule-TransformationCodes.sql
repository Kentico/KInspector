IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name ='KI_TransformationNames')
    CREATE TYPE dbo.KI_TransformationNames AS TABLE(
        TransformationName VARCHAR(MAX)
    )


SELECT TransformationID, TransformationName, TransformationCode 
	FROM CMS_Transformation 
	WHERE TransformationName IN (SELECT * FROM @ListOfNames)

DROP TYPE [dbo].[KI_TransformationNames] 