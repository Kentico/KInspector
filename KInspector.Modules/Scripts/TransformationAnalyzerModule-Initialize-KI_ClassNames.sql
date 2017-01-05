IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name ='KI_ClassNames')
    CREATE TYPE dbo.KI_ClassNames AS TABLE(
        ClassName VARCHAR(MAX)
    )