-- CLEAR OPTION TO OUTPUT MODIFIED TABLES
SET NOCOUNT ON

-- DECLARE VARIABLES
DECLARE @siteID int
DECLARE @keyValue nvarchar(max)
DECLARE @keyName NVARCHAR(100) 
DECLARE @SiteName NVARCHAR(100)
DECLARE @NodeAliasPath NVARCHAR(200) 
DECLARE @NodeID int
DECLARE @NodeACLID int
DECLARE @DocumentID int

-- General settings related to CMS.FILE doc.type
PRINT 'SETTINGS'

DECLARE wTempSite CURSOR LOCAL FAST_FORWARD FOR 
	SELECT [SiteID], [SiteName] FROM [CMS_Site]
	UNION ALL
	SELECT 0 AS SiteID, 'Global' as SiteName 

OPEN wTempSite
	FETCH NEXT FROM wTempSite INTO @siteID, @siteName
	WHILE @@FETCH_STATUS = 0
		BEGIN
		
		PRINT ' - WEB SITE: ' + @siteName
		
		DECLARE wTempSettings CURSOR LOCAL FAST_FORWARD FOR
				SELECT [KeyName], [KeyValue] FROM [CMS_SettingsKey]
				WHERE (SiteID = @siteID OR ISNULL(SiteID, 0) = @siteID) AND 
				KeyName IN (
					'CMSAnalyticsEnabled',
					'CMSEnableOnlineMarketing'
				) ORDER BY [KeyName]		
		
			OPEN wTempSettings
				FETCH NEXT FROM wTempSettings INTO @keyName, @keyValue
					WHILE @@FETCH_STATUS = 0
					BEGIN
						IF @keyValue IS NULL
							BEGIN
								SET @keyValue = (SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE @keyName AND SiteID IS NULL)
							END
						
						IF @keyValue IS NULL
							BEGIN
								SET @keyValue = '--EMPTY--'
							END
						
						PRINT '    ' + @keyName + '(' + @keyValue + ')'
						
						FETCH NEXT FROM wTempSettings INTO @keyName, @keyValue
					END
			CLOSE wTempSettings
			DEALLOCATE wTempSettings
		
			FETCH NEXT FROM wTempSite INTO @siteID, @siteName
		END
CLOSE wTempSite
DEALLOCATE wTempSite

-- SEPARATOR
PRINT ''

-- USE OF 'robots.txt' or 'favicon.ico' files
PRINT 'SPECIAL USE: robots.txt AND/OR favicon.ico files'

DECLARE wTempSite CURSOR LOCAL FAST_FORWARD FOR SELECT [SiteID], [SiteName] FROM [CMS_Site]
OPEN wTempSite
	FETCH NEXT FROM wTempSite INTO @siteID, @siteName
	WHILE @@FETCH_STATUS = 0
		BEGIN
			PRINT ' - WEB SITE: ' + @siteName

			DECLARE wTempTree CURSOR LOCAL FAST_FORWARD FOR 
			
				SELECT @NodeAliasPath FROM View_CMS_Tree_Joined WHERE (NodeAliasPath LIKE '/robots' OR NodeAliasPath LIKE '/favicon' OR DocumentUrlPath LIKE '/robots' OR  DocumentUrlPath LIKE '/favicon') 
					AND
				NodeSiteID = @siteID
			
			OPEN wTempTree
				FETCH NEXT FROM wTempTree INTO @NodeAliasPath
					WHILE @@FETCH_STATUS = 0
						BEGIN
						   PRINT '   ' + @NodeAliasPath
						   FETCH NEXT FROM wTempTree INTO @NodeAliasPath
						END
				CLOSE wTempTree
				DEALLOCATE wTempTree
			FETCH NEXT FROM wTempSite INTO @siteID, @siteName
		END
CLOSE wTempSite
DEALLOCATE wTempSite

-- SEPARATOR
PRINT ''

-- USE OF 'document library' module
PRINT 'DOCUMENT LIBRARY'

