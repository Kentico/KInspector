using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.TemplateLayoutAnalysis.Models;

namespace KInspector.Reports.TemplateLayoutAnalysis
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
            ModuleTags.Information,
            ModuleTags.PortalEngine
        };

        public async override Task<ModuleResults> GetResults()
        {
            var identicalLayouts = await databaseService.ExecuteSqlFromFile<IdenticalPageLayouts>(Scripts.GetIdenticalLayouts);

            return CompileResults(identicalLayouts);
        }

        private ModuleResults CompileResults(IEnumerable<IdenticalPageLayouts> identicalPageLayouts)
        {
            var countIdenticalPageLayouts = identicalPageLayouts.Count();
            if (countIdenticalPageLayouts == 0)
            {
                return new ModuleResults
                {
                    Type = ResultsType.NoResults,
                    Status = ResultsStatus.Information,
                    Summary = Metadata.Terms.NoIdenticalPageLayoutsFound
                };
            }

            var results = new ModuleResults
            {
                Type = ResultsType.TableList,
                Status = ResultsStatus.Information,
                Summary = Metadata.Terms.CountIdenticalPageLayoutFound?.With(new { count = countIdenticalPageLayouts })
            };
            results.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.IdenticalPageLayouts,
                Rows = identicalPageLayouts
            });

            return results;
        }
    }
}