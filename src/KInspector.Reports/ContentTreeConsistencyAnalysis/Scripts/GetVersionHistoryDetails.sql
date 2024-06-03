SELECT 
	DocumentID,
    CAST (NodeXML as XML) NodeXml,
	VersionClassID,
	VersionHistoryID,
    WasPublishedFrom
FROM CMS_VersionHistory
WHERE VersionHistoryID in @IDs