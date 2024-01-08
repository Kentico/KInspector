SELECT count(*) FROM [CMS_SearchTask]
	WHERE [SearchTaskCreated] < DATEADD(hour, -24, GETDATE())