SELECT TOP 25
	o.Name as 'Table name', 
    MAX(s.row_count) AS 'Rows',
    SUM(s.reserved_page_count) * 8.0 / 1024 as 'Size [MB]',
    (8 * 1024 * sum(s.reserved_page_count)) / (max(s.row_count)) as 'Bytes/Row'
FROM sys.dm_db_partition_stats s, sys.objects o
WHERE o.object_id = s.object_id
GROUP BY o.Name
ORDER BY 'Size [MB]' DESC