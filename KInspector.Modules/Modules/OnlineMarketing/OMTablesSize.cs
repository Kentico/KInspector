using System;
using System.Collections.Generic;
using System.Linq;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class OMTablesSize : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Size of the online marketing tables",
                SupportedVersions = new[] { 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0")
                },
                Comment = @"Checks whether Online marketing tables aren't too big.

Checks the following: 
    * OM_Activity > 10 000 000,
    * OM_Contact > 1 000 000,
    * OM_ContactGroup > 50,
    * OM_Rule > 50",
                Category = "Online marketing"
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            List<string> responses = new List<string>();

            var dbService = instanceInfo.DBService;
            var activityCount = dbService.ExecuteAndGetScalar<int>("SELECT COUNT(*) FROM OM_Activity");
            if (activityCount > 10000000)
            {
                responses.Add("There are over 10 000 000 (" + activityCount + " exactly) activities in the database. Consider using deleting old page visits or setting up the old contact's deletion");
            }

            var contactsCount = dbService.ExecuteAndGetScalar<int>("SELECT COUNT(*) FROM OM_Contact");
            if (contactsCount > 1000000)
            {
                responses.Add("There are over 1 000 000 (" + contactsCount + " exactly) contacts in the database. Consider using old contact's deletion");

                var anonymousCount = dbService.ExecuteAndGetScalar<int>("SELECT COUNT(*) FROM OM_Contact WHERE ContactLastName LIKE 'Anonymous%'");
                var mergedCount = dbService.ExecuteAndGetScalar<int>("SELECT COUNT(*) FROM OM_Contact WHERE ContactMergedWithContactID NOT NULL");

                responses.Add("Out of these " + contactsCount + " contacts, " + anonymousCount + " are anonymous and " + mergedCount + " are merged");
            }

            var contactGroupCount = dbService.ExecuteAndGetScalar<int>("SELECT COUNT(*) FROM OM_ContactGroup");
            if (contactGroupCount > 50)
            {
                responses.Add("There are over 50 contact groups (" + contactGroupCount + " exactly). This might affect performance, are all of those really necessary?");
            }

            var scoringRuleCount = dbService.ExecuteAndGetScalar<int>("SELECT COUNT(*) FROM OM_Rule");
            if (scoringRuleCount > 50)
            {
                responses.Add("There are over 50 scoring rules (" + scoringRuleCount + " exactly). This might affect performance, are all of those really necessary?");
            }

            if (responses.Any())
            {
                return new ModuleResults
                {
                    Result = responses,
                    ResultComment = @"Check the counts in the result table. Exceeding the limits doesn't mean it must be wrong. 
It depends on other things like traffic, hardware and so on.",
                    Status = Status.Error,
                };
            }
            else
            {
                return new ModuleResults
                {
                    ResultComment = "All of critical Online marketing are small enough to use Online marketing without affecting performance.",
                    Status = Status.Good
                };
            }
        }
    }
}
