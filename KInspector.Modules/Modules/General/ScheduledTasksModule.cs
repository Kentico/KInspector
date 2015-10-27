using System;
using System.Linq;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class ScheduledTasksModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "Important scheduled tasks",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0")
                },
                Comment = @"Selects important scheduled tasks",
                
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetPrintsFromFile("ScheduledTasksModule.sql");

            var res = new ModuleResults
            {
                Result = results,
            };

            if (results.Any(x => x.Contains("DISABLE SCHEDULED TASK!") 
                || x.Contains("DON NOT RUN THE task as EXTERNAL") 
                || x.Contains("RUN THE task as EXTERNAL")))
            {
                res.Status = Status.Warning;
            }

            return res;
        }
    }
}
