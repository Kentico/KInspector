IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name ='KI_TransformationList')
    CREATE TYPE dbo.KI_TransformationList AS TABLE(
        TransformationName VARCHAR(MAX),
		TransformationClassID VARCHAR(MAX)
    )