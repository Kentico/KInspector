SELECT count(*) FROM [Staging_Task]
	WHERE [TaskTime] < DATEADD(hour, -24, GETDATE())