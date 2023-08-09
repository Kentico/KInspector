using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.PageTypeFieldAnalysis.Models;

namespace KenticoInspector.Reports.PageTypeFieldAnalysis
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

        public override ReportResults GetResults()
        {
            var pagetypeFields = databaseService.ExecuteSqlFromFile<CmsPageTypeField>(Scripts.GetCmsPageTypeFields);
            var fieldsWithMismatchedTypes = CheckForMismatchedTypes(pagetypeFields);

            return CompileResults(fieldsWithMismatchedTypes);
        }
        

        private ReportResults CompileResults(IEnumerable<CmsPageTypeField> fieldsWithMismatchedTypes)
        {
            if (!fieldsWithMismatchedTypes.Any())
            {
                return new ReportResults
                {
                    Status = ResultsStatus.Good,
                    Summary = Metadata.Terms.Summaries.Good
                };
            }

            var fieldResultCount = fieldsWithMismatchedTypes.Count();
            var fieldResults = new TableResult<dynamic>()
            {
                Name = Metadata.Terms.TableTitles.MatchingPageTypeFieldsWithDifferentDataTypes,
                Rows = fieldsWithMismatchedTypes
            };

            var results = new ReportResults
            {
                Type = ResultsType.TableList,
                Status = ResultsStatus.Information,
                Summary = Metadata.Terms.Summaries.Information.With(new { fieldResultCount }),
                
            };

            results.Data.FieldResults = fieldResults;

            return results;
        }

        private IEnumerable<CmsPageTypeField> CheckForMismatchedTypes(IEnumerable<CmsPageTypeField> pagetypeFields)
        {
            var fieldsWithMismatchedTypes =
                pagetypeFields
                    .Distinct()
                    .GroupBy(x => x.FieldName)
                    .Where(g => g.Count() > 1)
                    .SelectMany(g => g)
                    .OrderBy(i => i.FieldName);

            return fieldsWithMismatchedTypes;
        }
    }
}