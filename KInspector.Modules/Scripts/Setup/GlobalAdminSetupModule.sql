--clear the password for the admin account
UPDATE CMS_User
	SET UserName = 'administrator', UserIsGlobalAdministrator = 1, UserPassword = '', UserEnabled = 1
	WHERE UserID = 53

UPDATE CMS_UserSettings
	SET UserPasswordLastChanged = GETDATE()
	WHERE UserSettingsUserID = 53