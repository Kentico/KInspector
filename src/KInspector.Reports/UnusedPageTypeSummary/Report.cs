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

        public Report(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string>
        {
            ModuleTags.Information
        };

        public async override Task<ModuleResults> GetResults()
        {
            var unusedPageTypes = await databaseService.ExecuteSqlFromFile<PageType>(Scripts.GetUnusedPageTypes);
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