DECLARE wTempSite CURSOR LOCAL FAST_FORWARD FOR SELECT [SiteID], [SiteName] FROM [CMS_Site]
OPEN wTempSite
	FETCH NEXT FROM wTempSite INTO @siteID, @siteName
	WHILE @@FETCH_STATUS = 0
		BEGIN
			PRINT ' - WEB SITE: ' + @siteName

			DECLARE wTempTree CURSOR LOCAL FAST_FORWARD FOR 
			
				SELECT NodeAliasPath FROM View_CMS_Tree_Joined WHERE 
				(
				NodeTemplateID IN (SELECT PageTemplateID FROM CMS_PageTemplate WHERE PageTemplateWebParts LIKE '%DocumentLibrary%' OR PageTemplateWebParts LIKE '%GroupDocumentLibrary%')
					OR
				DocumentPageTemplateID IN (SELECT PageTemplateID FROM CMS_PageTemplate WHERE PageTemplateWebParts LIKE '%DocumentLibrary%' OR PageTemplateWebParts LIKE '%GroupDocumentLibrary%')
				)
					AND
				NodeSiteID = @siteID
			
			OPEN wTempTree
				FETCH NEXT FROM wTempTree INTO @NodeAliasPath
					WHILE @@FETCH_STATUS = 0
						BEGIN
						   PRINT '   ' + @NodeAliasPath
						   FETCH NEXT FROM wTempTree INTO @NodeAliasPath
						END
				CLOSE wTempTree
				DEALLOCATE wTempTree
			FETCH NEXT FROM wTempSite INTO @siteID, @siteName
		END
CLOSE wTempSite
DEALLOCATE wTempSite

-- SEPARATOR
PRINT ''

-- LIST OF Documents on which 'search in attachments' is enabled
PRINT 'Full-Text search in Attachments'

DECLARE wTempSite CURSOR LOCAL FAST_FORWARD FOR SELECT [SiteID], [SiteName] FROM [CMS_Site]
OPEN wTempSite
	FETCH NEXT FROM wTempSite INTO @siteID, @siteName
	WHILE @@FETCH_STATUS = 0
		BEGIN
			PRINT ' - WEB SITE: ' + @siteName

			DECLARE wTempTree CURSOR LOCAL FAST_FORWARD FOR 
			
				SELECT NodeAliasPath FROM View_CMS_Tree_Joined WHERE 
				(
				NodeTemplateID IN (SELECT PageTemplateID FROM CMS_PageTemplate WHERE PageTemplateWebParts LIKE '%<property name="searchinattachments">True</property>%')
					OR
				DocumentPageTemplateID IN (SELECT PageTemplateID FROM CMS_PageTemplate WHERE PageTemplateWebParts LIKE '%<property name="searchinattachments">True</property>%')
				)
					AND
				NodeSiteID = @siteID
			
			OPEN wTempTree
				FETCH NEXT FROM wTempTree INTO @NodeAliasPath
					WHILE @@FETCH_STATUS = 0
						BEGIN
						   PRINT '   ' + @NodeAliasPath
						   FETCH NEXT FROM wTempTree INTO @NodeAliasPath
						END
				CLOSE wTempTree
				DEALLOCATE wTempTree
			FETCH NEXT FROM wTempSite INTO @siteID, @siteName
		END
CLOSE wTempSite
DEALLOCATE wTempSite

-- SEPARATOR
PRINT ''

-- LIST OF CMS.FILE documents
PRINT 'CMS.FILE LIST'

DECLARE @aliasCount INT
DECLARE @categoriesCount INT
DECLARE @tagsCount INT
DECLARE @attachmentsCount INT
DECLARE @slashIndex INT
DECLARE @OMActivityEnabled bit
DECLARE @relatedDocsCount INT
DECLARE @linkedDocuments INT
DECLARE @permissionCount INT
DECLARE @ScopeClassID INT

-- Get ClassID of 'CMS.FILE' page type
SET @ScopeClassID = (SELECT ClassID FROM CMS_Class WHERE ClassName LIKE 'cms.file')

