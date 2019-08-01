SELECT 
	UserName,
	Email,
	UserPassword,
	UserPasswordFormat,
	UserPrivilegeLevel,
	FirstName,
	MiddleName,
	LastName,
	FullName

	FROM CMS_User
	
	WHERE 
		UserEnabled = 1 
		AND UserIsExternal = 0 
        AND UserName NOT IN @ExcludedUserNames
