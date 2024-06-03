UPDATE CMS_WebFarmServer SET ServerDisplayName = ServerDisplayName + N'.disabled', ServerEnabled = 0
WHERE ServerID = @ServerID