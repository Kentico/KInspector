using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
					new Version("6.0"),
					new Version("7.0"),
					new Version("8.0"),
					new Version("8.1"),
					new Version("8.2"),
					new Version("9.0")
				},
				Comment =
@"Compares the list of Attachments and Media Library items against the database, to find:
1) Media Library records not associated with any file in the filesystem.
2) Form Attachments records not associated with any file in the filesystem.
3) TODO: Form Attachments files not associated with any Form record in Kentico

Note: Page attachments and metafiles can be administered in System->Files: https://docs.kentico.com/display/K9/Administering+files+globally",
				Category = "Content"
			};
		}

		public ModuleResults GetResults(IInstanceInfo instanceInfo)
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

			//Next, let's go through the three cases:

			var siteSettings = new AllSiteSettings(allData);
			var ResultSet = new DataSet();
			ResultSet.Tables.Add(allData.Tables["MediaLibraryRecords"].Clone());
			ResultSet.Tables[0].TableName = "MediaLibraryMissingRecords";
			ResultSet.Tables[0].Columns.Add("AttachmentURL");

			/* Media Library:
			 * //MediaLibraryBaseFolder/SiteName?/folder/image.filetype
			*/

			foreach(DataRow row in allData.Tables["MediaLibraryRecords"].Rows)
			{
				var siteID = Convert.ToInt32(row["FileSiteID"]);
				var rowURL = UriExtensions.Combine(siteSettings[siteID].baseMediaFolder, row["LibraryFolder"].ToString(), row["FilePath"].ToString());
				if(!UriExtensions.Exists(rowURL, instanceInfo))
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

					var tempArray = new List<object>(row.ItemArray);
					tempArray.Add(rowURL);
					ResultSet.Tables["MediaLibraryMissingRecords"].Rows.Add(tempArray.ToArray());
				}
					
			}

			ResultSet.Tables.Add(allData.Tables["BizFormAttachmentRecords"].Clone());
			ResultSet.Tables[1].TableName = "BizFormAttachmentMissingRecords";
			ResultSet.Tables[1].Columns.Add("FileName");
			ResultSet.Tables[1].Columns.Add("AttachmentURL");


			/* bizform Attachments:
			 * //UploadedFormFiles/SiteName?/unknownguid.filetype)
			 * AttachmentGuid is unknownguid.filetype/filename.filetype
			 */
			foreach(DataRow row in allData.Tables["BizFormAttachmentRecords"].Rows)
			{
				var siteID = Convert.ToInt32(row["SiteID"]);
				var guidSplit = row["AttachmentGUID"].ToString().Split(new char[] { '/' }, 2);
				if(guidSplit.Length != 2) { throw new ApplicationException($"AttachmentGUID of '{row["AttachmentGUID"].ToString()}' expected a '/' but did not find one."); }
				var rowURL = UriExtensions.Combine(siteSettings[siteID].baseFormAttachmentsFolder, guidSplit[0]);
				if(!UriExtensions.Exists(rowURL, instanceInfo))
				{
					/* [AttachmentGUID]
					 * ,[SiteID]
					 * ,[TableName]
					 * ,[FileName]
					 * ,[AttachmentURL] 
					 */
					var tempArray = new List<object>(row.ItemArray);
					tempArray[0] = guidSplit[0];
					tempArray.Add(guidSplit[1]);
					tempArray.Add(rowURL);
					ResultSet.Tables["BizFormAttachmentMissingRecords"].Rows.Add(tempArray.ToArray());
				}
			}

			return new ModuleResults
			{
				Result = ResultSet,
			};
		}

		/// <summary>
		/// This class organizes the various Folder settings per-site, so I can just query for the base folder for a given site/object combination.
		/// </summary>
		private class AllSiteSettings : Dictionary<int, PerSiteSettings>
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
			/// This method does not work currently, but should return whether the file path passed in exists.
			/// </summary>
			/// <param name="baseUri">The virtual, server, or physical path</param>
			/// <returns></returns>
			public static bool Exists(string baseUri, IInstanceInfo info)
			{
				if(baseUri == null)
				{
					throw new ArgumentNullException($"baseUri of '{baseUri}' is null");
				}

				if(baseUri.StartsWith("~/"))
				{
					//If we're using a relative path, it's relative to the "CMS" folder, not the Kentico base folder.
					var absPath = Path.Combine(info.Directory.FullName, "CMS", baseUri.Substring(2));
					return File.Exists(absPath);
				}
				else
					return File.Exists(baseUri);
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
					throw new ArgumentNullException($"baseUri of '{baseUri}' is null");
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
					var allUris = new string[addUris.Length + 1];
					allUris[0] = baseUri;
					addUris.CopyTo(allUris, 1);
					return Path.Combine(allUris);
				}
			}
		}
	}
}
