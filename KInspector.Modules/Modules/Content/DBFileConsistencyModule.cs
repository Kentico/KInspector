using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
	public class DBFileConsistencyModule: IModule
	{
		public ModuleMetadata GetModuleMetadata()
		{
			return new ModuleMetadata
			{
				Name = "Form Attachments and Media Library Consistency Check",
				SupportedVersions = new[] {
					new Version("8.0"),
					new Version("8.1"),
					new Version("8.2"),
					new Version("9.0")
				},
				Comment =
@"Compares the list of Attachments and Media Library items against the database, to find:
1) Media Library records not associated with any file in the filesystem.
2) Form Attachments records not associated with any file in the filesystem.
3) Form Attachments files not associated with any Form record in Kentico

Note: Page attachments and metafiles can be administered in System->Files: https://docs.kentico.com/display/K9/Administering+files+globally",
				Category = "Content"
			};
		}

		public ModuleResults GetResults(IInstanceInfo instanceInfo)
		{
			var allData = GetAllData(instanceInfo);
			var siteSettings = new AllSiteSettings(allData);
			var resultSet = new DataSet();

			ProcessMediaLibraryRecords(instanceInfo, allData, siteSettings, resultSet);
			Dictionary<int, Dictionary<string, bool>> formAttachmentRecords = GetFormAttachmentFiles(instanceInfo, allData, siteSettings);

			ProcessMissingFormAttachmentFiles(instanceInfo, allData, siteSettings, resultSet, formAttachmentRecords);

			ProcessMissingFormAttachmentRecords(resultSet, formAttachmentRecords);

			ApplyUserFriendlyTableNames(resultSet);

			return new ModuleResults
			{
				Result = resultSet,
			};
		}

		private DataSet GetAllData(IInstanceInfo instanceInfo)
		{
			var dbService = instanceInfo.DBService;

			var allData = dbService.ExecuteAndGetDataSetFromFile("GetLibraryAndAttachmentFiles.sql");
			allData.Tables[0].TableName = "KenticoSites";
			/* [SiteID]
			  ,[SiteName] */

			allData.Tables[1].TableName = "KenticoSettings";
			/* [CategoryName]
			  ,[KeyName]
			  ,[KeyValue]
			  ,[SiteID] */

			allData.Tables[2].TableName = "MediaLibraryRecords";
			/* [FileName]
			  ,[FileSiteID]
			  ,[SiteName]
			  ,[LibraryFolder]
			  ,[FilePath]
			  ,[FileTitle]
			  ,[FileDescription] */

			allData.Tables[3].TableName = "BizFormAttachmentRecords";
			/* [AttachmentGUID]
			 * ,[SiteID]
			 * ,[TableName]
			 */
			return allData;
		}

		private static void ProcessMediaLibraryRecords(IInstanceInfo instanceInfo, DataSet allData, AllSiteSettings siteSettings, DataSet ResultSet)
		{
			AddMediaLibraryResultsTable(allData, ResultSet);

			foreach(DataRow row in allData.Tables["MediaLibraryRecords"].Rows)
			{
				/* Media Library:
				 * //MediaLibraryBaseFolder/SiteName?/folder/image.filetype
				 */

				var siteID = Convert.ToInt32(row["FileSiteID"]);
				var siteBaseFolder = siteSettings[siteID].baseMediaFolder;
				var libraryFolder = row["LibraryFolder"].ToString();
				var filePath = row["FilePath"].ToString();
				var fullFilePath = UriExtensions.Combine(siteBaseFolder, libraryFolder, filePath);

				if(!UriExtensions.Exists(fullFilePath, instanceInfo))
				{
					var tempArray = AddAttachmentUrlToMediaRecord(row, fullFilePath);
					ResultSet.Tables["MediaLibraryMissingFiles"].Rows.Add(tempArray);
				}
			}
		}

		private static void AddMediaLibraryResultsTable(DataSet allData, DataSet ResultSet)
		{
			/*[FileName]
			  ,[FileSiteID]
			  ,[SiteName]
			  ,[LibraryFolder]
			  ,[FilePath]
			  ,[FileTitle]
			  ,[FileDescription]
			  ,[AttachmentURL] 
			  */
			ResultSet.Tables.Add(allData.Tables["MediaLibraryRecords"].Clone());
			ResultSet.Tables[0].TableName = "MediaLibraryMissingFiles";
			ResultSet.Tables[0].Columns.Add("AttachmentURL");
		}

		private static object[] AddAttachmentUrlToMediaRecord(DataRow row, string fullFilePath)
		{
			var tempArray = new List<object>(row.ItemArray);
			tempArray.Add(fullFilePath);
			return tempArray.ToArray();
		}

		private static Dictionary<int, Dictionary<string, bool>> GetFormAttachmentFiles(IInstanceInfo instanceInfo, DataSet allData, AllSiteSettings siteSettings)
		{
			Dictionary<int, Dictionary<string, bool>> fileItems = new Dictionary<int, Dictionary<string, bool>>();

			foreach(DataRow siteRow in allData.Tables["KenticoSites"].Rows)
			{
				var siteID = Convert.ToInt32(siteRow["SiteID"]);
				fileItems.Add(siteID, new Dictionary<string, bool>());
				foreach(var fileObj in UriExtensions.GetFiles(siteSettings[siteID].baseFormAttachmentsFolder, true, instanceInfo))
				{
					fileItems[siteID].Add(fileObj.Name, false);
				}
			}

			return fileItems;
		}
		
		private static void ProcessMissingFormAttachmentFiles(IInstanceInfo instanceInfo, DataSet allData, AllSiteSettings siteSettings, DataSet resultSet, Dictionary<int, Dictionary<string, bool>> formAttachmentRecords)
		{
			AddMissingFormAttachmentFilesTable(allData, resultSet);

			/* bizform Attachments:
			 * //UploadedFormFiles/SiteName?/unknownguid.filetype)
			 * AttachmentGuid is unknownguid.filetype/filename.filetype
			 */
			foreach(DataRow row in allData.Tables["BizFormAttachmentRecords"].Rows)
			{
				var siteID = Convert.ToInt32(row["SiteID"]);
				var guidSplit = row["AttachmentGUID"].ToString().Split(new char[] { '/' }, 2);
				if(guidSplit.Length != 2)
					{ throw new ApplicationException($"AttachmentGUID of '{row["AttachmentGUID"].ToString()}' expected a '/' but did not find one."); }
				var rowURL = UriExtensions.Combine(siteSettings[siteID].baseFormAttachmentsFolder, guidSplit[0]);
				if(!UriExtensions.Exists(rowURL, instanceInfo))
				{
					var tempArray = new List<object>(row.ItemArray);
					tempArray[0] = guidSplit[0];
					tempArray.Add(guidSplit[1]);
					tempArray.Add(rowURL);
					resultSet.Tables["BizFormAttachmentMissingFiles"].Rows.Add(tempArray.ToArray());
				}

				//As long as we're looping over the Attachment records anyways, take note of which records were found.
				if(formAttachmentRecords[siteID].ContainsKey(guidSplit[0]))
				{
					formAttachmentRecords[siteID][guidSplit[0]] = true;
				}
			}
		}

		private static void AddMissingFormAttachmentFilesTable(DataSet allData, DataSet resultSet)
		{
			/* [AttachmentGUID]
			 * ,[SiteID]
			 * ,[TableName]
			 * ,[FileName]
			 * ,[AttachmentURL] 
			 */
			resultSet.Tables.Add(allData.Tables["BizFormAttachmentRecords"].Clone());
			resultSet.Tables[1].TableName = "BizFormAttachmentMissingFiles";
			resultSet.Tables[1].Columns.Add("FileName");
			resultSet.Tables[1].Columns.Add("AttachmentURL");
		}

		private static void ProcessMissingFormAttachmentRecords(DataSet resultSet, Dictionary<int, Dictionary<string, bool>> formAttachmentRecords)
		{
			AddMissingFormAttachmentRecordsTable(resultSet);

			/* bizform Attachments:
			 * //UploadedFormFiles/SiteName?/unknownguid.filetype)
			 * AttachmentGuid is unknownguid.filetype/filename.filetype
			 */
			foreach(var siteItem in formAttachmentRecords)
			{
				foreach(var record in siteItem.Value)
				{
					if(record.Value == false)
					{
						var tempArray = new List<object>();
						tempArray.Add(siteItem.Key);
						tempArray.Add(record.Key);
						resultSet.Tables["BizFormAttachmentMissingRecords"].Rows.Add(tempArray.ToArray());
					}
				}
			}
		}

		private static void AddMissingFormAttachmentRecordsTable(DataSet resultSet)
		{
			/* [SiteID]
			 * ,[AttachmentGUID] 
			 */
			resultSet.Tables.Add(new DataTable());
			resultSet.Tables[2].TableName = "BizFormAttachmentMissingRecords";
			resultSet.Tables[2].Columns.Add("SiteID");
			resultSet.Tables[2].Columns.Add("AttachmentGUID");
		}

		private static void ApplyUserFriendlyTableNames(DataSet resultSet)
		{
			resultSet.Tables[0].TableName = "1) Media Library, Missing Files";
			resultSet.Tables[1].TableName = "2) BizForm Attachments, Missing Files";
			resultSet.Tables[2].TableName = "3) BizForm Attachments, Missing Records";
		}

		/// <summary>
		/// This class organizes the various Folder settings per-site, so I can just query for the base folder for a given site/object combination.
		/// </summary>
		private class AllSiteSettings: Dictionary<int, PerSiteSettings>
		{
			public AllSiteSettings(DataSet settingsData)
			{
				foreach(DataRow siteRow in settingsData.Tables["KenticoSites"].Rows)
				{
					this.Add(siteRow.Field<int>("SiteID"), new PerSiteSettings(siteRow.Field<int>("SiteID"), siteRow.Field<string>("SiteName"), settingsData.Tables["KenticoSettings"]));
				}
			}

			public string GetBaseMediaFolder(int siteID)
			{
				return this[siteID].baseMediaFolder;
			}

			public string GetBaseFormAttachmentsFolder(int siteID)
			{
				return this[siteID].baseFormAttachmentsFolder;
			}
		}

		/// <summary>
		/// Provides a single Kentico Site's settings for getting the Media Library and Bizform base folders.
		/// </summary>
		private class PerSiteSettings
		{
			public PerSiteSettings(int siteID, string siteName, DataTable settingsTable)
			{
				baseMediaFolder = $"~/{siteName}/media";
				baseFormAttachmentsFolder = $"~/{siteName}/BizFormFiles";

				DataTable siteSettings = new DataView(settingsTable, $"SiteID = {siteID}", "KeyName", DataViewRowState.CurrentRows).ToTable();
				DataTable globalSettings = new DataView(settingsTable, "SiteID is null", "KeyName", DataViewRowState.CurrentRows).ToTable();

				var baseMediaFolderData = siteSettings.Select($"KeyName = 'CMSMediaLibrariesFolder'");
				var baseMediaUseSiteData = siteSettings.Select($"KeyName = 'CMSUseMediaLibrariesSiteFolder'");
				var baseFormAttachmentFolderData = siteSettings.Select($"KeyName = 'CMSBizFormFilesFolder'");
				var baseFormAttachmentsUseSiteData = siteSettings.Select($"KeyName = 'CMSUseBizFormsSiteFolder'");

				if(baseMediaFolderData.GetLength(0) == 0) baseMediaFolderData = globalSettings.Select($"KeyName = 'CMSMediaLibrariesFolder'");
				if(baseMediaUseSiteData.GetLength(0) == 0) baseMediaUseSiteData = globalSettings.Select($"KeyName = 'CMSUseMediaLibrariesSiteFolder'");
				if(baseFormAttachmentFolderData.GetLength(0) == 0) baseFormAttachmentFolderData = globalSettings.Select($"KeyName = 'CMSBizFormFilesFolder'");
				if(baseFormAttachmentsUseSiteData.GetLength(0) == 0) baseFormAttachmentsUseSiteData = globalSettings.Select($"KeyName = 'CMSUseBizFormsSiteFolder'");

				var baseMediaFolderString = baseMediaFolderData[0]["KeyValue"] as String;
				var baseMediaUseSiteBool = Convert.ToBoolean(baseMediaUseSiteData[0]["KeyValue"]);
				var baseFormAttachmentFolderString = baseFormAttachmentFolderData[0]["KeyValue"] as String;
				var baseFormAttachmentsUseSiteBool = Convert.ToBoolean(baseFormAttachmentsUseSiteData[0]["KeyValue"]);

				if(!String.IsNullOrWhiteSpace(baseMediaFolderString))
				{
					baseMediaFolder = UriExtensions.Combine(baseMediaFolderString, (baseMediaUseSiteBool ? $"{siteName}" : ""));
				}
				if(!String.IsNullOrWhiteSpace(baseFormAttachmentFolderString))
				{
					baseFormAttachmentsFolder = UriExtensions.Combine(baseFormAttachmentFolderString, (baseFormAttachmentsUseSiteBool ? $"{siteName}" : ""));
				}

				if(!Regex.IsMatch(baseMediaFolder, "^[a-zA-Z]:/.*")
				&& !Regex.IsMatch(baseMediaFolder, "^~/.*")
				&& !Regex.IsMatch(baseMediaFolder, "^\\\\.*"))
				{
					throw new ArgumentException($"This method supports Kentico's three suggested file formats: '\\servername\', 'c:/', and '~/'. Your Media library setting of '{baseMediaFolder}' does not match these.");
				}

				if(!Regex.IsMatch(baseFormAttachmentsFolder, "^[a-zA-Z]:/.*")
				&& !Regex.IsMatch(baseFormAttachmentsFolder, "^~/.*")
				&& !Regex.IsMatch(baseFormAttachmentsFolder, "^\\\\.*"))
				{
					throw new ArgumentException($"This method supports Kentico's three suggested file formats: '\\servername\', 'c:/', and '~/'. Your Form Attachment setting of '{baseFormAttachmentsFolder}' does not match these.");
				}

			}

			public string baseMediaFolder;

			public string baseFormAttachmentsFolder;
		}

		/// <summary>
		/// This class provides some extensions for dealing with the fact that we aren't in Kentico's context.
		/// </summary>
		private class UriExtensions
		{
			/// <summary>
			/// This method should return whether the file path passed in exists.
			/// </summary>
			/// <param name="baseUri">The virtual, server, or physical path</param>
			/// <returns></returns>
			public static bool Exists(string baseUri, IInstanceInfo info)
			{
				if(baseUri == null)
				{
					throw new ArgumentNullException($"baseUri value is null");
				}

				try
				{

					if(baseUri.StartsWith("~/"))
					{
						var absPath = Combine(info.Directory.FullName, baseUri.Substring(2));
						return File.Exists(absPath);
					}
					else
						return File.Exists(baseUri);

				}
				catch(Exception ex)
				{
					throw ex;
				}

			}

			/// <summary>
			/// This method should return whether the file path passed in exists.
			/// </summary>
			/// <param name="baseUri">The virtual, server, or physical path</param>
			/// <returns></returns>
			public static FileInfo[] GetFiles(string baseUri, bool recursive, IInstanceInfo info)
			{
				if(baseUri == null)
				{
					throw new ArgumentNullException($"baseUri value is null");
				}

				try
				{
					if(baseUri.StartsWith("~/"))
					{
						var absPath = Combine(info.Directory.FullName, baseUri.Substring(2));
						return Directory.Exists(absPath) ? new DirectoryInfo(absPath).GetFiles("*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly) : new FileInfo[] { };
					}
					else
						return Directory.Exists(baseUri) ? new DirectoryInfo(baseUri).GetFiles("*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly) : new FileInfo[] { };
				}
				catch(Exception ex)
				{
					throw ex;
				}
	}

			/// <summary>
			/// Combines a base url with a list of folders afterwards.
			/// </summary>
			/// <param name="baseUri">The virtual, server, or physical path of origination</param>
			/// <param name="addUris">Any number of additional strings for URI additions.</param>
			/// <returns></returns>
			public static string Combine(string baseUri, params string[] addUris)
			{
				if(baseUri == null)
				{
					throw new ArgumentNullException($"baseUri value is null");
				}

				if(addUris.Length == 0)
				{
					throw new ArgumentNullException("addUris was empty");
				}

				if(baseUri.StartsWith("~/"))
				{
					string combinedUrl = baseUri;
					foreach(var addUri in addUris)
					{
						combinedUrl = System.Web.VirtualPathUtility.AppendTrailingSlash(new Uri(combinedUrl, UriKind.Relative).ToString());
						combinedUrl = System.Web.VirtualPathUtility.Combine(combinedUrl, new Uri(addUri, UriKind.Relative).ToString());
					}

					return new Uri(combinedUrl, UriKind.Relative).ToString();
				}
				else
				{
					bool usesForwardSlash = Regex.IsMatch(baseUri, "^[a-zA-Z]:/.*");
					var allUris = new string[addUris.Length + 1];
					allUris[0] = baseUri;
					addUris.CopyTo(allUris, 1);
					var combinedPath = Path.Combine(allUris);
					return !usesForwardSlash ? combinedPath : combinedPath.Replace('\\', '/');
				}
			}
		}
	}
}
