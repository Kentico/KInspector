SELECT 
    KeyValue 
    
    FROM 
        CMS_SettingsKey 
        
        WHERE 
            KeyName IN (
                'CMSDBVersion',
                'CMSHotfixVersion'
            )