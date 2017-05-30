SELECT
  (SELECT count(*) FROM [CMS_ScheduledTask] WHERE [TaskDeleteAfterLastRun] = 1 AND [TaskNextRunTime] < DATEADD(hour, -24, GETDATE())) AS SystemTasks,
  (SELECT count(*) FROM [CMS_WebFarmTask] WHERE TaskCreated < DATEADD(hour, -24, GETDATE())) AS WebFarmTasks,
  (SELECT count(*) FROM [Integration_Task] WHERE TaskTime < DATEADD(hour, -24, GETDATE())) AS IntegrationTasks,
  (SELECT count(*) FROM [Staging_Task] WHERE TaskTime < DATEADD(hour, -24, GETDATE())) AS StagingTasks,
  (SELECT count(*) FROM [CMS_SearchTask] WHERE SearchTaskCreated < DATEADD(hour, -24, GETDATE())) AS SearchTasks