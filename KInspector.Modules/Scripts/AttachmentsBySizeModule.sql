SELECT AttachmentName, AttachmentSize as N'AttachmentSize [B]', NodeAliasPath, S.SiteName, AttachmentSiteID 
FROM CMS_Attachment AS A
INNER JOIN View_CMS_Tree_Joined AS T ON T.DocumentID = A.AttachmentDocumentID
INNER JOIN CMS_Site AS S ON S.SiteID = T.NodeSiteID
ORDER BY AttachmentSize DESC
