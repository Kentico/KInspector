SELECT EventCode, EventTime, EventMachineName 
FROM CMS_EventLog 
WHERE EventCode = 'ENDAPP' OR EventCode = 'STARTAPP' 
ORDER BY EventTime DESC, EventMachineName