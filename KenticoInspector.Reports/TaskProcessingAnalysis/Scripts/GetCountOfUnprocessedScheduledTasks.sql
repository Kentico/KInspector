SELECT count(*) FROM [CMS_ScheduledTask]
	WHERE [TaskDeleteAfterLastRun] = 1 
	AND [TaskNextRunTime] < DATEADD(hour, -24, GETDATE())