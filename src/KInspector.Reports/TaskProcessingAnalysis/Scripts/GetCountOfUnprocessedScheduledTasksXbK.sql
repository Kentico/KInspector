SELECT COUNT(*) FROM CMS_ScheduledTaskConfiguration
	WHERE ScheduledTaskConfigurationDeleteAfterLastRun = 1 
	AND ScheduledTaskConfigurationNextRunTime < DATEADD(hour, -24, GETDATE())