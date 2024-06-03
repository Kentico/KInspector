using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.PageTypeFieldAnalysis.Models;

namespace KInspector.Reports.PageTypeFieldAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) 
            : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string>
        {
            ModuleTags.Information,
            ModuleTags.Health
        };

        public async override Task<ModuleResults> GetResults()
        {
            var pagetypeFields = await databaseService.ExecuteSqlFromFile<CmsPageTypeField>(Scripts.GetCmsPageTypeFields);
            var fieldsWithMismatchedTypes = CheckForMismatchedTypes(pagetypeFields);

            return CompileResults(fieldsWithMismatchedTypes);
        }

        private ModuleResults CompileResults(IEnumerable<CmsPageTypeField> fieldsWithMismatchedTypes)
        {
            if (!fieldsWithMismatchedTypes.Any())
            {
                return new ModuleResults
                {
                    Status = ResultsStatus.Good,
                    Summary = Metadata.Terms.Summaries?.Good,
                    Type = ResultsType.NoResults
                };
            }

            var fieldResultCount = fieldsWithMismatchedTypes.Count();
            var results = new ModuleResults
            {
                Type = ResultsType.TableList,
                Status = ResultsStatus.Information,
                Summary = Metadata.Terms.Summaries?.Information?.With(new { fieldResultCount })
            };

            results.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.TableTitles?.MatchingPageTypeFieldsWithDifferentDataTypes,
                Rows = fieldsWithMismatchedTypes
            });

            return results;
        }

        private static IEnumerable<CmsPageTypeField> CheckForMismatchedTypes(IEnumerable<CmsPageTypeField> pagetypeFields) =>
            pagetypeFields
                .Distinct()
                .GroupBy(x => x.FieldName)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .OrderBy(i => i.FieldName);
    }
}