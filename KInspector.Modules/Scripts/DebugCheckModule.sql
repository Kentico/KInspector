﻿SELECT Count(KeyName) as 'DebugCount' from CMS_SettingsKey where KeyName LIKE 'CMSDebug%' and KeyValue = 'True'