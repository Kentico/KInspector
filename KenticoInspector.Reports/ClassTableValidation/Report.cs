using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ClassTableValidation.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.ClassTableValidation
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
            ReportTags.Health,
        };

        public override ReportResults GetResults()
        {
            var instance = instanceService.CurrentInstance;

            var instanceDetails = instanceService.GetInstanceDetails(instance);

            var tablesWithMissingClass = GetResultsForTables(instanceDetails);
            var classesWithMissingTable = GetResultsForClasses();

            return CompileResults(tablesWithMissingClass, classesWithMissingTable);
        }

        private ReportResults CompileResults(IEnumerable<TableWithNoClass> tablesWithMissingClass, IEnumerable<ClassWithNoTable> classesWithMissingTable)
        {
            var tableErrors = tablesWithMissingClass.Count();
            var tableResults = new TableResult<TableWithNoClass>
            {
                Name = Metadata.Terms.DatabaseTablesWithMissingKenticoClasses,
                Rows = tablesWithMissingClass
            };

            var classErrors = classesWithMissingTable.Count();
            var classResults = new TableResult<ClassWithNoTable>
            {
                Name = Metadata.Terms.KenticoClassesWithMissingDatabaseTables,
                Rows = classesWithMissingTable
            };

            var totalErrors = tableErrors + classErrors;

            var results = new ReportResults
            {
                Data =
                {
                    tableResults,
                    classResults
                }
            };

            switch (totalErrors)
            {
                case 0:
                    results.Status = ReportResultsStatus.Good;
                    results.Summary = Metadata.Terms.NoIssuesFound;
                    break;

                default:
                    results.Status = ReportResultsStatus.Error;
                    results.Summary = Metadata.Terms.CountIssueFound.With(new { count = totalErrors });
                    break;
            }

            return results;
        }

        private IEnumerable<ClassWithNoTable> GetResultsForClasses()
        {
            var classesWithMissingTable = databaseService.ExecuteSqlFromFile<ClassWithNoTable>(Scripts.ClassesWithNoTable);
            return classesWithMissingTable;
        }

        private IEnumerable<TableWithNoClass> GetResultsForTables(InstanceDetails instanceDetails)
        {
            var tablesWithMissingClass = databaseService.ExecuteSqlFromFile<TableWithNoClass>(Scripts.TablesWithNoClass);

            var tableWhitelist = GetTableWhitelist(instanceDetails.DatabaseVersion);
            if (tableWhitelist.Count > 0)
            {
                tablesWithMissingClass = tablesWithMissingClass.Where(t => !tableWhitelist.Contains(t.TableName)).ToList();
            }

            return tablesWithMissingClass;
        }

        private List<string> GetTableWhitelist(Version version)
        {
            var whitelist = new List<string>();

            if (version.Major >= 10)
            {
                whitelist.Add("CI_Migration");
            }

            return whitelist;
        }
    }
}