using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class WebPartsInTransformationsModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Performance demanding web parts in transformations",
                Comment = @"Displays transformations containing any of the following web parts:
- CMSRepeater
- CMSBreadCrumbs
- CMSListMenu
- CMSDataList

Having those web parts in a transformation can cause a significant performance hit as they load all the data from the database every time the transformation item is processed.

(e.g.: If you have 50 items processed in a transformation, you will end up with 50 database calls instead of 1)

You should use hierarchical transformation instead (see https://docs.kentico.com/display/K82/Using+hierarchical+transformations).",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0")
                },
                Category = "Content",
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("WebPartsInTransformationsModule.sql");

            return new ModuleResults
            {
                Result = results,
                Status = results.Rows.Count > 0 ? Status.Error : Status.Good
            };
        }
    }
}
