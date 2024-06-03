using KInspector.Actions.StopRunningSites.Models;
using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;

namespace KInspector.Actions.StopRunningSites
{
    public class Action : AbstractAction<Terms, Options>
    {
        private readonly IDatabaseService databaseService;

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("12", "13");

        public override IList<string> Tags => new List<string> {
            ModuleTags.Site,
            ModuleTags.Configuration
        };

        public Action(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public async override Task<ModuleResults> Execute(Options? options)
        {
            var isValid = await SiteIsValid(options?.SiteId);
            if (!isValid)
            {
                return await GetInvalidOptionsResult();
            }

            await databaseService.ExecuteNonQuery(Scripts.StopSite, new { SiteID = options?.SiteId });
            var result = await ExecuteListing();
            result.Status = ResultsStatus.Good;
            result.Summary = Metadata.Terms.SiteStopped?.With(new
            {
                siteId = options?.SiteId
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
            var sites = await databaseService.ExecuteSqlFromFile<CmsSite>(Scripts.GetSiteSummary);
            var results = new ModuleResults
            {
                Type = ResultsType.TableList,
                Status = ResultsStatus.Information,
                Summary = Metadata.Terms.ListSummary
            };
            results.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.TableTitle,
                Rows = sites
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

        private async Task<bool> SiteIsValid(int? siteId)
        {
            var sites = await databaseService.ExecuteSqlFromFile<CmsSite>(Scripts.GetSiteSummary);

            return siteId > 0 && sites.Any(s => s.ID == siteId);
        }
    }
}