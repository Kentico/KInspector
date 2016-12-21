SELECT TransformationID, TransformationName, TransformationCode 
	FROM CMS_Transformation 
	WHERE TransformationName IN (SELECT * FROM @ListOfNames)

DROP TYPE [dbo].[KI_TransformationNames] 