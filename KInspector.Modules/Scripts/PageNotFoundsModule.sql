SELECT Count(EventUrl) AS Count, EventUrl, 
		MIN(EventTime) AS 'Event First Date', EventUrlReferrer as 'Referrer' 
FROM CMS_EventLog 
WHERE EventCode LIKE 'PAGENOTFOUND' 
GROUP BY EventUrl, EventUrlReferrer 
ORDER BY Count DESC