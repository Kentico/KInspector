using KInspector.Actions.ResetCmsUserLogin.Models;
using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;

namespace KInspector.Actions.ResetCmsUserLogin
{
    public class Action : AbstractAction<Terms, Options>
    {
        private readonly IDatabaseService databaseService;

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string> {
            ModuleTags.Reset,
            ModuleTags.User
        };

        public Action(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public async override Task<ModuleResults> Execute(Options? options)
        {
            var isValid = await UserIsValid(options?.UserId);
            if (!isValid)
            {
                return await GetInvalidOptionsResult();
            }

            // Reset provided user
            await databaseService.ExecuteNonQuery(Scripts.ResetAndEnableUser, new { UserID = options?.UserId });
            var result = await ExecuteListing();
            result.Status = ResultsStatus.Good;
            result.Summary = Metadata.Terms.UserReset?.With(new {
                userId = options?.UserId
            });

            return result;
        }

        public override Task<ModuleResults> ExecutePartial(Options? options)
        {
            // All options are required for this action
            throw new NotImplementedException();
        }

        public async override Task<ModuleResults> ExecuteListing()
        {
            var administratorUsers = await databaseService.ExecuteSqlFromFile<CmsUser>(Scripts.GetAdministrators);
            var results = new ModuleResults
            {
                Type = ResultsType.TableList,
                Status = ResultsStatus.Information,
                Summary = Metadata.Terms.ListSummary
            };
            results.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.TableTitle,
                Rows = administratorUsers
            });

            return results;
        }

        public async override Task<ModuleResults> GetInvalidOptionsResult()
        {
            var result = await ExecuteListing();
            result.Status = ResultsStatus.Error;
            result.Summary = Metadata.Terms.InvalidOptions;

            return result;
        }

        private async Task<bool> UserIsValid(int? userId)
        {
            var users = await databaseService.ExecuteSqlFromFile<CmsUser>(Scripts.GetAdministrators);

            return userId > 0 && users.Any(u => u.UserID == userId);
        }
    }
}
