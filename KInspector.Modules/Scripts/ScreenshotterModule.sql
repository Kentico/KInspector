select NodeGUID
from View_CMS_Tree_Joined
where ClassName != 'cms.file' and ClassName != 'cms.folder' and Published = 1
order by DocumentNamePath