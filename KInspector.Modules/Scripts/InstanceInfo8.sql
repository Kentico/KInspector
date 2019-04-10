------------- FUNCTIONS
IF OBJECT_ID (N'KInspector_GetTableRowsCount', N'FN') IS NOT NULL
DROP FUNCTION KInspector_GetTableRowsCount;

EXEC('CREATE FUNCTION KInspector_GetTableRowsCount (@TableName nvarchar(MAX))
RETURNS int
WITH EXECUTE AS CALLER
AS
BEGIN
RETURN(SELECT TOP 1 p.rows FROM
sys.tables t
INNER JOIN
sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN
sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
WHERE
t.is_ms_shipped = 0 AND t.name = @TableName
);
END;
')


----------------------------------------------------
-------- INSTANCE PROPERTIES -----------------------
----------------------------------------------------


DECLARE @InstanceProperties TABLE (Title nvarchar(250), Value nvarchar(MAX))

-- Custom tables

DECLARE @CustomTables TABLE (ID int, Name nvarchar(MAX), TableName nvarchar(MAX))
INSERT INTO @CustomTables SELECT
ClassID
,ClassName
,ClassTableName
FROM [CMS_Class] WHERE ClassIsCustomTable = 1

INSERT INTO @InstanceProperties SELECT
'Custom Tables' AS [Title]
,COUNT(ID)
FROM @CustomTables

-- iterate throught custom tables
DECLARE @CustomTableID int
SELECT @CustomTableID = MIN(ID) FROM @CustomTables
WHILE @CustomTableID is not null
BEGIN
DECLARE @CTName nvarchar(MAX)
DECLARE @CTTable nvarchar(MAX)
SELECT @CTName = Name, @CTTable = TableName FROM @CustomTables WHERE ID = @CustomTableID
DECLARE @CTRows int;
EXEC @CTRows = KInspector_GetTableRowsCount @TableName = @CTTable
INSERT INTO @InstanceProperties VALUES (
CONCAT('-- ', @CTName)
, @CTRows)
SELECT @CustomTableID = MIN(ID) FROM @CustomTables where ID > @CustomTableID
END


-- Custom modules (classes, settings, UI elements)
IF OBJECT_ID('tempdb..#CustomModules') IS NOT NULL
BEGIN
DROP TABLE CustomModules
END

CREATE TABLE #CustomModules (ID int, Name nvarchar(MAX), DisplayName nvarchar(MAX))
INSERT INTO #CustomModules SELECT
ResourceID
,ResourceName
,ResourceDisplayName
FROM [CMS_Resource] WHERE ResourceIsInDevelopment IS NOT NULL

INSERT INTO @InstanceProperties SELECT
'Custom Modules' AS [Title]
,COUNT(ID)
FROM #CustomModules

-- iterate throught custom modules
DECLARE @CustomModuleID int
SELECT @CustomModuleID = MIN(ID) FROM #CustomModules
WHILE @CustomModuleID is not null
BEGIN
DECLARE @CMName nvarchar(MAX)
DECLARE @CMDisplayName nvarchar(MAX)
DECLARE @CMID int
SELECT @CMName = Name, @CMID = ID, @CMDisplayName = DisplayName FROM #CustomModules WHERE ID = @CustomModuleID

INSERT INTO @InstanceProperties VALUES (
CONCAT('-- ', @CMName), @CMDisplayName)
INSERT INTO @InstanceProperties SELECT
'---- Classes' AS [Title]
,COUNT(ClassID)
FROM [CMS_Class] WHERE ClassResourceID = @CMID
INSERT INTO @InstanceProperties SELECT
'---- Settings' AS [Title]
,COUNT(KeyID)
FROM [CMS_SettingsKey] JOIN [CMS_SettingsCategory] ON KeyCategoryID = CategoryID WHERE SiteID IS NULL AND CategoryResourceID = @CMID
INSERT INTO @InstanceProperties SELECT
'---- UI Elements' AS [Title]
,COUNT(ElementID)
FROM [CMS_UIElement] WHERE ElementResourceID = @CMID

SELECT @CustomModuleID = MIN(ID) FROM #CustomModules where ID > @CustomModuleID
END
DROP TABLE #CustomModules

--- Event log
INSERT INTO @InstanceProperties  SELECT
'Event log records (global)' AS [Title]
,COUNT(EventID)
FROM [CMS_EventLog] WHERE SiteID IS NULL;

-- Web farms
INSERT INTO @InstanceProperties  SELECT
'Web farms' AS [Title]
,KeyValue
FROM [CMS_SettingsKey] WHERE SiteID IS NULL AND KeyName = 'CMSWebFarmEnabled';


-- Metafiles
INSERT INTO @InstanceProperties  SELECT
'Metafiles (global)' AS [Title]
,COUNT(MetaFileID)
FROM [CMS_MetaFile] WHERE MetaFileSiteID IS NULL;

-- Users
INSERT INTO @InstanceProperties SELECT
'Users' AS [Title]
,COUNT(UserID)
FROM [CMS_User]
INSERT INTO @InstanceProperties SELECT
'-- External' AS [Title]
,COUNT(UserID)
FROM [CMS_User] WHERE UserIsExternal = 1
INSERT INTO @InstanceProperties SELECT
'-- Not enabled or hidden' AS [Title]
,COUNT(UserID)
FROM [CMS_User] WHERE UserEnabled = 0 OR UserIsHidden = 1
INSERT INTO @InstanceProperties SELECT
'-- Editors' AS [Title]
,COUNT(UserID)
FROM [CMS_User] WHERE UserIsEditor = 1
INSERT INTO @InstanceProperties SELECT
'-- Global Admins' AS [Title]
,COUNT(UserID)
FROM [CMS_User] WHERE UserIsGlobalAdministrator = 1



SELECT 'Global information' AS [#KInspectorNextTableName]
SELECT * FROM @InstanceProperties

----------------------------------------------------
-------- END INSTANCE PROPERTIES -------------------
----------------------------------------------------



DECLARE @SiteID int
SELECT @SiteID = MIN(SiteID) FROM [CMS_Site]
WHILE @SiteID is not null
BEGIN

----------------------------------------------------
-------- SITE PROPERTIES ---------------------------
----------------------------------------------------


DECLARE @SiteProperties TABLE (Title nvarchar(250), Value nvarchar(MAX))

DECLARE @SiteName nvarchar(100)

-- Site properties
SET @SiteName = (SELECT [SiteName] FROM [CMS_Site] WHERE SiteID = @SiteId)

INSERT INTO @SiteProperties  VALUES (
'Name'
,@SiteName)


INSERT INTO @SiteProperties  SELECT
'Status' AS [Title]
,[SiteStatus]
FROM [CMS_Site]WHERE SiteID = @SiteId;

INSERT INTO @SiteProperties  SELECT
'Culture' AS [Title]
,[CultureName]
FROM [CMS_SiteCulture] AS SC JOIN [CMS_Culture] AS C ON C.[CultureID] = SC.CultureID WHERE SC.SiteID = @SiteId;


-- Site domains and licenses
IF OBJECT_ID('tempdb..#SiteDomains') IS NOT NULL
BEGIN
DROP TABLE #SiteDomains
END

CREATE TABLE #SiteDomains (ID int, Name nvarchar(MAX), License nvarchar(MAX))
INSERT INTO #SiteDomains SELECT
-1,
[SiteDomainName],
[LicenseEdition]
FROM [CMS_Site] LEFT JOIN [CMS_LicenseKey] ON LicenseDomain = SiteDomainName  WHERE SiteID = @SiteId
INSERT INTO #SiteDomains SELECT
[SiteDomainAliasID],
[SiteDomainAliasName],
[LicenseEdition]
FROM [CMS_SiteDomainAlias] LEFT JOIN [CMS_LicenseKey] ON LicenseDomain = SiteDomainAliasName WHERE SiteID = @SiteId

INSERT INTO @SiteProperties SELECT
'Domains' AS [Title]
,COUNT(Name)
FROM #SiteDomains
INSERT INTO @SiteProperties SELECT
CONCAT('-- ', Name) AS [Title]
,CONCAT('License: ', ISNULL(License, 'NO LICENSE!'))
FROM #SiteDomains

DROP TABLE #SiteDomains

--- Event log
INSERT INTO @SiteProperties  SELECT
'Event log records' AS [Title]
,COUNT(EventID)
FROM [CMS_EventLog] WHERE SiteID = @SiteID;

-- Users
INSERT INTO @SiteProperties SELECT
'Users' AS [Title]
,COUNT(UserID)
FROM [CMS_UserSite] WHERE SiteID = @SiteId

-- Stylesheets
INSERT INTO @SiteProperties VALUES (
'Stylesheets'
,'' )
INSERT INTO @SiteProperties SELECT
CONCAT('-- ', StylesheetName) AS [Title],
''
FROM [CMS_CssStylesheet] S JOIN CMS_CssStylesheetSite SS ON S.StylesheetID = SS.StylesheetID  WHERE SiteID = @SiteId

-- Pages
INSERT INTO @SiteProperties SELECT
'Nodes' AS [Title]
,COUNT(NodeID)
FROM [CMS_Tree] WHERE NodeSiteID = @SiteId

INSERT INTO @SiteProperties SELECT
'Documents' AS [Title]
,COUNT(DocumentID)
FROM [CMS_Document] JOIN [CMS_Tree] ON [DocumentNodeID] = [NodeID]  WHERE NodeSiteID = @SiteId

INSERT INTO @SiteProperties SELECT
'Document aliases',
COUNT(AliasID)
FROM [CMS_DocumentAlias] WHERE AliasSiteID = @SiteID

INSERT INTO @SiteProperties SELECT
'Attachements',
COUNT(AttachmentID)
FROM [CMS_Attachment] WHERE AttachmentSiteID = @SiteID

INSERT INTO @SiteProperties  SELECT
'Metafiles' AS [Title]
,COUNT(MetaFileID)
FROM [CMS_MetaFile] WHERE MetaFileSiteID = @SiteID;


INSERT INTO @SiteProperties VALUES (
'Page types'
,'' )
INSERT INTO @SiteProperties SELECT
CONCAT('-- ', ClassName) AS [Title]
,COUNT(ClassID)
FROM [CMS_Class] JOIN [CMS_Tree] ON [NodeClassID] = [ClassID]  WHERE NodeSiteID = @SiteId GROUP BY ClassID, ClassName

-- Custom tables and forms
INSERT INTO @SiteProperties VALUES (
'Custom tables'
,'' )
INSERT INTO @SiteProperties SELECT
CONCAT('-- ', C.ClassName) AS [Title],
''
FROM [CMS_Class] AS C JOIN CMS_ClassSite AS CS ON CS.[ClassID] = C.[ClassID]  WHERE SiteID = @SiteId AND ClassIsCustomTable = 1

INSERT INTO @SiteProperties VALUES (
'Forms'
,'' )
INSERT INTO @SiteProperties SELECT
CONCAT('-- ', FormName) AS [Title],
''
FROM [CMS_Form]  WHERE FormSiteID = @SiteId

-- used custom modules
INSERT INTO @SiteProperties VALUES (
'Custom modules'
,'' )
INSERT INTO @SiteProperties SELECT
CONCAT('-- ', ResourceName) AS [Title],
''
FROM [CMS_Resource] R JOIN CMS_ResourceSite RS ON R.ResourceID = RS.ResourceID  WHERE SiteID = @SiteId AND ResourceIsInDevelopment IS NOT NULL


SELECT CONCAT('Site ', @SiteName) AS [#KInspectorNextTableName]
SELECT * FROM @SiteProperties

----------------------------------------------------
-------- END SITE PROPERTIES -----------------------
----------------------------------------------------

SELECT @SiteID = MIN(SiteID) FROM [CMS_Site] where SiteID > @SiteID

END

-- CLEAN UP

DROP FUNCTION KInspector_GetTableRowsCount;
