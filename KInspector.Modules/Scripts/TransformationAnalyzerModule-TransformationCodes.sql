SELECT TransformationID, TransformationName, TransformationCode 
FROM CMS_Transformation 
WHERE TransformationName IN (@ListOfNames)