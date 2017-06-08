using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class WebPartColumnsModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Unspecified 'columns' setting in web parts",
                SupportedVersions = new[] {
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"),
                    new Version("8.1"),
                    new Version("8.2"),
                    new Version("9.0"),
                    new Version("10.0")
                },
                Comment = @"Displays list of web parts where 'columns' property is not specified.

Web parts without specified 'columns' property must load all field from the database. 
By specifying this property, you can significantly lower the data transmission from database to the server and improve the load times.

For more information, see documentation:
https://docs.kentico.com/display/K82/Loading+data+efficiently",
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;

            string scriptFileName = string.Empty;

            if (instanceInfo.Version.Major == 6)
            {
                scriptFileName = "WebPartColumnsModule6.sql";
            }

            if (instanceInfo.Version.Major == 7 || instanceInfo.Version.Major == 8)
            {
                scriptFileName = "WebPartColumnsModule7.sql";
            }

            if (instanceInfo.Version.Major >= 9)
            {
                scriptFileName = "WebPartColumnsModule9.sql";
            }


            return new ModuleResults
            {
                Result = string.IsNullOrWhiteSpace(scriptFileName) ? null : dbService.ExecuteAndGetPrintsFromFile(scriptFileName)
            };
        }
    }
}
