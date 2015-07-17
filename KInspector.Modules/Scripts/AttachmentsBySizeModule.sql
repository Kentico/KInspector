SELECT AttachmentName, AttachmentSize as N'AttachmentSize [B]', NodeAliasPath, t.SiteName, AttachmentSiteID FROM CMS_Attachment as a
INNER JOIN View_CMS_Tree_Joined as t ON t.DocumentID = a.AttachmentDocumentID
ORDER BY AttachmentSize DESC
