
/****** Script for getting the list of all sites  ******/

select [SiteID]
      ,[SiteName]
from CMS_Site

/****** Script for getting the settings that decide where the files are stored in the filesystem  ******/

select [CategoryName]
      ,[KeyName]
	  ,[KeyValue]
	  ,[SiteID]
from CMS_SettingsKey
inner join CMS_SettingsCategory on CMS_SettingsKey.KeyCategoryID = CMS_SettingsCategory.CategoryID
where 
KeyName in ('CMSMediaLibrariesFolder', 'CMSUseMediaLibrariesSiteFolder', 'CMSBizFormFilesFolder', 'CMSUseBizFormsSiteFolder') 
and CategoryName in ('CMS.MediaLibraries.Storage', 'CMS.Files.Storage')
order by CategoryName, KeyOrder

/****** Script for Media Library Files  ******/

SELECT [FileName]
      ,[FileSiteID]
      ,[SiteName]
      ,[LibraryFolder]
      ,[FilePath]
	  ,[FileTitle]
	  ,[FileDescription]
  FROM [dbo].[Media_File]
  inner join [dbo].[CMS_Site] on [dbo].[Media_File].[FileSiteID] = [dbo].[CMS_Site].[SiteID]
  inner join [dbo].[Media_Library] on [dbo].[Media_File].[FileLibraryID] = [dbo].[Media_Library].[LibraryID]


/* Script to get BizForm attachment values */


declare @t table( siteID varchar(50), tablename varchar(50), columnname varchar(50))
declare @sql varchar(max)
set @sql = ''

/* Use XML to parse the Form defintion, find any Attachment-type records (Text using UploadControl) */

insert into @t
select CMS_Form.FormSiteID, ClassTableName, T.N.value('@column', 'varchar(50)') as name
from CMS_Class
inner join CMS_Form on CMS_Form.FormClassID = CMS_Class.ClassID
cross apply (select cast(ClassFormDefinition as xml)) as X(y)
cross apply X.y.nodes('/form/field[@columntype="text"][settings/controlname="UploadControl"]') T(N)
 where 
ClassIsForm = 1

/* Build a query for each of these, getting all of the attachment items within each table */

select @sql = @sql + 'Select [' + columnname + '] as AttachmentGUID, ' + siteID + ' as SiteID, ''' + tablename + ''' as TableName From ' + tablename + ' where [' + columnname + '] is not null union ' from @t


--remove the trailing 'union'
Select @sql = CASE WHEN  len(@sql) - 6 >= 0 THEN substring(@sql, 1, len(@sql) - 6) ELSE 'Select top 0 newid() as AttachmentGUID, 0 as SiteID, '''' as TableName' END

exec (@sql)