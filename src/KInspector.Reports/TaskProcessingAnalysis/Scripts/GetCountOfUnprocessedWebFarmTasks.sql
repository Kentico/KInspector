SELECT count(*) FROM [CMS_WebFarmTask]
	WHERE [TaskCreated] < DATEADD(hour, -24, GETDATE())