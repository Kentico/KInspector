-- Add 'localhost' license key
DELETE FROM CMS_LicenseKey WHERE LicenseDomain = 'localhost'

DECLARE @licenseKey nvarchar(max)
DECLARE @licenseKey6 nvarchar(max)
DECLARE @licenseKey7 nvarchar(max)
DECLARE @licenseKey8 nvarchar(max)
DECLARE @kenticoVersion nvarchar(5)
SET @kenticoVersion =  (SELECT TOP 1 KeyValue FROM CMS_SettingsKey WHERE KeyName = 'CMSDBVersion')

SET @licenseKey6 = '--add your license here--'

SET @licenseKey7 = '--add your license here--'

SET @licenseKey8 = '--add your license here--'

IF @kenticoVersion = '6.0'
	BEGIN
		SET @licenseKey = @licenseKey6 
	END

IF @kenticoVersion = '7.0'
	BEGIN
		SET @licenseKey = @licenseKey7 
	END

IF @kenticoVersion LIKE '8.%'
	BEGIN
		SET @licenseKey = @licenseKey8 
	END

INSERT INTO [CMS_LicenseKey]
           ([LicenseDomain]
           ,[LicenseKey]
           ,[LicenseEdition]
           ,[LicenseExpiration]
           ,[LicensePackages]
           ,[LicenseServers])
     VALUES
           ('localhost'
           ,@licenseKey
           ,'X'
           ,'LicenseInfoProvider.FullLicense'
           ,''
           ,1)
