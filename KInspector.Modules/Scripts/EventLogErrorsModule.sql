SELECT Count(EventDescription) AS Count, EventCode, Source, 
	   EventDescription, MIN(EventTime) AS 'Event First Date', 
	   MAX(EventTime) AS 'Event Last Date' 
FROM CMS_EventLog 
WHERE EventType = 'E' 
GROUP BY Source, EventCode, EventDescription 
ORDER BY Count DESC