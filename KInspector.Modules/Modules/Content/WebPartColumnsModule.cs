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
                    new Version("9.0")
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
            if (instanceInfo.Version == new Version("6.0"))
            {
                return new ModuleResults
                {
                    Result = dbService.ExecuteAndGetPrintsFromFile("WebPartColumnsModule6.sql"),
                };
            } else if (instanceInfo.Version == new Version("9.0"))
            {
                return new ModuleResults
                {
                    Result = dbService.ExecuteAndGetPrintsFromFile("WebPartColumnsModule9.sql"),
                };
            }

            return new ModuleResults
            {
                Result = dbService.ExecuteAndGetPrintsFromFile("WebPartColumnsModule.sql"),
            };
        }
    }
}
