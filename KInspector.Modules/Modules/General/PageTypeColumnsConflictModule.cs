using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class PageTypeColumnsConflictModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "TODO: rename // (possible) Page type columns conflict module",
                SupportedVersions = new[] { 
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0"),
                },
                Comment = @"TODO add description",
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("PageTypeColumnsConflict.sql");

            return new ModuleResults
            {
                Result = results,
                Status = results.Rows.Count > 0 ? Status.Warning : Status.Good,
            };
        }
    }
}
