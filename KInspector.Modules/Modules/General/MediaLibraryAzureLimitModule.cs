using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
	public class MediaLibraryAzureLimitModule : IModule
	{
		public ModuleMetadata GetModuleMetadata()
		{
			return new ModuleMetadata
			{
				Name = "Maximum 100 files per folder in Azure Blob storage",
				SupportedVersions = new[] {
					new Version("6.0"),
					new Version("7.0"),
					new Version("8.0"),
					new Version("8.1"),
					new Version("8.2"),
					new Version("9.0")
				},
				Category = "Performance",
				Comment = @"Web sites utilizing Azure Blob storage should limit media library folder size to maximum 100 items per folder to achieve good performance.",
			};
		}


		public ModuleResults GetResults(IInstanceInfo instanceInfo)
		{
			var dbService = instanceInfo.DBService;
			var results = dbService.ExecuteAndGetTableFromFile("MediaLibraryAzureLimitModule.sql");

			if (results.Rows.Count > 0)
			{
				return new ModuleResults
				{
					Result = results,
					ResultComment = "Some of your media library folders contains more than 100 items. If your website is utilizing Azure Blob storage, you should rearrange your folders to improve the performance.",
					Status = Status.Warning
				};
			}

			return new ModuleResults
			{
				Result = "None of your media library folders contains more than 100 items.",
				Status = Status.Good
			};
		}
	}
}
