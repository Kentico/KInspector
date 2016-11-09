using System;
using System.Data;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class ExpiredTokensModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Expired tokens",
                Category = "Social marketing",
                Comment = "Checks that there are no expired tokens.",
                SupportedVersions = new[] { 
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0")
                },
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            DataTable expiredTokens = new DataTable("Expired account tokens");
            expiredTokens.Columns.Add("SocialNetwork");
            expiredTokens.Columns.Add("SiteName");
            expiredTokens.Columns.Add("AccountName");

            var dbService = instanceInfo.DBService;
            if (instanceInfo.Version != new Version("8.0"))
            {
                // LinkedIn integration is in 8.1 and newer
                var linResults = dbService.ExecuteAndGetTableFromFile("ExpiredTokensModule-LinkedIn.sql");
                foreach (DataRow token in linResults.Rows)
                {
                    var row = expiredTokens.NewRow();
                    row["SocialNetwork"] = "LinkedIn";
                    row["SiteName"] = token["SiteName"];
                    row["AccountName"] = token["AccountName"];
                    expiredTokens.Rows.Add(row);
                }
            }

            var fbResults = dbService.ExecuteAndGetTableFromFile("ExpiredTokensModule-Facebook.sql");
            if (fbResults.Rows.Count > 0)
            {
                foreach (DataRow token in fbResults.Rows)
                {
                    var row = expiredTokens.NewRow();
                    row["SocialNetwork"] = "Facebook";
                    row["SiteName"] = token["SiteName"];
                    row["AccountName"] = token["AccountName"];
                    expiredTokens.Rows.Add(row);
                }                
            }

            if (expiredTokens.Rows.Count > 0)
            {
                return new ModuleResults
                {
                    Result = expiredTokens,
                    Status = Status.Error,
                    ResultComment = "Tokens have expired, posting stuff on some social marketing accounts doesn't work at all for accounts in results. Tell customer to reauthorize the pages.",
                };
            }

            return new ModuleResults
            {
                Status = Status.Good,
                ResultComment = "There are no expired tokens.",
            };
        }
    }
}
