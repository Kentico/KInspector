
SELECT DocumentForeignKeyValue, DocumentID, DocumentName, DocumentNodeID
	FROM CMS_Document
	WHERE DocumentID in @IDs