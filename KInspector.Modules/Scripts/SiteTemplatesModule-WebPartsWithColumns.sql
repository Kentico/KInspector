SELECT 
	WebPartName,
	CASE 
		WHEN WebPartProperties LIKE '%column="TopN"%' THEN 1
		WHEN WebPartProperties LIKE '%column="SelectTopN"%' THEN 1
		ELSE 0 
	END AS TopN,
	CASE 
		WHEN WebPartProperties like '%column="Columns"%' THEN 1
		ELSE 0
	END AS Columns
  FROM [CMS_WebPart] where WebPartProperties like '%column="Columns"%' OR WebPartProperties LIKE '%column="TopN"%' OR WebPartProperties LIKE '%column="SelectTopN"%'