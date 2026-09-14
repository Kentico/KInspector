using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.UnusedPageTypeSummary.Models;

namespace KInspector.Reports.UnusedPageTypeSummary
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;
        private readonly IInstanceService instanceService;
        private readonly IConfigService configService;

        public Report(
            IDatabaseService databaseService,
            IInstanceService instanceService,
            IModuleMetadataService moduleMetadataService,
            IConfigService configService
            ) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
            this.instanceService = instanceService;
            this.configService = configService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13", "30", "31");

        public override IList<string> Tags => new List<string>
        {
            ModuleTags.Information
        };

        public async override Task<ModuleResults> GetResults()
        {
            var instance = configService.GetCurrentInstance();
            var instanceDetails = instanceService.GetInstanceDetails(instance);
            bool isXbK = instanceDetails?.AdministrationDatabaseVersion?.Major > 13;

            var script = isXbK ? Scripts.GetUnusedPageTypesXbK : Scripts.GetUnusedPageTypes;
            var unusedPageTypes = await databaseService.ExecuteSqlFromFile<PageType>(script);
            var countOfUnusedPageTypes = unusedPageTypes.Count();

            var results = new ModuleResults
            {
                Type = countOfUnusedPageTypes > 0 ? ResultsType.TableList : ResultsType.NoResults,
                Status = ResultsStatus.Information,
                Summary = Metadata.Terms.CountUnusedPageType?.With(new { count = countOfUnusedPageTypes })
            };
            results.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.UnusedPageTypes,
                Rows = unusedPageTypes
            });

            return results;
        }
    }
}