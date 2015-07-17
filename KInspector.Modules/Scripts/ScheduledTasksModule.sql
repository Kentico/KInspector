-- DECLARE VARIABLES
DECLARE @taskName nvarchar(200)
DECLARE @taskDisplayName nvarchar(200)
DECLARE @taskExecutions int
DECLARE @taskInterval nvarchar(1000)
DECLARE @taskEnabled bit
DECLARE @taskRunInSeparateThread bit
DECLARE @taskUseExternalService bit
DECLARE @taskAllowExternalService bit
DECLARE @keyValue nvarchar(200)
DECLARE @keyName nvarchar(200)
DECLARE @keyName2 nvarchar(200)
DECLARE @siteID int
DECLARE @siteName nvarchar(100)
DECLARE @taskWhere nvarchar(200)
DECLARE @taskIntervalIndex int
DECLARE @taskIntervalIndexSecond int

--- MESSAGES ---
DECLARE @moduleNotUsed nvarchar(300)
	SET @moduleNotUsed =					N'	- DISABLE SCHEDULED TASK! - Module is NOT used.'
DECLARE @moduleNotPossibleToCheck nvarchar(300)
	SET @moduleNotPossibleToCheck =			N'	- NOT ABLE TO determine if module is used - disable if not used.'
DECLARE @moduleRunAsExternal nvarchar(300)	
	SET @moduleRunAsExternal =				N'	- RUN THE task as EXTERNAL'
DECLARE @moduleDontRunAsExternal nvarchar(300)	
	SET @moduleDontRunAsExternal =			N'  - DON NOT RUN THE task as EXTERNAL'
DECLARE @tastAllowExternalServiceIsNull nvarchar(300)	
	SET @tastAllowExternalServiceIsNull =	N'  - Allow EXTERNAL SERVICE IS NULL - PLEASE CHECK THE SETTINGS'

DECLARE wTempSite CURSOR LOCAL FAST_FORWARD FOR 
	SELECT [SiteID], [SiteName] FROM [CMS_Site]
	UNION ALL
	SELECT 0 AS SiteID, 'Global' as SiteName 

