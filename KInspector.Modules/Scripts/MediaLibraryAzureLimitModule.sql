-- File parent folder path is used to group items. 
-- This path is calcualted from FilePath field by removing the file part.
-- Example: 
-- /somefolder/subfolder/filename.ext -> /somefolder/subfolder
-- /filename.ext -> '' (empty string)
SELECT LibraryDisplayName as [Library name],
	SUBSTRING('/' + f.FilePath, 0, LEN('/' + f.FilePath) - CHARINDEX('/', REVERSE('/' + f.FilePath)) + 1) as [Folder Name], 
	SiteName as [Site name],
	COUNT(*) as [Number of files]
FROM Media_File AS f 
INNER JOIN Media_Library AS l ON f.FileLibraryID = l.LibraryID 
INNER JOIN CMS_Site AS s ON s.SiteID = l.LibrarySiteID
GROUP BY SUBSTRING('/' + f.FilePath, 0, LEN('/' + f.FilePath) - CHARINDEX('/', REVERSE('/' + f.FilePath)) + 1), LibraryDisplayName, SiteName
HAVING COUNT(*) > 100
ORDER BY LibraryDisplayName