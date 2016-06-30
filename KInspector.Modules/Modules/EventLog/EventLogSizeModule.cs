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
                Comment = @"Kentico recommends that the event log size is set around 5000-10,000 entries.

https://devnet.kentico.com/articles/maintenance-in-kentico",
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


        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {

            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("EventLogSizeModule.sql");

            if (results.Rows.Count > 0)
            {
                foreach (DataRow resultRow in results.Rows)
                {                    
                    if(!EventLogIsRecommendedSize(resultRow))
                    {
                        return new ModuleResults
                        {
                            Result = results,
                            ResultComment = "The event log settings are set outside the recommended range.",
                            Status = Status.Warning,
                        };
                    }
                }

            }
               
            return new ModuleResults
            {
                ResultComment = "The event log settings are as per recommendations.",
                Status = Status.Good
            };

        }

        private bool EventLogIsRecommendedSize(DataRow settingRow)
        {
            var size = Convert.ToInt32(settingRow["KeyValue"]);

            return size >= 5000 && size <= 10000;   
        }
    }
}