OPEN wTempSite
	FETCH NEXT FROM wTempSite INTO @siteID, @siteName
	WHILE @@FETCH_STATUS = 0
		BEGIN
			DECLARE wTemp CURSOR LOCAL FAST_FORWARD FOR
				SELECT [TaskName], [TaskDisplayName], [TaskExecutions], [TaskInterval], [TaskEnabled], [TaskRunInSeparateThread],[TaskUseExternalService], [TaskAllowExternalService]  FROM [CMS_ScheduledTask]
				WHERE (TaskSiteID = @siteID OR ISNULL(TaskSiteID, 0) = @siteID) AND TaskEnabled = 1 ORDER BY [TaskDisplayName]
				
			PRINT @siteName
			
			OPEN wTemp
			FETCH NEXT FROM wTemp INTO @taskName, @taskDisplayName, @taskExecutions, @taskInterval, @taskEnabled, @taskRunInSeparateThread, @taskUseExternalService, @taskAllowExternalService
			WHILE @@FETCH_STATUS = 0
				BEGIN
				
				SET @keyName = NULL
				SET @keyValue = NULL
				
				SET @taskIntervalIndex = CHARINDEX(';', @taskInterval)
				SET @taskIntervalIndexSecond = CHARINDEX(';', @taskInterval, @taskIntervalIndex + 1)
				
				PRINT ' - ' + @taskDisplayName + N' (' +  SUBSTRING (@taskInterval , @taskIntervalIndexSecond + 1 , 1) + '-' + SUBSTRING (@taskInterval , 0 , @taskIntervalIndex )  + N')'

				--- GLOBAL TASKS ONLY ---
				 
				 IF @siteName LIKE 'Global'
					 BEGIN
				
						---- Clean chat online users & Deleted Rooms Cleaner & Clean old initiated chat requests ----
						IF @taskName LIKE 'cleanchatonlineusers' OR @taskName LIKE 'DeletedRoomsCleaner' OR @taskName LIKE 'ChatOldInitiatedChatRequestsCleaner'
							BEGIN
								PRINT @moduleNotPossibleToCheck
								BEGIN
									IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
										BEGIN
											PRINT @moduleRunAsExternal
										END
									ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
										BEGIN
											PRINT @moduleDontRunAsExternal
										END
									ELSE IF @taskAllowExternalService IS NULL
										BEGIN
											PRINT @tastAllowExternalServiceIsNull
										END
								END
							END				
						---- Clean chat online users & Deleted Rooms Cleaner & Clean old initiated chat requests ----
						
						---- Clean old chat records ----
						IF @taskName LIKE 'ChatOldRecordsCleaner'
							BEGIN
								SET @keyName = 'CMSChatDaysNeededToDeleteRecods'
								IF (SELECT COUNT(KeyValue) FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND ISNUMERIC(KeyValue) = 1 AND CAST(KeyValue AS INT) > 0) = 0
									BEGIN
										PRINT @moduleNotUsed  
									END
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
							END				
						---- Clean old chat records ----
						
						---- Clean e-mail queue ----
						IF @taskName LIKE 'Email.QueueCleaner'
							BEGIN
								SET @keyName = 'CMSArchiveEmails'
								IF (SELECT COUNT(KeyValue) FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND ISNUMERIC(KeyValue) = 1 AND CAST(KeyValue AS INT) > 0) = 0
									BEGIN
										PRINT @moduleNotUsed  
									END
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
							END				
						---- Clean e-mail queue ----
						
						---- Delete inactive contacts ----
						IF @taskName LIKE 'DeleteInactiveContacts'
							BEGIN
								--  BOTH SETTINGS NEEDS TO BE 'TRUE'
								SET @keyName = 'CMSEnableOnlineMarketing'
								SET @keyName2 = 'CMSDeleteInactiveContacts'
										
								IF(SELECT MAX(SettingsNumber) FROM
								(
									SELECT COUNT(ISNULL(SiteID, '0')) AS SettingsNumber FROM CMS_SettingsKey WHERE (KeyName LIKE @keyName AND KeyValue LIKE 'True') OR (KeyName LIKE @keyName2 AND KeyValue LIKE 'True') GROUP BY SiteID
								) AS MaxValue) < 2
									BEGIN
										PRINT @moduleNotUsed
									END
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END		
							END				
						---- Delete inactive contacts ----
						
						---- Delete old file system cache files ----
						IF @taskName LIKE 'Cache.DeleteOldFileSystemCache'
							BEGIN
								SET @keyName = 'CMSFileSystemOutputCacheMinutes'
								
								BEGIN
									IF (SELECT COUNT(KeyValue) FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND ISNUMERIC(KeyValue) = 1 AND CAST(KeyValue AS INT) > 0) = 0
										BEGIN
											PRINT @moduleNotUsed  
										END
									ELSE
										BEGIN
											IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
												BEGIN
													PRINT @moduleRunAsExternal
												END
											ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
												BEGIN
													PRINT @moduleDontRunAsExternal
												END
											ELSE IF @taskAllowExternalService IS NULL
												BEGIN
													PRINT @tastAllowExternalServiceIsNull
												END
										END
									END	
								END			
						---- Delete old file system cache files ----
						
						---- Delete old temporary upload files ----
						IF @taskName LIKE 'Content.DeleteOldTemporaryUploadFiles'
							BEGIN
								IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 1)
									BEGIN
										PRINT @moduleDontRunAsExternal
									END
								ELSE IF @taskAllowExternalService IS NULL
									BEGIN
										PRINT @tastAllowExternalServiceIsNull
									END
							END	
						---- Delete old temporary upload files ----
						
						---- E-product reminder ----
						IF @taskName LIKE 'EProductReminder'
							BEGIN
								SET @keyName = 'CMSStoreSendEmailsFrom'
								
								IF (SELECT COUNT(KeyValue) FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND ISNULL(KeyValue, '') != '') = 0
									BEGIN
										PRINT @moduleNotUsed  
									END
								ELSE IF (SELECT COUNT(EmailTemplateID) FROM CMS_EmailTemplate WHERE EmailTemplateName LIKE 'Ecommerce.EproductExpirationNotification') = 0
									BEGIN
										PRINT @moduleNotUsed
									END	
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
							END
						---- E-product reminder ----
						
						---- Execute search tasks ----
						IF @taskName LIKE 'Search.TaskExecutor'
							BEGIN
								SET @keyName = 'CMSSearchIndexingEnabled'
								
								IF (SELECT TOP 1 KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE @keyName ) LIKE 'false'
									BEGIN
										PRINT @moduleNotUsed  
									END
								ELSE IF (SELECT COUNT(IndexID) FROM CMS_SearchIndex) = 0
									BEGIN
										PRINT @moduleNotUsed
									END	
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
							END
						---- Execute search tasks ----
						
						---- Membership reminder ----
						IF @taskName LIKE 'MembershipReminder'
							BEGIN
								SET @keyName = 'CMSAdminEmailAddress'
								
								IF (SELECT COUNT(KeyValue) FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND ISNULL(KeyValue, '') != '') = 0
									BEGIN
										PRINT @moduleNotUsed  
									END
								ELSE IF (SELECT COUNT(EmailTemplateID) FROM CMS_EmailTemplate WHERE EmailTemplateName LIKE 'Membership.ExpirationNotification') = 0
									BEGIN
										PRINT @moduleNotUsed
									END	
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
							END
						---- Membership reminder ----
						
						---- Process analytics log ----
						IF @taskName LIKE 'Analytics.LogProcessing'
							BEGIN
								SET @keyName = 'CMSAnalyticsEnabled'
								
								IF (SELECT COUNT(KeyValue) FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND ISNULL(KeyValue, 'false') != 'false') = 0
									BEGIN
										PRINT @moduleNotUsed  
									END
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
							END
						---- Process analytics log ----
						
						---- Process external integration tasks ----
						IF @taskName LIKE 'Integration.ProcessExternalTasks'
							BEGIN
								
								IF ( SELECT COUNT(KeyValue) FROM CMS_SettingsKey WHERE 
										( KeyName LIKE 'CMSIntegrationEnabled' AND ISNULL(KeyValue, 'false') = 'true' )
										OR
										( KeyName LIKE 'CMSIntegrationLogExternal' AND ISNULL(KeyValue, 'false') = 'true' )
										OR
										( KeyName LIKE 'CMSIntegrationProcessExternal' AND ISNULL(KeyValue, 'false') = 'true' )
									) < 3
									BEGIN
										PRINT @moduleNotUsed  
									END
								ELSE IF (SELECT COUNT(ConnectorID) FROM Integration_Connector WHERE ConnectorEnabled = 1) = 0
									BEGIN
										PRINT @moduleNotUsed
									END	
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
							END
						---- Process external integration tasks ----
						
						---- Process forum thread views ----
						IF @taskName LIKE 'ForumThreadViewsProcessor'
							BEGIN
								IF (SELECT COUNT(ForumID) FROM Forums_Forum) = 0
									BEGIN
										PRINT @moduleNotUsed
									END	
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
							END
						---- Process forum thread views ----
						
						---- Project task - overdue task reminder ----
						IF @taskName LIKE 'ProjectTaskOverdueTaskReminder'
							BEGIN
								IF (SELECT COUNT(ProjectTaskID) FROM PM_ProjectTask) = 0
									BEGIN
										PRINT @moduleNotUsed
									END	
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
							END
						---- Project task - overdue task reminder ----
						
						---- Recalculate time zone ----
						IF @taskName LIKE 'TimeZone.Recalculate'
							BEGIN
								SET @keyName = 'CMSTimeZonesEnable'
								
								IF (SELECT TOP 1 KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE @keyName) LIKE 'false'
									BEGIN
										PRINT @moduleNotUsed  
									END
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
							END
						---- Recalculate time zone ----
			
						---- Remove expired sessions ----
						IF @taskName LIKE 'SessionsRemoveExpiredSessions'
							BEGIN
								SET @keyName = 'CMSUseSessionManagement'
								
								IF (SELECT TOP 1 KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE @keyName) LIKE 'false'
									BEGIN
										PRINT @moduleNotUsed  
									END
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
							END
						---- Remove expired sessions ----
			
						---- Report subscription sender ----
						IF @taskName LIKE 'Report_subscription_sender'
							BEGIN
								IF (SELECT COUNT(ReportSubscriptionID) FROM Reporting_ReportSubscription WHERE ReportSubscriptionEnabled = 1) = 0
									BEGIN
										PRINT @moduleNotUsed
									END	
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
							END
						---- Report subscription sender ----
			
						---- Send queued e-mails ----
						IF @taskName LIKE 'Email.QueueSender'
							BEGIN
								--  BOTH SETTINGS NEEDS TO BE 'TRUE'
								SET @keyName = 'CMSEmailsEnabled'
								SET @keyName2 = 'CMSEmailQueueEnabled'
									
								IF(SELECT MAX(SettingsNumber) FROM
								(
									SELECT COUNT(ISNULL(SiteID, '0')) AS SettingsNumber FROM CMS_SettingsKey WHERE (KeyName LIKE @keyName AND KeyValue LIKE 'True') OR (KeyName LIKE @keyName2 AND KeyValue LIKE 'True') GROUP BY SiteID
								) AS MaxValue) < 2
									BEGIN
										PRINT @moduleNotUsed
									END
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END		
							END			
						---- Send queued e-mails ----
						
						---- Send queued newsletters ----
						IF @taskName LIKE 'NewsletterSender'
							BEGIN
								IF (SELECT COUNT(NewsletterID) FROM Newsletter_Newsletter) = 0
									BEGIN
										PRINT @moduleNotUsed
									END	
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
						END
						---- Send queued newsletters ----
						
						---- Synchronize web farm changes ----
						IF @taskName LIKE 'SynchronizeWebFarmChanges'
							BEGIN
								IF ( SELECT COUNT(KeyID) FROM CMS_SettingsKey WHERE 
										( KeyName LIKE 'CMSWebFarmEnabled' AND ISNULL(KeyValue, 'false') LIKE 'False' )
										OR
										( KeyName LIKE 'CMSWebFarmUpdateWithinRequest' AND ISNULL(KeyValue, 'false') LIKE 'True' )
									) > 0
									BEGIN
										PRINT @moduleNotUsed  
									END
								ELSE IF (SELECT COUNT(ServerID) FROM CMS_WebFarmServer WHERE ServerEnabled = 1) = 0
									BEGIN
										PRINT @moduleNotUsed
									END	
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
							END
						---- Synchronize web farm changes ----
						
						---- Update database session ----
						IF @taskName LIKE 'SessionsUpdateDatabaseSession'
							BEGIN
								SET @keyName = 'CMSUseSessionManagement'
								SET @keyName2 = 'CMSSessionUseDBRepository'
								
								IF (SELECT COUNT(KeyID) FROM CMS_SettingsKey WHERE (KeyName LIKE @keyName AND KeyValue LIKE 'false')
									OR
									( KeyName LIKE @keyName2 AND KeyValue LIKE 'false')
									) > 0
									BEGIN
										PRINT @moduleNotUsed  
									END
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
							END
						---- Update database session ----
						
					END	
				
				--- GLOBAL TASKS ONLY ---
								
				--- SITE SPECIFIC TASKS ONLY ---
				 
				 IF @siteName NOT LIKE 'Global'
					 BEGIN
					 
						---- SALES FORCE ----
						IF @taskName LIKE 'SalesForce.Replicate'
							BEGIN
								SET @keyName = 'CMSSalesForceLeadReplicationEnabled'
								SET @keyValue = (SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND  SiteID = @siteID)
								IF @keyValue IS NULL
								BEGIN
									SET @keyValue = (SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND  SiteID IS NULL)
								END
								IF @keyValue IS NULL OR @keyValue LIKE 'False'
									BEGIN
										PRINT @moduleNotUsed
									END
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
							END				
						---- SALES FORCE ----
					 
					 	---- Newsletter - CheckBounces ----
						IF @taskName LIKE 'Newsletter.CheckBounces'
							BEGIN
								SET @keyName = 'CMSMonitorBouncedEmails'
								SET @keyValue = (SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND  SiteID = @siteID)
								IF @keyValue IS NULL
								BEGIN
									SET @keyValue = (SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND  SiteID IS NULL)
								END
								IF @keyValue IS NULL OR @keyValue LIKE 'False'
									BEGIN
										PRINT @moduleNotUsed  
									END
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
							END				
						---- Newsletter - CheckBounces ----
						
						---- Content synchronization ----
						IF @taskName LIKE 'Staging.Synchronization'
							BEGIN
								IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
									BEGIN
										PRINT @moduleRunAsExternal
									END
								ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
									BEGIN
										PRINT @moduleDontRunAsExternal
									END
								ELSE IF @taskAllowExternalService IS NULL
									BEGIN
										PRINT @tastAllowExternalServiceIsNull
									END
							END				
						---- Content synchronization ----
						
						---- Content publishing ----
						IF @taskName LIKE 'Content.Publish'
							BEGIN
								IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
									BEGIN
										PRINT @moduleRunAsExternal
									END
								ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
									BEGIN
										PRINT @moduleDontRunAsExternal
									END
								ELSE IF @taskAllowExternalService IS NULL
									BEGIN
										PRINT @tastAllowExternalServiceIsNull
									END
							END			
						---- Content publishing ----
												
						---- Delete old shopping carts ----
						IF @taskName LIKE 'Ecommerce.DeleteOldShoppingCarts'
							BEGIN
								IF (SELECT COUNT(SKUID) FROM COM_SKU) = 0 
									BEGIN
										PRINT @moduleNotUsed
									END
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
							END				
						---- Delete old shopping carts ----
						
						---- Translation Services ----
						IF @taskName LIKE 'TranslationsRetrieval'
							BEGIN
								SET @keyName = 'CMSEnableTranslations'
								SET @keyValue = (SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND  SiteID = @siteID)
								IF @keyValue IS NULL
								BEGIN
									SET @keyValue = (SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND SiteID IS NULL)
								END
								IF @keyValue IS NULL OR @keyValue LIKE 'False'
									BEGIN
										PRINT @moduleNotUsed  
									END
								ELSE
									BEGIN
										IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
											BEGIN
												PRINT @moduleRunAsExternal
											END
										ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
											BEGIN
												PRINT @moduleDontRunAsExternal
											END
										ELSE IF @taskAllowExternalService IS NULL
											BEGIN
												PRINT @tastAllowExternalServiceIsNull
											END
									END
							END				
						---- Translation Services ----
						
						---- Delete Non Activated Users ---- 
						IF @taskName LIKE 'UsersDeleteNonActivatedUser'
							BEGIN
								SET @keyName = 'CMSRegistrationEmailConfirmation'

								SET @keyValue = (SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND  SiteID = @siteID)
								IF @keyValue IS NULL
								BEGIN
									SET @keyValue = (SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND SiteID IS NULL)
								END
								IF @keyValue IS NULL OR @keyValue LIKE 'False'
									BEGIN
										PRINT @moduleNotUsed  
									END
								ELSE
									BEGIN
										SET @keyName = 'CMSDeleteNonActivatedUserAfter'
										SET @keyValue = (SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND  SiteID = @siteID)
											IF @keyValue IS NULL
												BEGIN
													SET @keyValue = (SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND SiteID IS NULL)
												END
											IF (SELECT CASE WHEN ISNUMERIC(@keyValue) = 1 THEN CAST(@keyValue AS INT) ELSE 0 END) = 0
												BEGIN
													PRINT @moduleNotUsed  
												END
											ELSE
												BEGIN
													IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
														BEGIN
															PRINT @moduleRunAsExternal
														END
													ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
														BEGIN
															PRINT @moduleDontRunAsExternal
														END
													ELSE IF @taskAllowExternalService IS NULL
														BEGIN
															PRINT @tastAllowExternalServiceIsNull
														END
												END
									END	
							END				
						---- Delete Non Activated Users ----
						
					 END
				 
				--- SITE SPECIFIC TASKS ONLY ---

				--- BOTH SITE AND GLOBAL TASKS ---
				
					---- OM - ACTIVITY ----
					IF @taskName LIKE 'OnlineMarketing.ActivitiesLog'
						BEGIN
							SET @keyName = 'CMSEnableOnlineMarketing'
							SET @keyValue = (SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND  SiteID = @siteID)
							IF @keyValue IS NULL
							BEGIN
								SET @keyValue = (SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND  SiteID IS NULL)
							END
							IF @keyValue IS NULL OR @keyValue LIKE 'False'
								BEGIN
									PRINT @moduleNotUsed  
								END
							ELSE
								BEGIN
									IF (@taskAllowExternalService = 1 AND @taskUseExternalService = 0)
										BEGIN
											PRINT @moduleRunAsExternal
										END
									ELSE IF (@taskAllowExternalService = 0 AND @taskUseExternalService = 1)
										BEGIN
											PRINT @moduleDontRunAsExternal
										END
									ELSE IF @taskAllowExternalService IS NULL
										BEGIN
											PRINT @tastAllowExternalServiceIsNull
										END
								END
						END				
					---- OM - ACTIVITY ----

				--- BOTH SITE AND GLOBAL TASKS ---

					FETCH NEXT FROM wTemp INTO @taskName, @taskDisplayName, @taskExecutions, @taskInterval, @taskEnabled, @taskRunInSeparateThread, @taskUseExternalService, @taskAllowExternalService
				END
			CLOSE wTemp
			DEALLOCATE wTemp
			
			FETCH NEXT FROM wTempSite INTO @siteID, @siteName
		END
CLOSE wTempSite
DEALLOCATE wTempSite
