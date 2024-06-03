SELECT ServerID AS 'ID', ServerName AS 'Name', ServerAuthenticationType AS 'Authentication', ServerUserName AS 'User',
	CASE WHEN ServerDeliveryMethod = 0 THEN 'Network' WHEN ServerDeliveryMethod = 1 THEN 'Pickup (directory)' WHEN ServerDeliveryMethod = 2 THEN 'Pickup (IIS)' END AS 'DeliveryMethod',
	ServerUseSSL AS 'SSL', ServerIsGlobal AS 'Global', ServerEnabled AS 'Enabled'
FROM CMS_SMTPServer