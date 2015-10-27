using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class OMInactiveContactsDeletion : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Inactive contact deletion settings",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0")
                },
                Comment = @"Checks whether there is Online marketing module enabled and if so, checks the old contacts deletion settings. These are important to keep OM database smaller.",
                Category = "Online marketing",
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var contactNumber = dbService.ExecuteAndGetScalar<int>(@"SELECT COUNT(*) FROM OM_Contact");

            var results = dbService.ExecuteAndGetTableFromFile(@"OMInactiveContactsDeletion.sql");

            return new ModuleResults
            {
                Result = results,
                ResultComment = @"Inactive contact deletion setting should be always set up, so that the database doesn't get too big over time.
It is a business decision what contacts could be deleted. There are now " + (contactNumber < 10000 ? "only " : "") + contactNumber + " contacts in the database.",
                Status = (contactNumber > 100000 && results.Rows.Count > 0) 
                            ? Status.Error
                            : (results.Rows.Count > 0)
                                ? Status.Warning
                                : Status.Good
            };
        }
    }
}
