SELECT NodeGUID
FROM View_CMS_Tree_Joined
WHERE 
ClassName != 'cms.file' 
AND ClassName != 'cms.folder'
AND DocumentCanBePublished = 1
AND (DocumentPublishFrom IS NULL OR DocumentPublishFrom <= GETDATE())
AND (DocumentPublishTo IS NULL OR "DocumentPublishTo" >= GETDATE())
ORDER BY DocumentNamePath