SELECT ClassID, ClassName 
	FROM CMS_Class 
	WHERE ClassName IN (SELECT * FROM @TableValueParameter)

DROP TYPE [dbo].[KI_ClassNames] 