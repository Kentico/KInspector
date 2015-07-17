using System;
using KInspector.Core;

namespace KInspector.Modules.Modules.OnlineMarketing
{
    public class OMContactGroupsWithManualMacro : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Contact groups with manual macro",
                SupportedVersions = new[] { 
                    new Version("8.1"), 
                    new Version("8.2") 
                },
                Comment = @"Contact groups that have macro condition set via plain macro are always just slower and should be rewritten into MacroRuleDesigner, so that they can be translated into SQL queries to decrease recalculation time.
For more, see https://docs.kentico.com/display/K81/Improving+custom+macro+performance+in+scoring+and+contact+groups

NOTE: This applies only on Kentico 8.1, where the improvements were introduced.
",
                Category = "Online marketing"
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo, DatabaseService dbService)
        {
            var manualContactGroups = dbService.ExecuteAndGetTableFromFile("OMContactGroupsWithManualMacro.sql");
            if (manualContactGroups.Rows.Count > 0)
            {
                return new ModuleResults
                {
                    Result = manualContactGroups, 
                    ResultComment = "These contact groups use plain macro that should be translated into MacroRules, so that the SQL translation could be leveraged. This is like MUCH slower.",
                    Status = Status.Error,
                };
            }

            return new ModuleResults
            {
                ResultComment = "All existing contact groups are designed with Macro, so they can leverage fast recalculation ",
                Status = Status.Good
            };
        }
    }
}
