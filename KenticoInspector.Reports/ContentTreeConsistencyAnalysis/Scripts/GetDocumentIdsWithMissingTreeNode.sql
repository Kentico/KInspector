﻿SELECT DocumentID
	FROM CMS_Document
	WHERE DocumentNodeID NOT IN (SELECT NodeID FROM CMS_Tree)