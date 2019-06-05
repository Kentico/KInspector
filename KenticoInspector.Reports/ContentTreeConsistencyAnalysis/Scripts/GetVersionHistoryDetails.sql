SELECT 
	DocumentID,
    CAST (NodeXML as XML) NodeXml,
	VersionClassID,
    WasPublishedFrom
FROM CMS_VersionHistory
WHERE VersionHistoryID in @IDs