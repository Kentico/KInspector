using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.PagetypeFieldsDataTypeMisMatch.Models;

namespace KenticoInspector.Reports.PagetypeFieldsDataTypeMisMatch
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;
        private readonly IInstanceService instanceService;

        public Report(IDatabaseService databaseService, IInstanceService instanceService, IReportMetadataService reportMetadataService) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
            this.instanceService = instanceService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11");

        public override IList<string> Tags => new List<string> {
            ReportTags.Information,
            ReportTags.Health
        };

        public override ReportResults GetResults()
        {
            var fieldsWithMismatchedTypes = GetFieldsWithMismatchedTypes();

            return CompileResults(fieldsWithMismatchedTypes);
        }

        private ReportResults CompileResults(IEnumerable<ClassField> fieldsWithMismatchedTypes)
        {
            var fieldErrorCount = fieldsWithMismatchedTypes.Count();
            var fieldResults = new TableResult<dynamic>()
            {
                Name = Metadata.Terms.TableTitles.FieldsWithMismatchedTypes,
                Rows = fieldsWithMismatchedTypes
            };

            var results = new ReportResults
            {
                Type = ReportResultsType.TableList
            };

            results.Data.FieldResults = fieldResults;

            switch (fieldErrorCount)
            {
                case 0:
                    results.Status = ReportResultsStatus.Good;
                    results.Summary = Metadata.Terms.Summaries.Good;
                    break;

                default:
                    results.Status = ReportResultsStatus.Information;
                    results.Summary = Metadata.Terms.Summaries.Information.With(new { fieldResultCount = fieldErrorCount });
                    break;
            }

            return results;
        }

        private IEnumerable<ClassField> GetFieldsWithMismatchedTypes()
        {
            var fieldsWithMismatchedTypes = databaseService.ExecuteSqlFromFile<ClassField>(Scripts.GetFieldsWithMismatchedTypes);

            return fieldsWithMismatchedTypes;
        }
    }
}