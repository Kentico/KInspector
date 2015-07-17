SELECT  sys.objects.name AS 'Table Name',
        SUM(row_count) AS 'Row Count',
        SUM(reserved_page_count) * 8.0 / 1024 AS 'Table Size (MB)',
		(SELECT ClassIsDocumentType FROM CMS_Class WHERE ClassTableName = sys.objects.name) AS 'Is Document Type'

FROM sys.dm_db_partition_stats, sys.objects

WHERE sys.dm_db_partition_stats.object_id = sys.objects.object_id

GROUP BY sys.objects.name

ORDER BY 'Row Count' DESC