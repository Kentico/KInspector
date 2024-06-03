UPDATE CMS_User
	SET UserPassword = '', UserEnabled = 1
	WHERE UserID = @UserID

UPDATE CMS_UserSettings
	SET UserPasswordLastChanged = GETDATE()
	WHERE UserSettingsUserID = @UserID