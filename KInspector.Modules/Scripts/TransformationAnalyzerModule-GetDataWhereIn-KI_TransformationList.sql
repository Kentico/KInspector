SELECT ct.TransformationID, ct.TransformationClassID, ct.TransformationName, ct.TransformationCode 
  FROM CMS_Transformation ct 
    INNER JOIN @TableValueParameter tvp
			ON tvp.TransformationClassID = ct.TransformationClassID
				AND tvp.TransformationName = ct.TransformationName

DROP TYPE [dbo].[KI_TransformationList] 