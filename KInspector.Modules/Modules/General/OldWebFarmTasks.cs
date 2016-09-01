using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Configuration;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class OldWebFarmTasks : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Old Web Farm Tasks",
                Comment = @"As of Kentico 9 Web Farm health is monitored and managed, one of these features is that tasks are not generated after 24 hours.  Tasks that are older than 24 hours in age on Kentico 8.x and earlier versions indicate that there is a problem with the web farm health.

https://docs.kentico.com/display/K9/Troubleshooting+web+farms",
                SupportedVersions = new[] {
                    new Version("7.0"),
                    new Version("8.0"),
                    new Version("8.1"),
                    new Version("8.2")
                }
            };
        }


        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            List<string> responses = new List<string>();

            var dbService = instanceInfo.DBService;
            var taskRowCount = dbService.ExecuteAndGetScalar<int>("SELECT count(*) FROM CMS_WebFarmTask WHERE TaskCreated < DATEADD(hour, -24, GETDATE());");

            if (taskRowCount > 0)
            {
                responses.Add("There are tasks over 24 hours old in the web farm (" + taskRowCount + " tasks exactly).");
                return new ModuleResults
                {
                    Result = responses,
                    ResultComment = "There are tasks that are over 24 hours old in the CMS_WebFarmTask table. Please check the health of the web farm.",
                    Status = Status.Error,
                };
                
            }

            return new ModuleResults
            {
                ResultComment = "There are no aged tasks pending.",
                Status = Status.Good
            };

        }
    }
}
