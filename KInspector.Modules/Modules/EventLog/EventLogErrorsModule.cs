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
                Comment = @"Displays all errors from the event log.

In the ideal world, there should be no errors. If there are any, find a root cause and fix it to avoid this error happen again.
Sometimes it's not possible to avoid all the errors, but that should be an exception.",
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
