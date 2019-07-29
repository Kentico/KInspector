
SELECT DocumentForeignKeyValue, DocumentID, DocumentName, DocumentNamePath, DocumentNodeID
	FROM CMS_Document
	WHERE DocumentID in @IDs