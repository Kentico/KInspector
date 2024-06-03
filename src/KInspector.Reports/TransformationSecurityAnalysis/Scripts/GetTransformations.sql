SELECT 
    TransformationName,
    TransformationCode,
    TransformationType,
    ClassName

    FROM CMS_Transformation T
    JOIN CMS_Class C
        ON T.TransformationClassID = C.ClassID