-- Iterate all sites
DECLARE wTempSite CURSOR LOCAL FAST_FORWARD FOR SELECT [SiteID], [SiteName] FROM [CMS_Site]
OPEN wTempSite
	FETCH NEXT FROM wTempSite INTO @siteID, @siteName
	WHILE @@FETCH_STATUS = 0
		BEGIN
			PRINT ' - WEB SITE: ' + @siteName

			-- Iterate all 'CMS.FILE' pages within site
			DECLARE wTempTree CURSOR LOCAL FAST_FORWARD FOR SELECT [NodeID], [DocumentID], [NodeAliasPath], NodeACLID FROM View_CONTENT_File_Joined WHERE NodeSiteID = @siteID
			OPEN wTempTree
				FETCH NEXT FROM wTempTree INTO @NodeID, @DocumentID, @NodeAliasPath, @NodeACLID
					WHILE @@FETCH_STATUS = 0
						BEGIN
							PRINT '   ' + @NodeAliasPath
							
							-- Create table that will hold the NodeAliasPaths of parent elements
							CREATE TABLE #tb_UpperTree (NodeAliasPath nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS)
							
							DECLARE @tempAlias nvarchar(200)
							DECLARE @indexAlias INT
							DECLARE @previousNodeAlias nvarchar(200)
							DECLARE @previousNodeAliasLength INT

							-- Root is always present among the parent elements
							INSERT INTO #tb_UpperTree (NodeAliasPath) VALUES ('/')

							WHILE 1 = 1
								BEGIN
									SET @previousNodeAlias = (SELECT TOP 1 NodeAliasPath FROM #tb_UpperTree ORDER BY NodeAliasPath DESC)
									SET @previousNodeAliasLength = LEN(@previousNodeAlias)
									
									SET @tempAlias = (SELECT SUBSTRING(@NodeAliasPath, @previousNodeAliasLength + 1, LEN(@NodeAliasPath) - @previousNodeAliasLength))
									
									SET @indexAlias = (SELECT PATINDEX('%/%' , @tempAlias))
									IF @indexAlias = 1
										BEGIN
											SET @indexAlias = (SELECT PATINDEX('%/%' , (SELECT SUBSTRING(@tempAlias, 2, LEN(@tempAlias) -1 )))) + 1
											IF @indexAlias = 1
												BEGIN
													SET @indexAlias = LEN(@tempAlias) +1
												END
										END
									ELSE IF @indexAlias = 0 -- When 'CMS.FILE' is directly under root (unusual)
										SET @indexAlias = LEN(@tempAlias) +1

									SET @tempAlias = @previousNodeAlias + (SELECT SUBSTRING(@tempAlias, 0, @indexAlias))
									
									INSERT INTO #tb_UpperTree (NodeAliasPath) VALUES (@tempAlias)
									
									if @tempAlias = @NodeAliasPath
										BEGIN
											BREAK
										END
								END

							SET @OMActivityEnabled = (SELECT TOP 1 DocumentLogVisitActivity FROM View_CMS_Tree_Joined WHERE NodeSiteID = 1 and NodeAliasPath IN (SELECT NodeAliasPath FROM #tb_UpperTree) AND DocumentLogVisitActivity IS NOT NULL ORDER BY NodeLevel DESC)
							IF @OMActivityEnabled = 1
								BEGIN
									PRINT '    - OM Activity enabled'
								END

							-- DOCUMENT ALIAS
							SET @aliasCount = (SELECT COUNT(*) FROM CMS_DocumentAlias WHERE AliasNodeID = @NodeID)
							IF @aliasCount > 0
							 BEGIN
								PRINT '    - ' + CAST(@aliasCount AS NVARCHAR) + ' x document alias'
							 END
							
							-- TAGS
							SET @tagsCount = (SELECT COUNT(*) FROM CMS_DocumentTag WHERE DocumentID = @DocumentID)
							IF @tagsCount > 0
							 BEGIN
								PRINT '    - ' + CAST(@tagsCount AS NVARCHAR) + ' x document tag'
							 END
							
							-- CATEGORIES
							SET @tagsCount = (SELECT COUNT(*) FROM CMS_DocumentCategory WHERE DocumentID = @DocumentID)
							IF @tagsCount > 0
							 BEGIN
								PRINT '    - ' + CAST(@tagsCount AS NVARCHAR) + ' x document category'
							 END
							
							-- UNSORTED ATTACHMENTS
							SET @attachmentsCount = (SELECT COUNT(*) FROM CMS_Attachment WHERE AttachmentDocumentID = @DocumentID AND AttachmentIsUnsorted = 1)
							IF @attachmentsCount > 0
							 BEGIN
								PRINT '    - ' + CAST(@attachmentsCount AS NVARCHAR) + ' x unsorted attachment'
							 END
							
							SET @relatedDocsCount = (SELECT COUNT(*) FROM CMS_Relationship WHERE LeftNodeID = @NodeID OR RightNodeID = @NodeID)
							
							IF @relatedDocsCount > 0
							 BEGIN
								PRINT '    - ' + CAST(@relatedDocsCount AS NVARCHAR) + ' x related document'
							 END
								
							SET @linkedDocuments = (SELECT COUNT(*) FROM View_CMS_Tree_Joined WHERE NodeLinkedNodeID = @NodeID)	
							IF 	@linkedDocuments > 0
							 BEGIN
								PRINT '    - ' + CAST(@linkedDocuments AS NVARCHAR) + ' x linked document'
							 END
																					
							-- WORKFLOW
							DECLARE @scoupCount INT

							SET @scoupCount = (SELECT COUNT(*) 
							FROM CMS_WorkflowScope 
							WHERE ScopeSiteID = @siteID
							AND (ScopeClassID = @ScopeClassID OR ScopeClassID IS NULL) 
							AND ((ScopeStartingPath = '/' OR @NodeAliasPath LIKE ScopeStartingPath + '/%' OR @NodeAliasPath LIKE ScopeStartingPath) AND 
												-- Special case for root				
								 (ScopeStartingPath <> '/%' OR @NodeAliasPath <> '/') AND
											 -- Do not select scope with excluded children unless it is the node itself
								(ScopeExcludeChildren <> 1 OR ScopeExcludeChildren IS NULL OR ScopeStartingPath = @NodeAliasPath)))
							
							IF @scoupCount > 0
							 BEGIN
								PRINT '    - ' + CAST(@scoupCount AS NVARCHAR) + ' x workflow scope applied'
							 END
							
							-- SECURITY
							DECLARE @ACLID int
							DECLARE @ACLInheritedACLs nvarchar(200)

							SET @ACLID = (SELECT  ACLID FROM CMS_Tree INNER JOIN CMS_ACL ON CMS_ACL.ACLID = CMS_Tree.NodeACLID WHERE CMS_Tree.NodeID = @NodeID)
							SET @ACLInheritedACLs = (SELECT  ACLInheritedACLs FROM CMS_Tree INNER JOIN CMS_ACL ON CMS_ACL.ACLID = CMS_Tree.NodeACLID WHERE CMS_Tree.NodeID = @NodeID)
				
							SET @permissionCount = (SELECT COUNT(Operator) FROM View_CMS_ACLItem_ItemsAndOperators WHERE ACLID IN (SELECT r.value('.','VARCHAR(MAX)') as ACLID FROM (SELECT CONVERT(XML, N'<root><r>' + REPLACE(@ACLInheritedACLs , ',', '</r><r>') + '</r></root>') as valxml) x CROSS APPLY x.valxml.nodes('//root/r') AS RECORDS(r)
							UNION SELECT @ACLID))
							IF @permissionCount > 0
							 BEGIN
								PRINT '    - ' + CAST(@permissionCount AS NVARCHAR) + ' x permission'
							 END
							
							DROP TABLE #tb_UpperTree
							
						   FETCH NEXT FROM wTempTree INTO @NodeID, @DocumentID, @NodeAliasPath, @NodeACLID
						END
				CLOSE wTempTree
				DEALLOCATE wTempTree
			FETCH NEXT FROM wTempSite INTO @siteID, @siteName
		END
CLOSE wTempSite
DEALLOCATE wTempSite

-- TODO: how to check if
---- "USER CONTRIBUTION" MODULE IS USED
---- 'CMS.FILE' is in some menu or sitemap
