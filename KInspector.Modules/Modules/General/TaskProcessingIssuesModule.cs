using System;
using System.Data;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class TaskProcessingIssues : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Task processing issues",
                SupportedVersions = new[] {
                    new Version("9.0"),
                    new Version("10.0"),
                    new Version("11.0"),
                    new Version("12.0")
                },
                Comment = @"Checks for possible issues with processing system, workflow, marketing automation, smart search, and web farm tasks.",
                Category = "Database"
            };
        }


        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("TaskProcessingIssueModule.sql");

            var hasUnprocessedTasks = false;
            if (results != null && results.Rows.Count > 0)
            {
                var row = results.Rows[0];

                foreach (DataColumn column in results.Columns)
                {
                    var value = int.Parse(row[column].ToString());
                    if (value > 0)
                    {
                        hasUnprocessedTasks = true;
                        break;
                    }
                }
            }

            return new ModuleResults
            {
                Result = results,
                Status = hasUnprocessedTasks ? Status.Warning : Status.Good,
                ResultComment = hasUnprocessedTasks ? "There are unprocessed tasks that should be reviewed." : "All tasks have been processed."
            };
        }
    }
}