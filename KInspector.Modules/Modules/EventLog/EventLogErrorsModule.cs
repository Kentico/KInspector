using System;
using KInspector.Core;

namespace KInspector.Modules.Modules.EventLog
{
    public class EventLogErrorsModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Event log errors",
                SupportedVersions = new[] {
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"),
                    new Version("8.1"),
                    new Version("8.2")
                },
                Comment = @"Checks the event log for all errors.",
                Category = "Event log"
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo, DatabaseService dbService)
        {
            var results = dbService.ExecuteAndGetTableFromFile("EventLogErrorsModule.sql");

            if (results.Rows.Count > 0)
            {
                return new ModuleResults
                {
                    Result = results,
                    ResultComment = "Errors in event log found!",
                    Status = results.Rows.Count > 10 ? Status.Error : Status.Warning,
                };
            }

            return new ModuleResults
            {
                ResultComment = "No errors were found in the event log.",
                Status = Status.Good
            };
        }
    }
}
