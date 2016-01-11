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
    public class EventLogSize : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Event Log size",
                Comment = @"Kentico recommend that the event log size is set around 5000-10,000 entries.

https://devnet.kentico.com/articles/maintenance-in-kentico?feed=Kentico",
                SupportedVersions = new[] {
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"),
                    new Version("8.1"),
                    new Version("8.2"),
                    new Version("9.0")
                },
                Category = "Event log"
            };
        }


        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            List<string> responses = new List<string>();

            var dbService = instanceInfo.DBService;
            var eventLogSize = dbService.ExecuteAndGetScalar<int>("SELECT KeyValue FROM CMS_SettingsKey where KeyName = 'CMSLogSize';");

            if (eventLogSize < 5000 || eventLogSize > 10000)
            {
                return new ModuleResults
                {
                    Result = responses,
                    ResultComment = "Kentico recommend the event log is set between 5000 and 10,000.",
                    Status = Status.Warning,
                };
            }

            return new ModuleResults
            {
                ResultComment = "The event log settings are as per recommendations, the event log size is configured as: " + eventLogSize.ToString(),
                Status = Status.Good
            };

        }
    }
}
