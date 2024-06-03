SELECT count(*) FROM [Integration_Task]
	WHERE [TaskTime] < DATEADD(hour, -24, GETDATE())