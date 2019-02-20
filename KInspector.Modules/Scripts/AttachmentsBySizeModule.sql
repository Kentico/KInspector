SELECT 
	AttachmentSiteID,
	NodeAliasPath,
	AttachmentName, 
	AttachmentSize,
	ROW_NUMBER() OVER (PARTITION BY NodeAliasPath, COALESCE(AttachmentVariantParentID, AttachmentID) ORDER BY AttachmentSize desc) AS Rank
FROM CMS_Attachment AS A
INNER JOIN View_CMS_Tree_Joined AS T ON T.DocumentID = A.AttachmentDocumentID
INNER JOIN CMS_Site AS S ON S.SiteID = T.NodeSiteID
ORDER BY AttachmentSiteID,
-- Use original attachment size in case of a variant (the original attachment size should be the largest)
(
	SELECT 
		AttachmentSize 
	FROM 
		CMS_Attachment 
	WHERE 
		AttachmentID = COALESCE(A.AttachmentVariantParentID, A.AttachmentID)
) DESC,
NodeAliasPath 

SELECT SiteID, SiteDisplayName FROM CMS_Site ORDER BY SiteID
