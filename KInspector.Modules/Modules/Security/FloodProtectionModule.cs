using Kentico.KInspector.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentico.KInspector.Modules
{
    public class FloodProtectionModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Flood Protection",
                Comment = @"This module checks to see if Flood Protection is enabled on the site. This module also checks to make sure Flood Protection is enabled for the Chat module in V8 and later. https://docs.kentico.com/display/K9/Flood+protection",
                SupportedVersions = new[] {
                    new Version("7.0"),
                    new Version("8.0"),
                    new Version("8.1"),
                    new Version("8.2"),
                    new Version("9.0")
                },
                Category = "Security",
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("FloodProtectionModule.sql");
            Status resultstatus = Status.Good;

            //Make sure there are records
            if (results.Rows.Count > 0)
            {
                //Loop through the records
                foreach (DataRow drFloodProtection in results.Rows)
                {
                    //Determine which key is being examined
                    switch (drFloodProtection["KeyName"].ToString())
                    {
                        case "CMSFloodProtectionEnabled": //Flood Protection
                            if (drFloodProtection["KeyValue"].ToString().ToLower() != "true")
                            {
                                drFloodProtection["Notes"] = "It is recommended that you have CMSFloodProtectionEnabled set to True. You can find this setting here: Security & Membership > Protection > Flood protection'";
                                resultstatus = Status.Warning;
                            }
                            break;
                        case "CMSChatEnableFloodProtection": //Chat Flood Protection
                            if (drFloodProtection["KeyValue"].ToString().ToLower() != "true")
                            {
                                drFloodProtection["Notes"] = "It is recommended that you have CMSChatEnableFloodProtection set to True. You can find this setting here: Community > Chat > Flood protection";
                                resultstatus = Status.Warning;
                            }
                            break;
                    }
                }

                //Check if the all of the settings are set correctly.
                if (resultstatus != Status.Good)
                {
                    //Return the issues
                    return new ModuleResults
                    {
                        Result = results,
                        Status = resultstatus
                    };
                }
                else
                {
                    return new ModuleResults
                    {
                        ResultComment = "Flood Protection is enabled.",
                        Status = Status.Good
                    };
                }
            }

            return new ModuleResults
            {
                ResultComment = "Failed to check settings as expected.",
                Status = Status.Error
            };

        }
    }
}